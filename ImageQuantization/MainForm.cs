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
            List<color> distinctColor = ImageOperations.DistinctColors(ImageMatrix);
            var prim_algo = new prim_algo(distinctColor.Count);
            List<Edge> edges = prim_algo.graphOfMST(distinctColor);
            double mst = prim_algo.minimumSumOfMST();
            int K = int.Parse(textBox1.Text);
            var cluster = Clustering.getClusters(edges, K, distinctColor);
            var f = Clustering.ExtractColors(cluster);
            var v = Clustering.QuantizedImage(ImageMatrix,f);

            //=================================================================================
            textBox2.Text = distinctColor.Count.ToString();
            textBox3.Text = mst.ToString();

            ImageOperations.DistinctColours.Clear();

            //======================================================================================
            ImageMatrix = ImageOperations.GaussianFilter1D(v, maskSize, sigma);
            // to check number of distinct colors
            ImageOperations.DisplayImage(v, pictureBox2);

            double after = System.Environment.TickCount;
            double result = after - before;
            result /= 1000;
            textBox4.Text = result.ToString() + " Sec";
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}