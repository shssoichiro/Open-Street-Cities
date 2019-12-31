using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mapper {

    public class Segment
    {
        public Vector2 startPoint;
        public Vector2 controlA;
        public Vector2 controlB;
        public Vector2 endPoint;
        
        public Segment(Vector2 startPoint, Vector2 endPoint)
        {
            this.startPoint = startPoint;
            this.endPoint = endPoint;
        }

        public Segment(Vector2[] bezCurve)
        {
            this.startPoint = bezCurve[0];
            this.controlA = bezCurve[1];
            this.controlB = bezCurve[2];
            this.endPoint = bezCurve[3];
        }
    }

    public class Way
    {
        public bool valid;
        public List<long> nodes = new List<long>();
        public List<Segment> segments;

        public RoadTypes roadTypes;
        public int layer;
        public string name;

        public long StartNode
        {
            get { return nodes[0]; }
        }

        public long EndNode
        {
            get { return nodes[nodes.Count()-1]; }
        }

        public Way(List<long> points, RoadTypes rt,int layer, string name)
        {
            this.roadTypes = rt;
            this.nodes = points;
            this.layer = layer;
            this.name = name;
        }

        internal void Update(List<Segment> list)
        {
            valid = true;
            segments = list;
        }
    }
}
