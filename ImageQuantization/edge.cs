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

    class edge
    {
        public RGBPixel to_rgb, from_rgb;
        public double weight_e;
        public edge(RGBPixel to_rgb, RGBPixel from_rgb, double weight_e)
        {
            this.to_rgb = to_rgb;
            this.from_rgb = from_rgb;
            this.weight_e = weight_e;
        }
    }




}
