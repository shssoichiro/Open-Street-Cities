using Mapper.Curves;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

namespace Mapper.OSM
{
    public class OSMInterface
    {
        public RoadMapping mapping;
        private readonly FitCurves fc;

        public Dictionary<long, Vector2> nodes = new Dictionary<long, Vector2>();

        public Dictionary<OSMRoadTypes, LinkedList<Way>> ways = new Dictionary<OSMRoadTypes, LinkedList<Way>>();
        public SortedDictionary<OSMRoadTypes, int> roadTypeCount = new SortedDictionary<OSMRoadTypes, int>();

        double tolerance = .1f;
        double curveError = .1f;
        double scale = 1f;

        public OSMInterface(string path, double tolerance, double curveTolerance, double tiles, double scale)
        {
            this.tolerance = tolerance;
            this.curveError = curveTolerance;
            this.scale = scale;
            mapping = new RoadMapping(tiles);
            fc = new FitCurves();

            var serializer = new XmlSerializer(typeof(osm));
            var reader = new StreamReader(path);

            var osm = (osm)serializer.Deserialize(reader);
            reader.Dispose();

            Init(osm);
        }

        private void Init(osm osm)
        {
            mapping.InitBoundingBox(osm.bounds, scale);

            nodes.Clear();
            foreach (var node in osm.node)
            {
                if (!nodes.ContainsKey(node.id) && node.lat != 0 && node.lon != 0)
                {
                    Vector2 pos = Vector2.zero;
                    if (mapping.GetPos(node.lon, node.lat, ref pos))
                    {
                        nodes.Add(node.id, pos);
                    }
                }
            }

            ways.Clear();
            roadTypeCount.Clear();
            foreach (var way in osm.way)
            {
                RoadTypes rt = RoadTypes.None;
                List<long> points = null;
                int layer = 0;
                OSMRoadTypes osmrt = OSMRoadTypes.unclassified; 

                string streetName = "noname";
                if (way != null && way.tag != null) {
                    foreach (var tag in way.tag) {
                        if (tag != null) {
                            if (tag.k.Trim().ToLower() == "name") {
                                streetName = tag.v;
                            }
                        }
                    }
                }
                if (mapping.Mapped(way, ref points, ref rt, ref osmrt, ref layer))
                {
                    if (roadTypeCount.ContainsKey(osmrt)) {
                        roadTypeCount[osmrt] += points.Count;
                    } else {
                        roadTypeCount.Add(osmrt, points.Count);
                    }

                    var currentList = new List<long>();
                    for (var i = 0; i < points.Count; i += 1)
                    {
                        var pp = points[i];
                        if (nodes.ContainsKey(pp))
                        {
                            currentList.Add(pp);
                        }
                        else
                        {
                            if (currentList.Count() > 1 || currentList.Contains(pp))
                            {
                                if (!ways.ContainsKey(osmrt)) {
                                    ways.Add(osmrt, new LinkedList<Way>());
                                }

                                ways[osmrt].AddLast(new Way(currentList, rt, layer, streetName));
                                currentList = new List<long>();
                            }
                        }

                    }
                    if (currentList.Count() > 1)
                    {
                        if (!ways.ContainsKey(osmrt)) {
                            ways.Add(osmrt, new LinkedList<Way>());
                        }
                        ways[osmrt].AddLast(new Way(currentList, rt, layer, streetName));
                    }
                }
            }
            
            var intersection = new Dictionary<OSMRoadTypes, Dictionary<long, List<Way>>>();
            var allSplits = new Dictionary<OSMRoadTypes, Dictionary<Way, List<int>>>();
            foreach (var rt in ways) {
                foreach (var ww in ways[rt.Key]) {
                    foreach (var pp in ww.nodes) {
                        if (!intersection.ContainsKey(rt.Key)) {
                            intersection.Add(rt.Key, new Dictionary<long, List<Way>>());
                        }
                        if (!intersection[rt.Key].ContainsKey(pp)) {
                            intersection[rt.Key].Add(pp, new List<Way>());
                        }
                        intersection[rt.Key][pp].Add(ww);
                    }
                }
            }
            foreach (var rt in ways) {
                foreach (var inter in intersection[rt.Key]) {
                    if (inter.Value.Count > 1) {
                        foreach (var way in inter.Value) {
                            if (!allSplits.ContainsKey(rt.Key)) {
                                allSplits.Add(rt.Key, new Dictionary<Way, List<int>>());
                            }

                            if (!allSplits[rt.Key].ContainsKey(way)) {
                                allSplits[rt.Key].Add(way, new List<int>());
                            }
                            allSplits[rt.Key][way].Add(way.nodes.IndexOf(inter.Key));
                        }
                    }
                }
            }
            foreach (var rt in allSplits) {
                foreach (var waySplits in allSplits[rt.Key]) {
                    SplitWay(waySplits.Key, rt.Key, waySplits.Value);
                }
            }
            
            BreakWaysWhichAreTooLong();
            SimplifyWays();

        }

