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
        public Edge(int to, int from, double weight) //Θ(1)
        {
            this.to = to; //Θ(1)
            this.from = from; //Θ(1)
            this.weight = weight; //Θ(1)
        }
    }
}
