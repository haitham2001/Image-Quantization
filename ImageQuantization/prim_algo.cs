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
            num_of_vertices = num_vertics;               //Θ(1)
            is_in_heap = new bool[num_vertics];          //Θ(1)
            minheap = new MinHeap(num_vertics);          //Θ(1)
            root = new int[num_vertics];                 //Θ(1)
            min_weights = new double[num_vertics];       //Θ(1)
            key = new double[num_vertics];               //Θ(1)
        }

        public List<Edge> graphOfMST(List<int>distinct_colors)
        {
            List<Edge> adjaceny_list = new List<Edge>();

            primMst(distinct_colors);                         // O(V log(V))
            for (int i = 0; i < num_of_vertices; i++)         //Θ(V)
            {
                if (root[i] >= 0)                             //Θ(1)
                {
                    Edge temp = new Edge(distinct_colors[root[i]], distinct_colors[i], min_weights[i]);  //Θ(1)
                    adjaceny_list.Add(temp); //Θ(1)
                }
            }

            List<Edge> sorted = adjaceny_list.OrderBy(x => x.weight).ToList();    //O(V log (V))
            return sorted;
        }

        public void primMst(List<int> distinct)             // O(V log(V))
        {
            // Initializing needed Variables
            double distinctColor_B = System.Environment.TickCount;

           
            for (int i = 0; i < num_of_vertices; i++)           //Θ(V)
            {
                // Constructing the Minimum Heap
                Node temp = new Node();                 //Θ(1)
                temp.vertex = i;                        //Θ(1)
                if (i!=0)                               //Θ(1)
                    temp.weight = double.MaxValue;      //Θ(1)
                else
                {
                    temp.weight = 0;                     //Θ(1)
                }
                minheap.insert(temp);                    //Θ(1)
                root[i] = int.MaxValue;                  //Θ(1)
                min_weights[i] = double.MaxValue;        //Θ(1)
                is_in_heap[i] = true; //Θ(1)  // bec it is has been put in the heap
                key[i] = int.MaxValue; //Θ(1)
            }

            double distinctColor_a = System.Environment.TickCount; //Θ(1)
            double distinctColor_r = distinctColor_a - distinctColor_B; //Θ(1)
            distinctColor_r /= 1000; //Θ(1)
            var s = distinctColor_r;  //Θ(1)
            root[0] = -1;                                //Θ(1)
            min_weights[0] = 0;                          //Θ(1)
            while (!minheap.isEmpty())                   //O(Log(V)) --> Θ(V*LogV)
            {
                Node minimum = minheap.extractMin();                    //Θ(1)
                int vertex_of_minimum_node = minimum.vertex;            //Θ(1)
                is_in_heap[vertex_of_minimum_node] = false;             //Θ(1)      // we removed it from the heap

                for (int i = 0; i < num_of_vertices; i++)                //Θ(V)
                {
                    if(is_in_heap[i])                      //Θ(1)
                    {

                        double temp_weight = ImageOperations.Distance_between_two_colors(distinct[vertex_of_minimum_node], distinct[i]); //Θ(1)

                        if (temp_weight < key[i])            //Θ(1)
                        {
                            // we are updating the value of the 
                            int at_index = minheap.indices[i];                   //Θ(1)
                            Node temp_node = minheap.node[at_index];             //Θ(1)
                            temp_node.weight = temp_weight;                      //Θ(1)
                            minheap.bubbleUp(at_index); //Θ(1) // changing its position in the heap

                            root[i] = vertex_of_minimum_node;                     //Θ(1)
                            min_weights[i] = temp_weight;                         //Θ(1)
                            key[i] = temp_weight;                                 //Θ(1)
                        }
                    }
                }
            }
        }

        public double min_sum_MST()          //Θ(V)
        {
            double total_sum = 0;            //Θ(1)

            for (int i = 0; i < num_of_vertices; i++)  //Θ(V)
                total_sum += min_weights[i];           //Θ(1)

            return Math.Round(total_sum, 2);           //Θ(1)
        }
    }
}
