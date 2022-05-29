using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageQuantization
{
    public class MinHeap
    {
        //size of total MinHeap
        public int size;
        //size of current filled nodes
        public int num_filled;
        // each node of MinHeap
        public Node[] node;
        //will be used to decrease the key
        public int[] indices; 

        public MinHeap(int size) //Θ(1)
        {
            this.size = size;   //Θ(1)
            node = new Node[size + 1]; //Θ(1)
            indices = new int[size]; //Θ(1)
            node[0] = new Node(); //Θ(1)
            node[0] = new Node(); //Θ(1)
            node[0].weight = int.MinValue; //Θ(1)
            node[0].vertex = -1; //Θ(1)
            num_filled = 0; //Θ(1)
        }

        // insert node in MinHeap
        public void insert(Node x) //Θ(1)
        {
            num_filled++; //Θ(1)
            int idx = num_filled; //Θ(1)
            node[idx] = x; //Θ(1)
            indices[x.vertex] = idx; //Θ(1)
            bubbleUp(idx); //Θ(1)
        }

        // raises the Min node in MinHeap
        public void bubbleUp(int position) //??
        {
            int root_Index = position / 2; //Θ(1)
            int cuurent_Index = position; //Θ(1)
            while (cuurent_Index > 0 && node[root_Index].weight > node[cuurent_Index].weight) //??
            {
                Node currentNode = node[cuurent_Index]; //Θ(1)
                Node parentNode = node[root_Index]; //Θ(1)
                //swap the position
                indices[currentNode.vertex] = root_Index; //Θ(1)
                indices[parentNode.vertex] = cuurent_Index; //Θ(1)
                swap(cuurent_Index, root_Index); //Θ(1)
                cuurent_Index = root_Index; //Θ(1)
                root_Index = root_Index / 2; //Θ(1)
            }
        }

        public Node extractMin() //Θ(1)
        {
            Node min = node[1]; //Θ(1)
            Node lastNode = node[num_filled]; //Θ(1)
            //update the indexes[] and move the last node to the top
            indices[lastNode.vertex] = 1; //Θ(1)
            node[1] = lastNode; //Θ(1)
            node[num_filled] = null; //Θ(1)
            sinkDown(1); //Θ(1)
            num_filled--; //Θ(1)
            return min; //Θ(1)
        }

        public void sinkDown(int k) //Θ(1)
        {
            int smallest = k; //Θ(1)
            int left_child_index = 2 * k; //Θ(1)
            int right_child_index = 2 * k + 1; //Θ(1)

            if (left_child_index < heapSize() && node[smallest].weight > node[left_child_index].weight) //Θ(1)
            {
                smallest = left_child_index; //Θ(1)
            }

            if (right_child_index < heapSize() && node[smallest].weight > node[right_child_index].weight) //Θ(1)
            {
                smallest = right_child_index; //Θ(1)
            }

            if (smallest != k) //Θ(1)
            {
                Node smallestNode = node[smallest]; //Θ(1)
                Node kNode = node[k]; //Θ(1)
                //swap the positions
                indices[smallestNode.vertex] = k; //Θ(1)
                indices[kNode.vertex] = smallest; //Θ(1)
                swap(k, smallest); //Θ(1)
                sinkDown(smallest); //Θ(1)
            }
        }

        public void swap(int a, int b) //Θ(1)
        {
            Node temp = node[a]; //Θ(1)
            node[a] = node[b]; //Θ(1)
            node[b] = temp; //Θ(1)
        }

        public bool isEmpty() //Θ(1)
        {
            return num_filled == 0; //Θ(1)
        }

        public int heapSize() //Θ(1)
        {
            return num_filled; //Θ(1)
        }

    }
}
