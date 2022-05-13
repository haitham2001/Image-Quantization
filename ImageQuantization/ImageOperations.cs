using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Linq;
///Algorithms Project
///Intelligent Scissors
///

namespace ImageQuantization
{
    /// <summary>
    /// Holds the pixel color in 3 byte values: red, green and blue
    /// </summary>
    public struct RGBPixel
    {
        public byte red, green, blue;
    }
    public struct RGBPixelD
    {
        public double red, green, blue;
    }


    /// <summary>
    /// Library of static functions that deal with images
    /// </summary>
    public class ImageOperations
    {
        /// <summary>
        /// Open an image and load it into 2D array of colors (size: Height x Width)
        /// </summary>
        /// <param name="ImagePath">Image file path</param>
        /// <returns>2D array of colors</returns>
        public static RGBPixel[,] OpenImage(string ImagePath)
        {
            Bitmap original_bm = new Bitmap(ImagePath);
            int Height = original_bm.Height;
            int Width = original_bm.Width;

            RGBPixel[,] Buffer = new RGBPixel[Height, Width];

            unsafe
            {
                BitmapData bmd = original_bm.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, original_bm.PixelFormat);
                int x, y;
                int nWidth = 0;
                bool Format32 = false;
                bool Format24 = false;
                bool Format8 = false;

                if (original_bm.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    Format24 = true;
                    nWidth = Width * 3;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format32bppArgb || original_bm.PixelFormat == PixelFormat.Format32bppRgb || original_bm.PixelFormat == PixelFormat.Format32bppPArgb)
                {
                    Format32 = true;
                    nWidth = Width * 4;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    Format8 = true;
                    nWidth = Width;
                }
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (y = 0; y < Height; y++)
                {
                    for (x = 0; x < Width; x++)
                    {
                        if (Format8)
                        {
                            Buffer[y, x].red = Buffer[y, x].green = Buffer[y, x].blue = p[0];
                            p++;
                        }
                        else
                        {
                            Buffer[y, x].red = p[2];
                            Buffer[y, x].green = p[1];
                            Buffer[y, x].blue = p[0];
                            if (Format24) p += 3;
                            else if (Format32) p += 4;
                        }
                    }
                    p += nOffset;
                }
                original_bm.UnlockBits(bmd);
            }

            return Buffer;
        }

