using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using MathNet.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Accord.IO;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace sol
{
    public partial class NeuralNet : Form
    {
        //custom variables
        int _panelsize = 16;
        float _learningRate = 0.02f;
        float h = 0.001f;

        Panel[,] Image = new Panel[28, 28];
        Label[] lightINLabel = new Label[10];
        Label[] lightOUTLabel = new Label[10];
        byte[][,] imgMatrix;
        byte[] labMatrix;
        int matID = 0;
        
        Random rnd = new Random();
        int TrainedData = 0;
        int[] structure = new int[] { 28 * 28, 16, 16, 10 };
        List<Matrix<double>> Weight = new List<Matrix<double>>();
        List<Matrix<double>> Bias = new List<Matrix<double>>();
        double[] outneuron = new double[10];

        public NeuralNet()
        {
            InitializeComponent();
            //define network set up
            imgMatrix = new IdxReader("C:\\Users\\Chikan\\Downloads\\MNIST\\train-images.idx3-ubyte").ReadToEndAsMatrices<byte>();
            labMatrix = new IdxReader("C:\\Users\\Chikan\\Downloads\\MNIST\\train-labels.idx1-ubyte").ReadToEndAsValues  <byte>();
            for (int i = 0; i < 10; i++)
            {
                lightINLabel[i] = new Label();
                this.Controls.Add(lightINLabel[i]);
                lightINLabel[i].Location = new Point(10 + 28 *_panelsize * i/10, 30 + 28*_panelsize);
                lightINLabel[i].TextAlign = ContentAlignment.MiddleCenter;
                lightINLabel[i].Size = new Size(28 * _panelsize / 10, 28 * _panelsize / 10);
                lightINLabel[i].Font = new Font("serifsans", 10f, FontStyle.Bold);
                lightINLabel[i].ForeColor = Color.White;
                lightINLabel[i].BackColor = Color.Black;
                lightINLabel[i].Text = i.ToString();


                lightOUTLabel[i] = new Label();
                this.Controls.Add(lightOUTLabel[i]);
                lightOUTLabel[i].Location = new Point(10 + 28 *_panelsize * i/10, 30 + 28*_panelsize + 28 * _panelsize / 10);
                lightOUTLabel[i].TextAlign = ContentAlignment.MiddleCenter;
                lightOUTLabel[i].Size = new Size(28 * _panelsize / 10, 28 * _panelsize / 10 / 2);
                lightOUTLabel[i].Font = new Font("serifsans", 6f, FontStyle.Regular);
                lightOUTLabel[i].ForeColor = Color.White;
                lightOUTLabel[i].BackColor = Color.Black;
                lightOUTLabel[i].Text = "";
            }
            for (int i = 0; i < 28 * 28; i++)
            {
                Image[i / 28, i % 28] = new Panel();
                Image[i / 28, i % 28].Location = new Point(10 + _panelsize * (i / 28), 10 + _panelsize * (i % 28));
                Image[i / 28, i % 28].Size = new Size(_panelsize, _panelsize);
                Image[i / 28, i % 28].BackColor = Color.White;
                this.Controls.Add( Image[i / 28, i % 28] );
            }
            this.Size = new Size(20 + 29 * _panelsize, 65 + 29 * _panelsize + 29 * _panelsize / 10 + 29 * _panelsize / 10 / 2);
            CreateNetwork(structure);
        }
        public void CreateNetwork(int[] structure)
        {
            for (int i = 1; i < structure.Length; i++)
            {
                Matrix<double> weightMatrix = Matrix.Build.Random(structure[i], structure[i - 1]);
                Matrix<double> biasMatrix = Matrix.Build.Random(structure[i], 1);
                Weight.Add(weightMatrix);
                Bias.Add(biasMatrix);
            }
        }
        private void UpdateNetwork(object sender, EventArgs e)
        {
            TrainedData++;
            matID = rnd.Next(0, 10000);
            ForwardNetwork();
            double UnperturbedError = FindError();
            UpdateGUI(UnperturbedError.ToString());
            BackPropagate();
        }
        private void UpdateGUI(string error)
        {
            string output = "";
            for (int i = 0; i < 10; i++)
            {
                lightINLabel[i].BackColor = labMatrix[matID] == i ? Color.Green : Color.Black;
                lightOUTLabel[i].Text = outneuron[i].ToString("p1");
                lightOUTLabel[i].BackColor = 255 * outneuron[i] > 0 ? Color.FromArgb(255, (int)((float)255 * outneuron[i]), 0, 0) : Color.FromArgb(255, 0, 0, (int)((float)255 * outneuron[i]));
            }
            for (int i = 0; i < 28 * 28; i++)
            {
                Image[i % 28, i / 28].BackColor = Color.FromArgb(255, imgMatrix[matID][i / 28, i % 28],
                                                                      imgMatrix[matID][i / 28, i % 28],
                                                                      imgMatrix[matID][i / 28, i % 28]);
            }
            output += "Error: " + error + Environment.NewLine +
                      "Data Trained: " + TrainedData.ToString();
            _lbout.Text = output;
        }
        private void ForwardNetwork()
        {
            Matrix<double> outputs = DenseMatrix.OfColumnVectors(ConvertToVector(imgMatrix[matID]));
            for (int i = 1; i < structure.Length; i++)
            {
                outputs = Sigmoid(Weight[i-1].Multiply(outputs)+Bias[i-1]);
            }
            for (int i = 0; i < 10; i++)
                outneuron[i] = outputs[i, 0];
        }
        private double FindError()
        {
            double error = 0;
            for (int i = 0; i < 10; i++)
            {
                error += i == labMatrix[matID] ?
                    (outneuron[i] - 1) * (outneuron[i] - 1) :
                    (outneuron[i]) * (outneuron[i]);
            }
            return error;
        }
        private void BackPropagate()
        {
            for (int i = 0; i < Weight.Count(); i++)
                for (int j = 0; j < Weight[i].RowCount; j++)
                    for (int k = 0; k < Weight[i].ColumnCount; k++)
                    {
                        double error = FindError();
                        Weight[i][j, k] += h;
                        ForwardNetwork();
                        double newerror = FindError();
                        double gradient = (error - newerror) / h;
                        Weight[i][j, k] -= h + _learningRate * gradient;
                    }
            for (int i = 0; i < Bias.Count(); i++)
                for (int j = 0; j < Weight[i].RowCount; j++)
                    {
                        double error = FindError();
                        Bias[i][j, 0] += h;
                        ForwardNetwork();
                        double newerror = FindError();
                        double gradient = (error - newerror) / h;
                        Bias[i][j, 0] -= h - _learningRate * gradient;
                    }
        }
        private void NeuralNet_Load(object sender, EventArgs e)
        {

        }
        public static Vector<double>[] ConvertToVector(byte[,] list)
        {
            double[] output = new double[list.GetLength(0) * list.GetLength(1)];
            for (int i = 0; i < list.GetLength(0); i++)
                for (int j = 0; j < list.GetLength(1); j++)
                    output[i * list.GetLength(1) + j] = (double)list[i, j];
            return new Vector<double>[] { DenseVector.OfArray(output) };
        }
        public static Matrix<double> Sigmoid(Matrix<double> matrix)
        {
            for (int i = 0; i < matrix.RowCount; i++)
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    matrix[i, j] = 1 / (1 + Math.Exp(matrix[i, j]));
                }
            return matrix;
        }
    }
}
