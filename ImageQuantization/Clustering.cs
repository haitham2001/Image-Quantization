using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageQuantization
{
    class Clustering
    {
        int k; // k clusters
        List<Edge> edges; // our edges that we got from the tree
        List<RGBPixel> num_of_colors;
        int[] parent; // to check if a vertex have a parent or no
        List<int>[] adjacency_list;
        Dictionary<RGBPixel, bool> list;

        public Clustering(int k, List<Edge> edges, List<RGBPixel> distinct_colors, int[] parent)
        {
            this.k = k;
            this.edges = edges;
            num_of_colors = distinct_colors;
            this.parent = parent;
            adjacency_list = new List<int>[num_of_colors.Count];
            list = new Dictionary<RGBPixel, bool>();
            for (int i = 0; i < num_of_colors.Count; i++)
            {
                list.Add(distinct_colors[i], false);
                adjacency_list[i] = new List<int>();
            }
        }

        public void getClusters()
        {
            //O(k * d)
            while (k != 0)//O(k)
            {
                int index_of_max = 0;
                double max = -1;
                for (int i = 0; i < num_of_colors.Count; i++)//O(D)
                {
                    double current_weight = edges[i].weight;
                    if (current_weight > max)
                    {
                        max = current_weight;
                        index_of_max = i;
                    }
                }
                edges[index_of_max].weight = -1;
                parent[index_of_max] = index_of_max;
                k--;
            }

            for (int i = 0; i < num_of_colors.Count; i++)
            {
                if(parent[i]!=i)
                {
                    adjacency_list[i].Add(parent[i]);
                    adjacency_list[parent[i]].Add(i);
                }
            }

            for (int i = 0; i < num_of_colors.Count; i++)
            {
                if(list[num_of_colors[i]]==false)
                {
                    DFS(num_of_colors[i]);

                }
            }
        }

        public void DFS(RGBPixel index)
        {
            list[index] = true;

            foreach(int i in adjacency_list[index])
            {
                if(list[num_of_colors[i]] == false)
                {
                    DFS(num_of_colors[i]);
                }
            }

        }

        public List<RGBPixel> colorPallette()
        {
            List<RGBPixel> colors = new List<RGBPixel>();
            RGBPixel color;

            return colors;
        }
        
    }
}

