using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text;

namespace ImageQuantization
{
    class Clustering
    {
        public static List<List<RGBPixel>> rgb_clusters;

        public static List<HashSet<int>> getClusters(List<Edge> MST, int num_cluster, List<color> distinctColor) // O(K *d)
        {
            Dictionary<int, List<int>> neighbours = new Dictionary<int, List<int>>();
            List<HashSet<int>> clusters = new List<HashSet<int>>();
            HashSet<int> reached = new HashSet<int>();
            int index_of_max;
            double max;

            for (int i = 0; i < num_cluster - 1; i++)
            {
                index_of_max = 0;
                max = 0;

                for (int j = 0; j < MST.Count; j++)
                {
                    if (i == 0)
                    {
                        neighbours.Add(distinctColor[j].int_color, new List<int>());
                        if (j == MST.Count - 1)
                            neighbours.Add(distinctColor[MST.Count].int_color, new List<int>());
                    }
                    if (MST[j].weight > max)
                    {
                        max = MST[j].weight;
                        index_of_max = j;
                    }

                }
                MST[index_of_max].weight = -1;
            }

            foreach (var edge in MST)
            {
                if (edge.weight != -1)
                {
                    neighbours[edge.from.int_color].Add(edge.to.int_color);
                    neighbours[edge.to.int_color].Add(edge.from.int_color);
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

        public static List<RGBPixel> ExtractColors(List<HashSet<int>> clusters)
        {
            rgb_clusters = new List<List<RGBPixel>>();
            for(int i = 0;i<clusters.Count;i++)
            {
                List<RGBPixel> rgppixel_color = new List<RGBPixel>();
                RGBPixel color_in_bytes;
                foreach (var color in clusters[i])
                {
                    color_in_bytes.red = (byte)(color >> 16);
                    color_in_bytes.green = (byte)(color >> 8);
                    color_in_bytes.blue = (byte)(color);
                    rgppixel_color.Add(color_in_bytes);
                }
                rgb_clusters.Add(rgppixel_color);
            }

            List<RGBPixel> Palette = new List<RGBPixel>();
            RGBPixel avgColor;
            int redSum, greenSum, blueSum;

            for (int i = 0; i < clusters.Count; i++)
            {
                redSum = 0; greenSum = 0; blueSum = 0;

                foreach (var color in rgb_clusters[i])
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

        public static RGBPixel[,] quantizeImage(RGBPixel[,] imageMatrix, List<RGBPixel> palette)
        {
            int height = ImageOperations.GetHeight(imageMatrix);
            int width = ImageOperations.GetWidth(imageMatrix);
            RGBPixel[,] quantized_image = new RGBPixel[height, width];
            RGBPixel[,,] new_colors = new RGBPixel[256, 256, 256];

            for (int i = 0; i < rgb_clusters.Count; i++)
            {
                foreach (var cluster in rgb_clusters[i])
                {
                    new_colors[cluster.red, cluster.blue, cluster.green] = palette[i];
                }
            }

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    quantized_image[i, j] = new_colors[imageMatrix[i, j].red, imageMatrix[i, j].blue, imageMatrix[i, j].green];
                }
            }
            return quantized_image;
        }
    }
}


