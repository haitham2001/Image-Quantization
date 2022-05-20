using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageQuantization
{
    class Clustering
    {

        public static Dictionary<RGBPixel, List<RGBPixel>> neighbours = new Dictionary<RGBPixel, List<RGBPixel>>();

        public static List<HashSet<RGBPixel>> getClusters(List<Edge> MST, int num_cluster, List<color> distinctColor) // O(K *d)
        {
            int index_of_max;
            double max;
            List<HashSet<RGBPixel>> clusters = new List<HashSet<RGBPixel>>();
            HashSet<RGBPixel> reached = new HashSet<RGBPixel>();

            while (num_cluster != 1)
            {
                index_of_max = 0;
                max = 0;
                var count =MST.Count-1;
                while (count != 0)
                {
                    if (MST[count].weight > max)
                    {
                        max = MST[count].weight;
                        index_of_max = count;
                    }
                    count--;
                }
                MST[index_of_max].weight = -1;
                num_cluster--;
            }
            
            List<RGBPixel> enig = new List<RGBPixel>();
            for (int i = 0; i < distinctColor.Count; i++)
            {
                neighbours.Add(distinctColor[i].RGB_color, enig);
            }


            foreach (var edge in MST)
            {

                if (edge.weight != -1)
                {
                    neighbours[edge.from.RGB_color].Add(edge.to.RGB_color);
                    neighbours[edge.to.RGB_color].Add(edge.from.RGB_color);
                    //    if (neighbours.ContainsKey(edge.from))
                    //    {
                    //        neighbours[edge.from].Add(edge.to);
                    //    }
                    //    else
                    //    {
                    //        List<int> l = new List<int>();
                    //        l.Add(edge.to);
                    //        neighbours.Add(edge.from, l);
                    //    }
                    //    if (neighbours.ContainsKey(edge.to))
                    //    {
                    //        neighbours[edge.to].Add(edge.from);
                    //    }
                    //    else
                    //    {
                    //        List<int> l = new List<int>();
                    //        l.Add(edge.from);
                    //        neighbours.Add(edge.to, l);
                    //    }
                    var s = neighbours;
                }
                else
                {
                    var s = neighbours;
                    continue;
                    //if (!neighbours.ContainsKey(edge.from))
                    //{
                    //    List<int> l = new List<int>();
                    //    neighbours.Add(edge.from, l);
                    //}
                    //if (!neighbours.ContainsKey(edge.to))
                    //{
                    //    List<int> l = new List<int>();
                    //    neighbours.Add(edge.to, l);
                    //}

                }
            }
            
            foreach (var vertex in neighbours)
            {
                if (!reached.Contains(vertex.Key))
                {
                    HashSet<RGBPixel> h = new HashSet<RGBPixel>();
                    DFS(vertex.Key, ref reached, ref neighbours, ref h);
                    clusters.Add(h);
                }
            }

            return clusters;
        }

        private static void DFS(RGBPixel cur, ref HashSet<RGBPixel> visited, ref Dictionary<RGBPixel, List<RGBPixel>> neighbours, ref HashSet<RGBPixel> cluster)
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


    }
}