        private void BreakWaysWhichAreTooLong()
        {
            var allSplits = new Dictionary<Way, List<int>>();
            foreach (var rt in ways) {
                foreach (Way way in ways[rt.Key]) {
                    
                    float length = 0f;
                    if (way.nodes.Count() > 1) {
                        for (var i = 0; i < way.nodes.Count() - 1; i += 1) {
                            length += (nodes[way.nodes[i + 1]] - nodes[way.nodes[i]]).magnitude;
                        }
                    }
                    int segments = Mathf.FloorToInt(length / 100f) + 1;
                    float averageLength = length / (float)segments;
                    if (segments <= 1) {
                        continue;
                    }
                    length = 0;
                    var splits = new List<int>();
                    if (way.nodes.Count() > 2) {
                        for (var i = 0; i < way.nodes.Count() - 1; i += 1) {
                            length += (nodes[way.nodes[i + 1]] - nodes[way.nodes[i]]).magnitude;
                            if (length > averageLength && i != way.nodes.Count - 2) {
                                splits.Add(i + 1);
                                length = 0;
                            }
                        }
                    }
                    if (splits.Any()) {
                        allSplits.Add(way, splits);
                    }
                }

                foreach (var waySplits in allSplits) {
                    SplitWay(waySplits.Key, rt.Key, waySplits.Value);
                }
            }
        }


        private void SplitWay(Way way, OSMRoadTypes rt, List<int> splits)
        {
            /*splits = splits.OrderBy(c => c).ToList();
            var index = ways[rt].Find(way);
            if (way.nodes.Count > 1) {
                for (var i = 0; i < splits.Count(); i += 1) {
                    var nextIndex = way.nodes.Count() - 1;
                    if (i != splits.Count - 1) {
                        nextIndex = splits[i + 1];
                    }
                    var newWay = new Way(way.nodes.GetRange(splits[i], 1 + nextIndex - splits[i]), way.roadTypes, way.layer, way.name);
                    ways[rt].AddAfter(index, newWay);
                }
                way.nodes.RemoveRange(splits[0] + 1, way.nodes.Count() - splits[0] - 1);
            }*/
        }


        private void SimplifyWays()
        {
            OSMRoadTypes[] rtArr = new OSMRoadTypes[ways.Count];
            ways.Keys.CopyTo(rtArr, 0);

            foreach (var rt in rtArr) {
                foreach (var way in ways[rt]) {

                    var points = new List<Vector2>();
                    foreach (var pp in way.nodes) {
                        points.Add(nodes[pp]);
                    }

                    List<Vector2> simplified;
                    simplified = Douglas.DouglasPeuckerReduction(points, tolerance);
                    if (simplified != null && simplified.Count > 1) {
                        way.Update(fc.FitCurve(simplified.ToArray(), curveError));
                    } else {
                    }
                }

                var newList = new LinkedList<Way>();
                foreach (var way in ways[rt])
                {
                    if (way.valid)
                    {
                        newList.AddLast(way);
                    }
                    this.ways[rt] = newList;
                }

            }
        }



    }
}
