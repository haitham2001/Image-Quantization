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

        public static double Distance_between_two_colors(RGBPixel x, RGBPixel y)//Θ(1)
        {
            //RGBPixel p;
            ///p.red = 
            return Math.Sqrt(Math.Pow(x.red - y.red, 2) + Math.Pow(x.blue - y.blue, 2) + Math.Pow(x.green - y.green, 2));//Θ(1)
        }
        
        public static HashSet<RGBPixel> list_color = new HashSet<RGBPixel>(); //Θ(1)
        public static List<RGBPixel> DistinctColors(RGBPixel[,] ImageMatrix)
        {
            int width = GetWidth(ImageMatrix); //Θ(1)
            int hight = GetHeight(ImageMatrix);//Θ(1)

            RGBPixel rGBPixelD;//Θ(1)
            for (int index_width = 0; index_width <width ; index_width++) // N*Inner=>Θ(N*N)
            {
                for (int index_height = 0; index_height < hight; index_height++) //N*Θ(1)
                {
                    rGBPixelD.red = ImageMatrix[index_height, index_width].red;//Θ(1)
                    rGBPixelD.green = ImageMatrix[index_height, index_width].green;//Θ(1)
                    rGBPixelD.blue = ImageMatrix[index_height, index_width].blue;//Θ(1)
                    list_color.Add(rGBPixelD);//Θ(1)
                }
            }
            
            return list_color.ToList();//Θ(1)
        }
        //public static Dictionary<KeyValuePair<int,int>,double>
        
        //public static double MiniSpanTree()
        //{
        //    List<RGBPixel> nodes = list_color.ToList();//Θ(1)
        //    int[] array = new int[list_color.Count];//Θ(1)
        //    double[] values = new double[list_color.Count];//Θ(1)
        //    double totalminvalue = 0;//Θ(1)            //minimum total cost of the whole tree
        //    bool[] flag = new bool[list_color.Count];//Θ(1)

        //    for (int i=0;i< list_color.Count; i++)//Θ(N)
        //    {
        //        values[i] = int.MaxValue;//Θ(1)
        //        flag[i] = false;//Θ(1)
        //    }

        //    //first value(Node) is always included in minispantree
        //    //First value will be 0 to be picked as first vertex which is always root
        //    values[0] = 0;//Θ(1)
        //    array[0] = 0;//Θ(1)

        //    int index = 0;//Θ(1)
        //    while (index< list_color.Count)//N*Inner
        //    {
        //        double minimum = int.MaxValue;//Θ(1)
        //        int minimum_index = -1;//Θ(1)  //try to get minimum node from array of vertices

        //        for (int i=0;i< list_color.Count; i++)//Θ(N)
        //        {
        //            if(flag[i] == false && values[i] < minimum)//Θ(1)
        //            {
        //                minimum = values[i];//Θ(1)
        //                minimum_index = i;//Θ(1)
        //            }
        //        }

        //        flag[minimum_index] = true;//Θ(1)

        //        for (int node=0;node< list_color.Count; node++)//N*Inner =>N*Θ(1) equal Θ(N)
        //        {
        //            double distance = Distance_between_two_colors(nodes[minimum_index], nodes[node]);//Θ(1)
        //            if (distance>= 0 && flag[node] == false && distance <= values[node])//Θ(1)
        //            {
        //                array[node] = minimum_index;//Θ(1)
        //                values[node] = distance;//Θ(1)
        //            }
        //        }

        //        index++;//Θ(1)
        //    }

        //    for (int i = 0; i < list_color.Count; i++)//N*Inner =>N*Θ(1) equal Θ(N)
        //    {
        //        totalminvalue += values[i];//Θ(1)
        //    }
        //    list_color.Clear();
        //    return totalminvalue;//Θ(1)
        //}

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




//publicstatic List<RGBPixel> DistinctColours;



//publicstatic int GetDistinctColors(RGBPixel[,] Buffer)

//{

//    bool[,,] Visited_Buffer = new bool[256, 256, 256];



//    DistinctColours = new List<RGBPixel>();



//    for (inti = 0; i < Buffer.GetLength(0); i++)

//    {

//        for (intj = 0; j < Buffer.GetLength(1); j++)

//        {



//            if (Visited_Buffer[Buffer[i, j].red, Buffer[i, j].green, Buffer[i, j].blue] == false)

//            {

//                Visited_Buffer[Buffer[i, j].red, Buffer[i, j].green, Buffer[i, j].blue] = true;

//                DistinctColours.Add(Buffer[i, j]);

//            }

//        }

//    }

//    returnDistinctColours.Count;

//}
