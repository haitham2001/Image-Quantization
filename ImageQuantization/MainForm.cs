using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ImageQuantization
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        RGBPixel[,] ImageMatrix;

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
            }
            txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();
        }

        private void btnGaussSmooth_Click(object sender, EventArgs e)
        {
            double before = System.Environment.TickCount;
            double sigma = double.Parse(txtGaussSigma.Text);
            int maskSize = (int)nudMaskSize.Value ;


            //double distinctColor_B = System.Environment.TickCount;

            List<int> distinctColor = ImageOperations.DistinctColors(ImageMatrix);

            //double distinctColor_a = System.Environment.TickCount;
            //double distinctColor_r = distinctColor_a - distinctColor_B;
            //distinctColor_r /= 1000;

            var prim_algo = new prim_algo(distinctColor.Count);
            
            //double prim_algo_B = System.Environment.TickCount;

            List<Edge> edges = prim_algo.graphOfMST(distinctColor);
            double mst = prim_algo.min_sum_MST();

            //double prim_algo_a = System.Environment.TickCount;
            //double prim_algo_r = prim_algo_a - prim_algo_B;
            //prim_algo_r /= 1000;


            if (textBox1.Text == "")
            {
                MessageBox.Show("Please Enter number of K cluster");
            }
            else
            {
                int K = int.Parse(textBox1.Text);

                //double getCluster_B = System.Environment.TickCount;

                var cluster = Clustering.getClusters(edges, K, distinctColor);

                //double getCluster_a = System.Environment.TickCount;
                //double getCluster_r = getCluster_a - getCluster_B;
                //getCluster_r /= 1000;


                //double ExtractColor_B = System.Environment.TickCount;

                var color_extract = Clustering.ExtractColors(cluster);

                //double ExtractColor_a = System.Environment.TickCount;
                //double ExtractColor_r = ExtractColor_a - ExtractColor_B;
                //ExtractColor_r /= 1000;


                //double quantizeImage_B = System.Environment.TickCount;

                var new_image = Clustering.quantizeImage(ImageMatrix, color_extract);

                //double quantizeImage_a = System.Environment.TickCount;
                //double quantizeImage_r = quantizeImage_a - quantizeImage_B;
                //quantizeImage_r /= 1000;


                //MessageBox.Show("Time of Distinct Colors : " +distinctColor_r.ToString()+"   " +
                //    "Time of prim_algo : " + prim_algo_r.ToString() + "   " +
                //    "Time of getCluster : " + getCluster_r.ToString() + "   " +
                //    "Time of ExtractColor : " + ExtractColor_r.ToString() + "   " +
                //    "Time of quantizeImage : " + quantizeImage_r.ToString() + "   "
                //    );

                //=================================================================================
                textBox2.Text = distinctColor.Count.ToString();
                textBox3.Text = mst.ToString();

                ImageOperations.DistinctColours.Clear();
                Clustering.clusters.Clear();


                //======================================================================================
                ImageMatrix = ImageOperations.GaussianFilter1D(new_image, maskSize, sigma);
                // to check number of distinct colors
                ImageOperations.DisplayImage(new_image, pictureBox2);

                double after = System.Environment.TickCount;
                double result = after - before;
                result /= 1000;
                textBox4.Text = result.ToString() + " Sec";
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}