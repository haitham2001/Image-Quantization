using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageQuantization
{
    class Edge
    {
        public color to, from;
        public double weight;
        public Edge(color to, color from, double weight)
        {
            this.to = to;
            this.from = from;
            this.weight = weight;
        }
    }


}
