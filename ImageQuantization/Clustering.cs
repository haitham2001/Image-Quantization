using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text;

namespace ImageQuantization
{
    class Clustering
    {    
        public static List<HashSet<int>> getClusters(List<Edge> MST, int num_cluster, List<int> distinctColor) // O(D)
        {
            Dictionary<int, HashSet<int>> neighbours = new Dictionary<int, HashSet<int>>();     //Θ(1)
            HashSet<int> reached = new HashSet<int>();                                          //Θ(1)
            List<HashSet<int>>clusters = new List<HashSet<int>>();                              //Θ(1)

            for (int i = 0; i < num_cluster - 1; i++)                                           //Θ(K)
            {
                MST.RemoveAt(MST.Count - 1);                                                    //O(1)
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

            foreach (var var in neighbours)                                                     //Θ(D)
            {
                if (!reached.Contains(var.Key))                                                 //Θ(1)
                {
                    HashSet<int> cluster = new HashSet<int>();                                  //Θ(1)
                    dfs(var.Key, ref reached, ref neighbours, ref cluster);                     
                    clusters.Add(cluster);                                                      //Θ(1)
                }
            }
            return clusters;                                                                    //Θ(1)
        }

        private static void dfs(int cur, ref HashSet<int> reached, ref Dictionary<int, HashSet<int>> neighbours, ref HashSet<int> cluster)
        {
            cluster.Add(cur);                                                  //Θ(1)
            reached.Add(cur);                                                  //Θ(1)

            foreach (var neighbour in neighbours[cur])                         //Θ(D)
            {
                if (!reached.Contains(neighbour))                              //Θ(1)
                    dfs(neighbour, ref reached, ref neighbours, ref cluster);  //Θ(1)
            }
        }

        public static RGBPixel[,,] Palette(List<HashSet<int>> clusters)          //Θ(D)
        {
            int sum_red, sum_green, sum_blue;                                    //Θ(1)
            RGBPixel[,,] palette = new RGBPixel[256, 256, 256];                  //Θ(1)
            List < RGBPixel> color_of_cluster = new List<RGBPixel>();            //Θ(1)

            for (int x = 0; x < clusters.Count; x++)                             //Θ(D)
            {
                RGBPixel average_color;
                sum_red = 0; 
                sum_green = 0; 
                sum_blue = 0;

                foreach (var cur in clusters[x])
                {
                    sum_red += (byte)(cur >> 16);                                      //Θ(1)
                    sum_green += (byte)(cur >> 8);                                     //Θ(1)
                    sum_blue += (byte)(cur);                                           //Θ(1)
                }
                average_color.red = (byte)(sum_red / clusters[x].Count);               //Θ(1)
                average_color.green = (byte)(sum_green / clusters[x].Count);           //Θ(1)
                average_color.blue = (byte)(sum_blue / clusters[x].Count);             //Θ(1)

                color_of_cluster.Add(average_color);
            }

            for (int i = 0; i < clusters.Count; i++)                                    //Θ(D)
            {                        
                foreach (var cluster in clusters[i])
                {
                    palette[(byte)(cluster >> 16), (byte)(cluster), (byte)(cluster >> 8)] = color_of_cluster[i]; //Θ(1)
                }
            }

            return palette; //Θ(1)
        }

        public static RGBPixel[,] quantizeImage(RGBPixel[,] imageMatrix,RGBPixel[,,] new_colors)   //Θ(N*N)
        {
            int height = ImageOperations.GetHeight(imageMatrix);                  //Θ(1)
            int width = ImageOperations.GetWidth(imageMatrix);                    //Θ(1)
            RGBPixel[,] quantized_image = new RGBPixel[height, width];            //Θ(1)

            for (int i = 0; i < height; i++)         //Θ(N*N)                     //Θ(N)          N-->height
            {
                for (int j = 0; j < width; j++)                                   //Θ(N)          N-->width
                {
                    quantized_image[i, j] = new_colors[imageMatrix[i, j].red, imageMatrix[i, j].blue, imageMatrix[i, j].green]; //Θ(1)
                }
            }
            return quantized_image; //Θ(1)
        }
    }
}


