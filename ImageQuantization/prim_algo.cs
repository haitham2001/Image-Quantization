using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageQuantization
{
    class prim_algo
    {
        private int num_vertix;
        private double[] _key;
        private int[] root;
        private bool[] reached;
        public List<edge> edges;

        public prim_algo(int n)  // Θ(n)
        {
            num_vertix = n;        // Θ(1)
            _key = new double[n];   // Θ(1)
            root = new int[n];   // Θ(1)
            reached = new bool[n];  // Θ(1)
            edges = new List<edge>(); // Θ(1)

            for (int i = 0; i < n; i++) // Θ(n * body)
            {
                _key[i] = Int32.MaxValue;   // Θ(1)
                root[i] = -1;            // Θ(1)
            }
        }

        public double MST(HashSet<RGBPixel> distinctColor) 
        {
            List<RGBPixel> distinctColors = distinctColor.ToList();
            var mstCost = 0d;   // Θ(1)
            int startNode = 0, nextNode = 0; // Θ(1)
            _key[0] = 0;    // Θ(1)
            root[0] = 0;    // Θ(1)
            for (var i = 0; i < num_vertix - 1; i++) // Θ(v * body)
            {
                reached[startNode] = true;  // Θ(1)
                double miniCost = Int32.MaxValue;   // Θ(1)
                for (var j = 0; j < num_vertix; j++)   // Θ(v * body)
                {

                    if (reached[j] == false)    // Θ(1)
                    {
                        var cost = ImageOperations.Distance_between_two_colors(distinctColors[startNode], distinctColors[j]);    // Θ(1)
                        if (cost < _key[j]) // Θ(1)
                        {
                            root[j] = startNode; // Θ(1)
                            _key[j] = cost; // Θ(1)
                        }
                        if (_key[j] < miniCost) // Θ(1)
                        {
                            nextNode = j;    // Θ(1)
                            miniCost = _key[j]; // Θ(1)
                        }
                    }
                }
                startNode = nextNode;    // Θ(1)
                mstCost += _key[startNode]; // Θ(1)
                edges.Add(new edge(startNode, root[startNode], _key[startNode])); // Θ(1)

            
            }
            
            return Math.Round(mstCost, 2);  // Θ(1)
        }
    }
}