        /// <summary>
        /// Get the height of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Height</returns>
        public static int GetHeight(RGBPixel[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(0);
        }

        /// <summary>
        /// Get the width of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Width</returns>
        public static int GetWidth(RGBPixel[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(1);
        }



        /// <summary>
        /// Display the given image on the given PictureBox object
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <param name="PicBox">PictureBox object to display the image on it</param>
        public static void DisplayImage(RGBPixel[,] ImageMatrix, PictureBox PicBox)
        {
            // Create Image:
            //==============
            int Height = ImageMatrix.GetLength(0);
            int Width = ImageMatrix.GetLength(1);

            Bitmap ImageBMP = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);

            unsafe
            {
                BitmapData bmd = ImageBMP.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, ImageBMP.PixelFormat);
                int nWidth = 0;
                nWidth = Width * 3;
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        p[2] = ImageMatrix[i, j].red;
                        p[1] = ImageMatrix[i, j].green;
                        p[0] = ImageMatrix[i, j].blue;
                        p += 3;
                    }

                    p += nOffset;
                }
                ImageBMP.UnlockBits(bmd);
            }
            PicBox.Image = ImageBMP;
        }


        /// <summary>
        /// Apply Gaussian smoothing filter to enhance the edge detection 
        /// </summary>
        /// <param name="ImageMatrix">Colored image matrix</param>
        /// <param name="filterSize">Gaussian mask size</param>
        /// <param name="sigma">Gaussian sigma</param>
        /// <returns>smoothed color image</returns>
        public static RGBPixel[,] GaussianFilter1D(RGBPixel[,] ImageMatrix, int filterSize, double sigma)
        {
            int Height = GetHeight(ImageMatrix);
            int Width = GetWidth(ImageMatrix);

            RGBPixelD[,] VerFiltered = new RGBPixelD[Height, Width];
            RGBPixel[,] Filtered = new RGBPixel[Height, Width];


            // Create Filter in Spatial Domain:
            //=================================
            //make the filter ODD size
            if (filterSize % 2 == 0) filterSize++;

            double[] Filter = new double[filterSize];

            //Compute Filter in Spatial Domain :
            //==================================
            double Sum1 = 0;
            int HalfSize = filterSize / 2;
            for (int y = -HalfSize; y <= HalfSize; y++)
            {
                //Filter[y+HalfSize] = (1.0 / (Math.Sqrt(2 * 22.0/7.0) * Segma)) * Math.Exp(-(double)(y*y) / (double)(2 * Segma * Segma)) ;
                Filter[y + HalfSize] = Math.Exp(-(double)(y * y) / (double)(2 * sigma * sigma));
                Sum1 += Filter[y + HalfSize];
            }
            for (int y = -HalfSize; y <= HalfSize; y++)
            {
                Filter[y + HalfSize] /= Sum1;
            }

            //Filter Original Image Vertically:
            //=================================
            int ii, jj;
            RGBPixelD Sum;
            RGBPixel Item1;
            RGBPixelD Item2;

            for (int j = 0; j < Width; j++)
                for (int i = 0; i < Height; i++)
                {
                    Sum.red = 0;
                    Sum.green = 0;
                    Sum.blue = 0;
                    for (int y = -HalfSize; y <= HalfSize; y++)
                    {
                        ii = i + y;
                        if (ii >= 0 && ii < Height)
                        {
                            Item1 = ImageMatrix[ii, j];
                            Sum.red += Filter[y + HalfSize] * Item1.red;
                            Sum.green += Filter[y + HalfSize] * Item1.green;
                            Sum.blue += Filter[y + HalfSize] * Item1.blue;
                        }
                    }
                    VerFiltered[i, j] = Sum;
                }

            //Filter Resulting Image Horizontally:
            //===================================
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                {
                    Sum.red = 0;
                    Sum.green = 0;
                    Sum.blue = 0;
                    for (int x = -HalfSize; x <= HalfSize; x++)
                    {
                        jj = j + x;
                        if (jj >= 0 && jj < Width)
                        {
                            Item2 = VerFiltered[i, jj];
                            Sum.red += Filter[x + HalfSize] * Item2.red;
                            Sum.green += Filter[x + HalfSize] * Item2.green;
                            Sum.blue += Filter[x + HalfSize] * Item2.blue;
                        }
                    }
                    Filtered[i, j].red = (byte)Sum.red;
                    Filtered[i, j].green = (byte)Sum.green;
                    Filtered[i, j].blue = (byte)Sum.blue;
                }

            return Filtered;
        }

        /*
                public static double Distance_between_two_colors(RGBPixel x, RGBPixel y)
                {
                    return Math.Sqrt(Math.Pow(x.red - y.red, 2) + Math.Pow(x.blue - y.blue, 2) + Math.Pow(x.green - y.green, 2));
                }
        */
        
       public static HashSet<RGBPixel> list_color = new HashSet<RGBPixel>();
        public static HashSet<RGBPixel> DistinctColors(RGBPixel[,] ImageMatrix)
        {
            

            int width = GetWidth(ImageMatrix);
            int hight = GetHeight(ImageMatrix);
            for (int index_width = 0; index_width <width ; index_width++)
            {
                for (int index_height = 0; index_height < hight; index_height++)
                {
                    RGBPixel p = ImageMatrix[index_height, index_width];
                    //int r = ImageMatrix[ind]
                    list_color.Add(p);
                }
            }
            return list_color;
        }
        
        /*
        public static List<RGBPixelD> DistinctColors(RGBPixel[,] ImageMatrix)
        {

            bool[,,] array = new bool[256, 256, 256]; 
            List<RGBPixelD> list_color = new List<RGBPixelD>();
            RGBPixelD rGBPixelD;

            for (int index_height = 0; index_height < GetHeight(ImageMatrix); index_height++)
            {
                for (int index_width = 0; index_width < GetWidth(ImageMatrix); index_width++) 
                {
                    if (!array[ImageMatrix[index_height, index_width].red, ImageMatrix[index_height, index_width].green, ImageMatrix[index_height, index_width].blue])
                    {
                        array[ImageMatrix[index_height, index_width].red, ImageMatrix[index_height, index_width].green, ImageMatrix[index_height, index_width].blue] = true;
                        rGBPixelD.red = ImageMatrix[index_height, index_width].red; 
                        rGBPixelD.green = ImageMatrix[index_height, index_width].green; 
                        rGBPixelD.blue = ImageMatrix[index_height, index_width].blue;
                        list_color.Add(rGBPixelD);              
                    }
                }
            }
            return list_color;
        }
        */
        public static double MiniSpanTree()
        {
            List<RGBPixel> nodes = list_color.ToList();
            int[] array = new int[list_color.Count];
            double[] values = new double[list_color.Count];
            double totalminvalue = 0; //minimum total cost of the whole tree
            bool[] flag = new bool[list_color.Count];
            
            for(int i=0;i< list_color.Count; i++)
            {
                values[i] = int.MaxValue;
                flag[i] = false;
            }

            //first value(Node) is always included in minispantree
            //First value will be 0 to be picked as first vertex which is always root
            values[0] = 0;
            array[0] = 0;

            int index = 0;
            while(index< list_color.Count)
            {
                double minimum = int.MaxValue;
                int minimum_index = -1; //try to get minimum node from array of vertices

                for(int i=0;i< list_color.Count; i++)
                {
                    if(flag[i] == false && values[i] < minimum)
                    {
                        minimum = values[i];
                        minimum_index = i;
                    }
                }

                flag[minimum_index] = true;

                for(int node=0;node< list_color.Count; node++)
                {
                    double Red_Color = nodes[minimum_index].red - nodes[node].red;
                    double Green_Color = nodes[minimum_index].green - nodes[node].green;
                    double Blue_Color = nodes[minimum_index].blue - nodes[node].blue;

                    double distance =  Math.Sqrt(Red_Color*Red_Color+Green_Color*Green_Color+Blue_Color*Blue_Color);

                    if(distance>= 0 && flag[node] == false && distance <= values[node])
                    {
                        array[node] = minimum_index;
                        values[node] = distance;
                    }
                }

                index++;
            }

            for (int i = 0; i < list_color.Count; i++)
            {
                totalminvalue += values[i];
            }
            return totalminvalue;
        }

        public static RGBPixel[,] quantize_image(RGBPixel[,,] rGBPixels, RGBPixel[,] ImageMatrix)
        {
            RGBPixel rGBPixel;
            for (int index_height = 0; index_height < GetHeight(ImageMatrix); index_height++)
            {
                for (int index_width = 0; index_width < GetWidth(ImageMatrix); index_width++)
                {
                    rGBPixel.red = rGBPixels[ImageMatrix[index_height, index_width].red, ImageMatrix[index_height, index_width].green, ImageMatrix[index_height, index_width].blue].red;
                    rGBPixel.green = rGBPixels[ImageMatrix[index_height, index_width].red, ImageMatrix[index_height, index_width].green, ImageMatrix[index_height, index_width].blue].green;
                    rGBPixel.blue = rGBPixels[ImageMatrix[index_height, index_width].red, ImageMatrix[index_height, index_width].green, ImageMatrix[index_height, index_width].blue].blue;
                    ImageMatrix[index_height, index_width] = rGBPixel;
                }
            }
            return ImageMatrix;
        }
    }
}
