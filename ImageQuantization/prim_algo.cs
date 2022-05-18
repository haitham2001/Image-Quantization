using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageQuantization
{
    class prim_algo
    {
        MinHeap minheap;
        int num_vertics;
        bool []isvisited;
        public List<Edge> adjlist;
        int[] root;
        double[] min_weights;
        //double[] weight;


        public prim_algo(int num_vertics)
        { 
            this.num_vertics = num_vertics;
            isvisited = new bool[num_vertics];
            minheap = new MinHeap(num_vertics);
            adjlist = new List<Edge>();
            root = new int[num_vertics];
            min_weights = new double[num_vertics];
        }

        public void primMst(RGBPixel[]colors)
        {


    


        }




    }
}
