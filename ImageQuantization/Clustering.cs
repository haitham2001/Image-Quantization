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
        public static List<RGBPixel> avgColor;
        
        public static List<HashSet<int>> getClusters(List<Edge> MST, int num_cluster, List<int> distinctColor) // O(K*D) ??
        {
            Dictionary<int, HashSet<int>> neighbours = new Dictionary<int, HashSet<int>>();     //Θ(1)
            HashSet<int> reached = new HashSet<int>();                                          //Θ(1)
            clusters = new List<HashSet<int>>();                                                //Θ(1)
            avgColor = new List<RGBPixel>();                                                     //Θ(1)
            int redSum, greenSum, blueSum;                                                      //Θ(1)

            for (int i = 0; i < num_cluster - 1; i++)                                           //Θ(K)
            {
                MST.RemoveAt(MST.Count - 1);                                                    //Θ(1)
            }

            for (int j = 0; j < distinctColor.Count; j++)                                       //Θ(D)
            {
                neighbours.Add(distinctColor[j], new HashSet<int>());                           //Θ(1)

            }

            for (int j = 0; j < MST.Count; j++)                                                 //Θ(E)
            {
                neighbours[MST[j].from].Add(MST[j].to);                                         //Θ(1)
                neighbours[MST[j].to].Add(MST[j].from);                                         //Θ(1)
            }

            //total = O(D)
            foreach (var neighbour in neighbours)                                                  //Θ(D)
            {
                if (!reached.Contains(neighbour.Key))                                              //Θ(1)
                {
                    RGBPixel avg_color;                                                            //Θ(1)
                    HashSet<int> cluster = new HashSet<int>();                                     //Θ(1)
                    redSum = 0; greenSum = 0; blueSum = 0;                                         //Θ(1)

                    DFS(neighbour.Key, ref reached, ref neighbours, ref cluster, ref redSum, ref greenSum, ref blueSum);            //Θ(D)
                    clusters.Add(cluster);                                                   //Θ(1)

                    avg_color.red = (byte)(redSum / cluster.Count);                          //Θ(1)
                    avg_color.green = (byte)(greenSum / cluster.Count);                      //Θ(1)
                    avg_color.blue = (byte)(blueSum / cluster.Count);                        //Θ(1)
                    avgColor.Add(avg_color);                                                 //Θ(1)

                }
            }
            return clusters;                                                                 //Θ(1)
        }

        private static void DFS(int cur, ref HashSet<int> visited, ref Dictionary<int, HashSet<int>> neighbours, ref HashSet<int> cluster
            ,ref int redSum, ref int greenSum, ref int blueSum)
        {
            cluster.Add(cur);                                                  //Θ(1)
            visited.Add(cur);                                                  //Θ(1)

            redSum += (byte)(cur >> 16);                                       //Θ(1)
            greenSum += (byte)(cur >> 8);                                      //Θ(1)
            blueSum += (byte)(cur);                                            //Θ(1)

            foreach (var neighbour in neighbours[cur])                         //Θ(D)
            {
                if (!visited.Contains(neighbour))                              //Θ(1)
                    DFS(neighbour, ref visited, ref neighbours, ref cluster,ref redSum,ref greenSum,ref blueSum); //Θ(1)
            }
        }

        public static List<RGBPixel> ExtractColors(List<HashSet<int>> clusters)  //Θ(K)
        {

            List<RGBPixel> Palette = new List<RGBPixel>();           //Θ(1)
            for (int i = 0; i < clusters.Count; i++)                 //Θ(K)
            {
                Palette.Add(avgColor[i]);                            //Θ(1)
            }

            return Palette; //Θ(1)
        }

        public static RGBPixel[,] quantizeImage(RGBPixel[,] imageMatrix, List<RGBPixel> palette)   //Θ(N*N)
        {
            int height = ImageOperations.GetHeight(imageMatrix);                  //Θ(1)
            int width = ImageOperations.GetWidth(imageMatrix);                    //Θ(1)
            RGBPixel[,] quantized_image = new RGBPixel[height, width];            //Θ(1)
            RGBPixel[,,] new_colors = new RGBPixel[256,256,256];                  //Θ(1)


            for (int i = 0; i < clusters.Count; i++)                              //Θ(K) --> Θ(K*D)
            {
                foreach (var cluster in clusters[i])                              //Θ(D)
                {
                    new_colors[(byte)(cluster >> 16), (byte)(cluster), (byte)(cluster >> 8)] = palette[i]; //Θ(1)
                }
            }

            for (int i = 0; i < height; i++)                 //Θ(N*N)                      //Θ(N)          N-->height
            {
                for (int j = 0; j < width; j++)                                    //Θ(N)          N-->width
                {
                    quantized_image[i, j] = new_colors[imageMatrix[i, j].red, imageMatrix[i, j].blue, imageMatrix[i, j].green]; //Θ(1)
                }
            }
            return quantized_image; //Θ(1)
        }
    }
}


