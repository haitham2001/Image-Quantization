﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageQuantization
{
    class MinHeap
    {
        //size of total MinHeap
        int size;
        //size of current filled nodes
        int num_filled;
        // each node of MinHeap
        Node[] node;
        //will be used to decrease the key
        int[] indcies; 

        public MinHeap(int size)
        {
            this.size = size;
            node = new Node[size + 1];
            indcies = new int[size];
            node[0] = new Node();
            node[0].weight = int.MinValue;
            node[0].vertex = -1;
            num_filled = 0;
        }

        // insert node in MinHeap
        public void insert(Node x)
        {
            num_filled++;
            int idx = num_filled;
            node[idx] = x;
            indcies[x.vertex] = idx;
            bubbleUp(idx);
        }

        // raises the Min node in MinHeap
        public void bubbleUp(int pos)
        {
            int parentIdx = pos / 2;
            int currentIdx = pos;
            while (currentIdx > 0 && node[parentIdx].weight > node[currentIdx].weight)
            {
                Node currentNode = node[currentIdx];
                Node parentNode = node[parentIdx];
                //swap the position
                indcies[currentNode.vertex] = parentIdx;
                indcies[parentNode.vertex] = currentIdx;
                swap(currentIdx, parentIdx);
                currentIdx = parentIdx;
                parentIdx = parentIdx / 2;
            }
        }

        public Node extractMin()
        {
            Node min = node[1];
            Node lastNode = node[num_filled];
            //update the indexes[] and move the last node to the top
            indcies[lastNode.vertex] = 1;
            node[1] = lastNode;
            node[num_filled] = null;
            sinkDown(1);
            num_filled--;
            return min;
        }

        public void sinkDown(int k)
        {
            int smallest = k;
            int leftChildIdx = 2 * k;
            int rightChildIdx = 2 * k + 1;

            if (leftChildIdx < heapSize() && node[smallest].weight > node[leftChildIdx].weight)
            {
                smallest = leftChildIdx;
            }

            if (rightChildIdx < heapSize() && node[smallest].weight > node[rightChildIdx].weight)
            {
                smallest = rightChildIdx;
            }

            if (smallest != k)
            {
                Node smallestNode = node[smallest];
                Node kNode = node[k];
                //swap the positions
                indcies[smallestNode.vertex] = k;
                indcies[kNode.vertex] = smallest;
                swap(k, smallest);
                sinkDown(smallest);
            }
        }

        public virtual void swap(int a, int b)
        {
            Node temp = node[a];
            node[a] = node[b];
            node[b] = temp;
        }

        public  bool Empty
        {
            get
            {
                return num_filled == 0;
            }
        }

        public int heapSize()
        {
            return num_filled;
        }

    }
}