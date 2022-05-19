using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageQuantization
{
    class Clustering
    {

        public static Dictionary<int, List<int>> neighbours;

        public static List<HashSet<int>> getClusters(List<Edge> MST, int num_cluster)
        {
            List<HashSet<int>> clusters = new List<HashSet<int>>();
            HashSet<int> reached = new HashSet<int>();
            int index_of_max;
            double max;
            
            for (int j = 0; j < num_cluster - 1; j++)
            {
                index_of_max = 0;
                max = 0;

                for (int i = 0; i < MST.Count; i++)
                {
                    if (MST[i].weight > max)
                    {
                        max = MST[i].weight;
                        index_of_max = i;
                    }
                }
                //Edge e = new Edge(MST[index_of_max].from, MST[index_of_max].to, 0);
                MST[index_of_max].weight = -1;
            }

            foreach (var edge in MST)
            {
                if (edge.weight != -1)
                {

                    if (neighbours.ContainsKey(edge.from))
                    {
                        neighbours[edge.from].Add(edge.to);
                    }
                    else
                    {
                        List<int> l = new List<int>();
                        l.Add(edge.to);
                        neighbours.Add(edge.from, l);
                    }
                    if (neighbours.ContainsKey(edge.to))
                    {
                        neighbours[edge.to].Add(edge.from);
                    }
                    else
                    {
                        List<int> l = new List<int>();
                        l.Add(edge.from);
                        neighbours.Add(edge.to, l);
                    }
                }
                else
                {
                    if (!neighbours.ContainsKey(edge.from))
                    {
                        List<int> l = new List<int>();
                        neighbours.Add(edge.from, l);
                    }
                    if (!neighbours.ContainsKey(edge.to))
                    {
                        List<int> l = new List<int>();
                        neighbours.Add(edge.to, l);
                    }
                }
            }
            foreach (var vertex in neighbours)
            {
                if (!reached.Contains(vertex.Key))
                {
                    HashSet<int> h = new HashSet<int>();
                    DFS(vertex.Key, ref reached, ref neighbours, ref h);
                    clusters.Add(h);
                }
            }

            return clusters;
        }

        private static void DFS(int cur, ref HashSet<int> visited, ref Dictionary<int, List<int>> neighbours, ref HashSet<int> cluster)
        {
            visited.Add(cur);
            cluster.Add(cur);
            foreach (var neighbour in neighbours[cur])
            {
                if (!visited.Contains(neighbour))
                    DFS(neighbour, ref visited, ref neighbours, ref cluster);
            }
        }

        public static List<RGBPixel> ExtractColors(List<HashSet<RGBPixel>> clusters)
        {
            List<RGBPixel> Palette = new List<RGBPixel>();
            RGBPixel avgColor;
            int redSum, greenSum, blueSum;

            for (int i = 0; i < clusters.Count; i++)
            {
                redSum = 0; greenSum = 0; blueSum = 0;

                foreach (var color in clusters[i])
                {
                    redSum += color.red;
                    greenSum += color.green;
                    blueSum += color.blue;
                }

                avgColor.red = (byte)(redSum / clusters[i].Count);
                avgColor.green = (byte)(greenSum / clusters[i].Count);
                avgColor.blue = (byte)(blueSum / clusters[i].Count);

                Palette.Add(avgColor);
            }

            return Palette;
        }

        public static RGBPixel[,] QuantizedImage(RGBPixel[,] imageMatrix, List<RGBPixel> palette)
        {
            int height = imageMatrix.GetLength(0);
            int width = imageMatrix.GetLength(1);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    imageMatrix[i, j] = calculateWeight(palette, imageMatrix[i, j]);
                }
            }


            return imageMatrix;
        }
        public static RGBPixel calculateWeight(List<RGBPixel> palette, RGBPixel myCurrentPixel)
        {
            RGBPixel returnPixel = myCurrentPixel;
            double weight = 10000000;
            int r1, g1, b1;
            int r2, g2, b2;

            for (int i = 0; i < palette.Count; i++)
            {
                r1 = palette[i].red;
                g1 = palette[i].green;
                b1 = palette[i].blue;
                r2 = myCurrentPixel.red;
                g2 = myCurrentPixel.green;
                b2 = myCurrentPixel.blue;
                double eq = Math.Sqrt((r2 - r1) * (r2 - r1) + (g2 - g1) * (g2 - g1) + (b2 - b1) * (b2 - b1));

                if (eq < weight)
                {
                    returnPixel = palette[i];
                    weight = eq;
                }
            }
            return returnPixel;
        }
        //int k; // k clusters
        //List<Edge> edges; // our edges that we got from the tree
        //List<RGBPixel> num_of_colors;
        //int[] parent; // to check if a vertex have a parent or no
        //List<int>[] adjacency_list;
        //Dictionary<RGBPixel, bool> list;

        //public Clustering(int k, List<Edge> edges, List<RGBPixel> distinct_colors, int[] parent)
        //{
        //    this.k = k;
        //    this.edges = edges;
        //    num_of_colors = distinct_colors;
        //    this.parent = parent;
        //    adjacency_list = new List<int>[num_of_colors.Count];
        //    list = new Dictionary<RGBPixel, bool>();
        //    for (int i = 0; i < num_of_colors.Count; i++)
        //    {
        //        list.Add(distinct_colors[i], false);
        //        adjacency_list[i] = new List<int>();
        //    }
        //}

        //public void getClusters()   //O(k * D)
        //{
        //    while (k != 0)//O(k)
        //    {
        //        int index_of_max = 0;
        //        double max = -1;
        //        for (int i = 0; i < num_of_colors.Count; i++)//O(D)
        //        {
        //            double current_weight = edges[i].weight;  //O(1)
        //            if (current_weight > max) //O(1)
        //            {
        //                max = current_weight;
        //                index_of_max = i;
        //            }
        //        }
        //        edges[index_of_max].weight = -1;   //O(1)
        //        parent[index_of_max] = index_of_max;  //O(1)
        //        k--;
        //    }

        //    for (int i = 0; i < num_of_colors.Count; i++)
        //    {
        //        if (parent[i] != i)
        //        {
        //            adjacency_list[i].Add(parent[i]);
        //            adjacency_list[parent[i]].Add(i);
        //        }
        //    }

            //    for (int i = 0; i < num_of_colors.Count; i++)
            //    {
            //        if(list[num_of_colors[i]]==false)
            //        {
            //            DFS(num_of_colors[i]);

            //        }
            //    }
            //}

            //public void DFS(RGBPixel index)
            //{
            //    list[index] = true;

            //    //foreach(int i in adjacency_list[index])
            //    //{
            //    //    if(list[num_of_colors[i]] == false)
            //    //    {
            //    //        DFS(num_of_colors[i]);
            //    //    }
            //    //}

            //}

            //public List<RGBPixel> colorPallette()
            //{
            //    List<RGBPixel> colors = new List<RGBPixel>();
            //    RGBPixel color;

            //    return colors;
            //}

        }
}

