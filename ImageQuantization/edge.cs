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
        public Edge(int next, int first, double weight)
        {
            this.to = next;
            this.from = first;
            this.weight = weight;
        }
    }
}
