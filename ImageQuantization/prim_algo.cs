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
            this.num_of_vertices = num_vertics;
            is_in_heap = new bool[num_vertics];
            minheap = new MinHeap(num_vertics);
            root = new int[num_vertics];
            min_weights = new double[num_vertics];
            key = new double[num_vertics];
        }

        public List<Edge> graphOfMST(RGBPixel[] distinct_colors)
        {
            List<Edge> adjaceny_list = new List<Edge>();
            
            primMst(distinct_colors);
            for (int i = 0; i < num_of_vertices; i++)
            {
                Edge temp = new Edge(root[i],i,min_weights[i]);
                adjaceny_list.Add(temp);
            }
            return adjaceny_list;
        }

        public void primMst(RGBPixel[] distinct)
        {
            // Initializing needed Variables
            for (int i = 0; i < num_of_vertices; i++)
            {
                // Constructing the Minimum Heap
                Node temp = new Node();
                temp.vertex = i;
                if(i!=0)
                    temp.weight = double.MaxValue;
                else
                {
                    temp.weight = 0;
                }
                minheap.insert(temp);
                root[i] = int.MaxValue;
                min_weights[i] = double.MaxValue;
                is_in_heap[i] = true; // bec it is has been put in the heap
                key[i] = int.MaxValue;
            }
            root[0] = -1;
            min_weights[0] = 0;
            while(!minheap.isEmpty())
            {
                Node minimum = minheap.extractMin();
                int vertex_of_minimum_node = minimum.vertex;
                is_in_heap[vertex_of_minimum_node] = false; // we removed it from the heap

                for (int i = 0; i < num_of_vertices; i++) //O(v)
                {
                    if(is_in_heap[i])
                    {
                        double temp_weight = ImageOperations.Distance_between_two_colors(distinct[vertex_of_minimum_node],distinct[i]);

                        if (temp_weight < key[i])
                        {
                            // we are updating the value of the 
                            int at_index = minheap.indices[i];
                            Node temp_node = minheap.node[at_index];
                            temp_node.weight = temp_weight;
                            minheap.bubbleUp(at_index); // changing its position in the heap

                            root[i] = vertex_of_minimum_node;
                            min_weights[i] = temp_weight;
                            key[i] = temp_weight;
                        }
                    }
                }
            }
        }

        public double minimumSumOfMST()
        {
            double total_sum = 0;

            for (int i = 0; i < num_of_vertices; i++)
                total_sum += min_weights[i];

            return Math.Round(total_sum, 1);
        }
    }
}
