using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImageQuantization
{
    class prim_algo
    {
        MinHeap minheap; 
        int num_of_vertices;
        bool[] is_in_heap;
        int[] root;
        double[] min_weights;
        double[] key;

        public prim_algo(int num_vertics)
        { 
            num_of_vertices = num_vertics;               //O(1)
            is_in_heap = new bool[num_vertics];          //O(1)
            minheap = new MinHeap(num_vertics);          //O(1)
            root = new int[num_vertics];                 //O(1)
            min_weights = new double[num_vertics];       //O(1)
            key = new double[num_vertics];               //O(1)
        }

        public List<Edge> graphOfMST(List<int>distinct_colors)
        {
            List<Edge> adjaceny_list = new List<Edge>();

            primMst(distinct_colors);                         // O(E log(V))
            for (int i = 0; i < num_of_vertices; i++)         //O(V)
            {
                if (root[i] >= 0)                             //O(1)
                {
                    Edge temp = new Edge(distinct_colors[root[i]], distinct_colors[i], min_weights[i]);  //O(1)
                    adjaceny_list.Add(temp);
                }
            }

            List<Edge> sorted = adjaceny_list.OrderBy(x => x.weight).ToList();       //O(E log(E))
            return sorted;
        }

        public void primMst(List<int> distinct)             // O(E log(V))
        {
            // Initializing needed Variables
            for (int i = 0; i < num_of_vertices; i++)           //O(V)
            {
                // Constructing the Minimum Heap
                Node temp = new Node();                 //O(1)
                temp.vertex = i;                        //O(1)
                if (i!=0)                               //O(1)
                    temp.weight = double.MaxValue;      //O(1)
                else
                {
                    temp.weight = 0;                     //O(1)
                }
                minheap.insert(temp);                    //O(1)
                root[i] = int.MaxValue;                  //O(1)
                min_weights[i] = double.MaxValue;        //O(1)
                is_in_heap[i] = true; // bec it is has been put in the heap
                key[i] = int.MaxValue;                   //O(1)
            }
            root[0] = -1;                                //O(1)
            min_weights[0] = 0;                          //O(1)
            while (!minheap.isEmpty())                   //Log(V)
            {
                Node minimum = minheap.extractMin();                    //O(1)
                int vertex_of_minimum_node = minimum.vertex;            //O(1)
                is_in_heap[vertex_of_minimum_node] = false; // we removed it from the heap

                for (int i = 0; i < num_of_vertices; i++)                //O(V)
                {
                    if(is_in_heap[i])                      //O(1)
                    {

                        double temp_weight = ImageOperations.Distance_between_two_colors(distinct[vertex_of_minimum_node], distinct[i]);

                        if (temp_weight < key[i])            //O(1)
                        {
                            // we are updating the value of the 
                            int at_index = minheap.indices[i];                   //O(1)
                            Node temp_node = minheap.node[at_index];             //O(1)
                            temp_node.weight = temp_weight;                      //O(1)
                            minheap.bubbleUp(at_index); // changing its position in the heap

                            root[i] = vertex_of_minimum_node;                     //O(1)
                            min_weights[i] = temp_weight;                         //O(1)
                            key[i] = temp_weight;                                 //O(1)
                        }
                    }
                }
            }
        }

        public double min_sum_MST()          //O(V)
        {
            double total_sum = 0;            //O(1)

            for (int i = 0; i < num_of_vertices; i++)  //O(V)
                total_sum += min_weights[i];           //O(1)

            return Math.Round(total_sum, 2);           //O(1)
        }
    }
}
