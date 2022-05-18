using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageQuantization
{
    class PriorityQueue
    {
        int Max, Current_Vertex;
        int[] heap, Index;
        double[] Keys;
        public PriorityQueue(int Max)
        {
            this.Max = Max;
            Current_Vertex = 0;
            heap = new int[Max + 5];
            Index = new int[Max + 5];
            Keys = new double[Max + 5];
            for (int i = 0; i < Max + 5; ++i)
                Index[i] = -1;
        }

        public bool IsEmpty()
        {
            return (Current_Vertex == 0);
        }

        public bool Contains(int x)
        {
            return (Index[x] != -1);
        }

        public void Insert(int i, double key)
        {
            Current_Vertex++;
            Index[i] = Current_Vertex;
            heap[Current_Vertex] = i;
            Keys[i] = key;
            BubbleUp(Current_Vertex);
        }

        public int DeleteMin()
        {
            int min = heap[1];
            Swap(1, Current_Vertex--);
            BubbleDown(1);
            Index[min] = -1;
            heap[Current_Vertex + 1] = -1;
            return min;
        }

        public void DecreaseKey(int i, double key)
        {
            Keys[i] = key;
            BubbleUp(Index[i]);
        }
        private void BubbleUp(int k)
        {
            while (k > 1 && Keys[heap[k / 2]] > Keys[heap[k]])
            {
                Swap(k, k / 2);
                k = k / 2;
            }
        }
        private void Swap(int i, int j)
        {
            int t = heap[i];
            heap[i] = heap[j];
            heap[j] = t;
            Index[heap[i]] = i; Index[heap[j]] = j;
        }
        private void BubbleDown(int k)
        {
            int j;
            while (2 * k <= Current_Vertex)
            {
                j = 2 * k;
                if (j < Current_Vertex && Keys[heap[j]] > Keys[heap[j + 1]])
                    ++j;
                if (Keys[heap[k]] <= Keys[heap[j]])
                    break;
                Swap(k, j);
                k = j;
            }
        }
    }
}
