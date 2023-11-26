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

        public List<Way> allWays = new List<Way>();
        public Dictionary<OSMRoadTypes, LinkedList<Way>> ways = new Dictionary<OSMRoadTypes, LinkedList<Way>>();
        public SortedDictionary<OSMRoadTypes, int> roadTypeCount = new SortedDictionary<OSMRoadTypes, int>();

        double tolerance = 1f;
        double curveError = .5f;
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

            // get nodes from OSM
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

            // get ways from OSM
            ways.Clear();
            roadTypeCount.Clear();
            foreach (var way in osm.way)
            {
                RoadTypes rt = RoadTypes.None;
                List<long> points = null;
                int layer = 0;
                OSMRoadTypes osmrt = OSMRoadTypes.unclassified;

                string streetName = "noname";
                if (way != null && way.tag != null)
                {
                    foreach (var tag in way.tag)
                    {
                        if (tag != null && tag.k.Trim().ToLower() == "name")
                        {
                            streetName = tag.v;
                        }
                    }
                }
                if (mapping.Mapped(way, ref points, ref rt, ref osmrt, ref layer))
                {
                    if (roadTypeCount.ContainsKey(osmrt))
                    {
                        roadTypeCount[osmrt] += points.Count;
                    }
                    else
                    {
                        roadTypeCount.Add(osmrt, points.Count);
                    }

                    var way_points = new List<long>();
                    for (var i = 0; i < points.Count; i += 1)
                    {
                        var pp = points[i];
                        if (nodes.ContainsKey(pp))
                        {
                            way_points.Add(pp);
                        }
                        else
                        {
                            if (way_points.Count() > 1 || way_points.Contains(pp))
                            {
                                if (!ways.ContainsKey(osmrt))
                                {
                                    ways.Add(osmrt, new LinkedList<Way>());
                                }

                                Way w = new Way(way_points, rt, osmrt, layer, streetName);
                                ways[osmrt].AddLast(w);
                                allWays.Add(w);

                                way_points = new List<long>();
                            }
                        }

                    }
                    if (way_points.Count() > 1)
                    {
                        if (!ways.ContainsKey(osmrt))
                        {
                            ways.Add(osmrt, new LinkedList<Way>());
                        }
                        Way w = new Way(way_points, rt, osmrt, layer, streetName);
                        ways[osmrt].AddLast(w);
                        if (allWays.IndexOf(w) == -1)
                        {
                            allWays.Add(w);
                        }

                    }
                }
            }

            allWays = new List<Way>();
            foreach (var rt in ways)
            {
                foreach (Way way in ways[rt.Key])
                {
                    if (allWays.IndexOf(way) == -1)
                    {
                        allWays.Add(way);
                    }
                }
            }

            var intersections = new Dictionary<long, List<Way>>();
            var allSplits = new Dictionary<Way, List<int>>();

            foreach (var ww in allWays)
            {
                foreach (var pp in ww.nodes)
                {
                    if (!intersections.ContainsKey(pp))
                    {
                        intersections.Add(pp, new List<Way>());
                    }
                    intersections[pp].Add(ww);
                }
            }
            foreach (var inter in intersections)
            {
                if (inter.Value.Count > 1)
                {
                    foreach (var way in inter.Value)
                    {
                        if (!allSplits.ContainsKey(way))
                        {
                            allSplits.Add(way, new List<int>());
                        }
                        allSplits[way].Add(way.nodes.IndexOf(inter.Key));
                    }
                }
            }
            foreach (var waySplits in allSplits)
            {
                SplitWay(waySplits.Key, waySplits.Value);
            }

            BreakWaysWhichAreTooLong();
            MergeWaysWhichAreTooShort();
            SimplifyWays();

        }

        private void MergeWaysWhichAreTooShort()
        {
            foreach (Way way in allWays)
            {
                int lastIndex = way.nodes.Count() - 1;
                bool removedPrev = false;
                float accumulateDist = 0f;
                for (int i = lastIndex - 1; i >= 0; i--)
                {
                    float dist = Vector2.SqrMagnitude(nodes[way.nodes[i]] - nodes[way.nodes[i + 1]]);

                    if (removedPrev)
                    {
                        dist += accumulateDist;
                    }

                    if (dist < 90f)
                    {
                        if (i == 0 && lastIndex > 1)
                        {
                            way.nodes.RemoveRange(1, 1);
                        }
                        else
                        {
                            way.nodes.RemoveRange(i, 1);
                            removedPrev = true;
                            accumulateDist = dist;
                        }
                    }
                    else
                    {
                        accumulateDist = 0f;
                        removedPrev = false;
                    }
                }
            }
        }

        private void BreakWaysWhichAreTooLong()
        {
            var allSplits = new Dictionary<Way, List<int>>();
            foreach (Way way in allWays)
            {
                float length = 0f;
                if (way.nodes.Count() > 1)
                {
                    for (var i = 0; i < way.nodes.Count() - 1; i += 1)
                    {
                        length += (nodes[way.nodes[i + 1]] - nodes[way.nodes[i]]).magnitude;
                    }
                }
                int segments = Mathf.FloorToInt(length / 100f) + 1;
                float averageLength = length / (float)segments;
                if (segments <= 1)
                {
                    continue;
                }
                length = 0;
                var splits = new List<int>();
                if (way.nodes.Count() > 1)
                {
                    for (var i = 0; i < way.nodes.Count() - 1; i += 1)
                    {
                        length += (nodes[way.nodes[i + 1]] - nodes[way.nodes[i]]).magnitude;
                        if (length > averageLength && i != way.nodes.Count - 2)
                        {
                            splits.Add(i + 1);
                            length = 0;
                        }
                    }
                }
                if (splits.Any())
                {
                    allSplits.Add(way, splits);
                }
            }

            foreach (var waySplits in allSplits)
            {
                SplitWay(waySplits.Key, waySplits.Value);
            }
        }

        private void SplitWay(Way way, List<int> splits)
        {
            if (splits.Count > 0)
            {
                splits.Sort();
                int index = allWays.IndexOf(way);
                if (way.nodes.Count > 1)
                {
                    for (int i = 0; i < splits.Count(); i++)
                    {
                        var nextIndex = way.nodes.Count() - 1;
                        if (i != splits.Count - 1)
                        {
                            nextIndex = splits[i + 1];
                        }
                        var newWay = new Way(way.nodes.GetRange(splits[i], 1 + nextIndex - splits[i]), way.roadType, way.osmRoadType, way.layer, way.name);
                        ways[way.osmRoadType].AddLast(newWay);
                        allWays.Add(newWay);

                    }
                    way.nodes.RemoveRange(splits[0] + 1, way.nodes.Count() - splits[0] - 1);
                }
            }
        }

        private void SimplifyWays()
        {
            OSMRoadTypes[] rtArr = new OSMRoadTypes[ways.Count];
            ways.Keys.CopyTo(rtArr, 0);

            foreach (var rt in rtArr)
            {
                foreach (Way way in ways[rt])
                {
                    var points = new List<Vector2>();
                    foreach (long pp in way.nodes)
                    {
                        points.Add(nodes[pp]);
                    }

                    List<Vector2> simplified = Douglas.DouglasPeuckerReduction(points, tolerance);
                    if (simplified != null && simplified.Count > 1)
                    {
                        way.Update(fc.FitCurve(simplified.ToArray(), curveError));
                    }
                }

                var newList = new LinkedList<Way>();
                foreach (var way in ways[rt])
                {
                    if (way.valid)
                    {
                        newList.AddLast(way);
                    }
                    ways[rt] = newList;
                }

            }
        }
    }
}