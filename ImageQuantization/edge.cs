using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageQuantization
{
    class Edge
    {
        public int to, from;
        public double weight;
        public Edge(int to, int from, double weight)
        {
            this.to = to;
            this.from = from;
            this.weight = weight;
        }
    }
}
