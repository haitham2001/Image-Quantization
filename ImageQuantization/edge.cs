using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageQuantization
{
    class edge
    {
        public int next, first;
        public double weight;
        public edge(int next, int first, double weight)
        {
            this.next = next;
            this.first = first;
            this.weight = weight;
        }
    }
}
