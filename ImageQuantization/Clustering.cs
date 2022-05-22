using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text;

namespace ImageQuantization
{
    class Clustering
    {
        public static List<HashSet<int>> clusters;

        public static List<HashSet<int>> getClusters(List<Edge> MST, int num_cluster, List<int> distinctColor) // O(K*D)
        {
            Dictionary<int, HashSet<int>> neighbours = new Dictionary<int, HashSet<int>>();     //O(1)
            clusters = new List<HashSet<int>>();                                                //O(1)
            HashSet<int> reached = new HashSet<int>();                                          //O(1)

            for (int i = 0; i < num_cluster - 1; i++)                                           //O(K)
            {
                MST.RemoveAt(MST.Count - 1);                                              //O(1)
            }

            for (int j = 0; j < distinctColor.Count; j++)                                       //O(D)
            {
                neighbours.Add(distinctColor[j], new HashSet<int>());                           //O(1)

            }

            for (int j = 0; j < MST.Count; j++)                                              //O(E)
            {
                neighbours[MST[j].from].Add(MST[j].to);                                   //O(1)
                neighbours[MST[j].to].Add(MST[j].from);                                   //O(1)
            }

            //total = O(D+E)
            foreach (var vertex in neighbours)                                                  //O(D)
            {
                if (!reached.Contains(vertex.Key))                                              //O(1)
                {
                    HashSet<int> h = new HashSet<int>();                                        //O(1)
                    DFS(vertex.Key, ref reached, ref neighbours, ref h);                        //O(E)
                    clusters.Add(h);                                                            //O(1)
                }
            }
            return clusters;                                                                    //O(1)
        }

        private static void DFS(int cur, ref HashSet<int> visited, ref Dictionary<int, HashSet<int>> neighbours, ref HashSet<int> cluster)
        {
            visited.Add(cur);                                                  //O(1)
            cluster.Add(cur);                                                  //O(1)
            foreach (var neighbour in neighbours[cur])                         //O(E)
            {
                if (!visited.Contains(neighbour))                             //O(1)
                    DFS(neighbour, ref visited, ref neighbours, ref cluster);
            }
        }

        public static List<RGBPixel> ExtractColors(List<HashSet<int>> clusters)  //O(D)
        {

            List<RGBPixel> Palette = new List<RGBPixel>();           //O(1)
            RGBPixel avgColor;                                       //O(1)
            int redSum, greenSum, blueSum;                           //O(1)

            for (int i = 0; i < clusters.Count; i++)                 //O(1)
            {
                redSum = 0; greenSum = 0; blueSum = 0;               //O(1)

                foreach (var color in clusters[i])                   //O(D)
                {
                    redSum += (byte)(color >> 16);                         //O(1)
                    greenSum += (byte)(color >> 8);                        //O(1)
                    blueSum += (byte)(color);                              //O(1)
                }
                avgColor.red = (byte)(redSum / clusters[i].Count);         //O(1)
                avgColor.green = (byte)(greenSum / clusters[i].Count);     //O(1)
                avgColor.blue = (byte)(blueSum / clusters[i].Count);       //O(1)

                Palette.Add(avgColor);                                     //O(1)
            }

            return Palette;
        }

        public static RGBPixel[,] quantizeImage(RGBPixel[,] imageMatrix, List<RGBPixel> palette)   //O(N*N)
        {
            int height = ImageOperations.GetHeight(imageMatrix);                  //O(1)
            int width = ImageOperations.GetWidth(imageMatrix);                    //O(1)
            RGBPixel[,] quantized_image = new RGBPixel[height, width];            //O(1)
            RGBPixel[,,] new_colors = new RGBPixel[256, 256, 256];                //O(1)

            for (int i = 0; i < clusters.Count; i++)                              //O(K)
            {
                foreach (var cluster in clusters[i])                              //O(D)
                {
                    new_colors[(byte)(cluster >> 16), (byte)(cluster), (byte)(cluster >> 8)] = palette[i];
                }
            }

            for (int i = 0; i < height; i++)                                       //O(N)          N-->height
            {
                for (int j = 0; j < width; j++)                                    //O(N)          N-->width
                {
                    quantized_image[i, j] = new_colors[imageMatrix[i, j].red, imageMatrix[i, j].blue, imageMatrix[i, j].green];
                }
            }
            return quantized_image;
        }
    }
}


