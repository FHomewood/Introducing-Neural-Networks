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
        
        Random rnd = new Random();
        int TrainedData = 0;
        int[] structure = new int[] { 28 * 28, 16, 16, 10 };
        List<Matrix<double>> Weight = new List<Matrix<double>>();
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
                Matrix<double> weightMatrix = Matrix.Build.Random(structure[i],structure[i-1]);
                Weight.Add(weightMatrix);
            }
        }
        private void UpdateNetwork(object sender, EventArgs e)
        {
            TrainedData++;
            int matID = rnd.Next(0, 10000);
            string output = "";
            ForwardNetwork(matID);
            float UnperturbedError = FindError(matID);
            //float newError = FindError(matID);

            //for (int x = 0; x < neuron.GetLength(0); x++)
            //    for (int y = 0; y < neuron.GetLength(1); y++)
            //        if (neuron[x, y] != null)
            //        {
            //            float perturbedError = FindError(matID);
            //            neuron[x, y].Bias += h;
            //            RunNetwork();
            //            newError = FindError(matID);
            //            neuron[x, y].Bias -= _learningRate * neuron[x, y].Bias*(perturbedError - newError) / h;

            //            for (int i = 0; i < neuron[x, y].weight.Length; i++)
            //            {
            //                perturbedError = FindError(matID);
            //                neuron[x, y].weight[i] += h;
            //                RunNetwork();
            //                newError = FindError(matID);
            //                neuron[x, y].weight[i] -= _learningRate * neuron[x, y].weight[i] * (perturbedError - newError) / h;
            //            }
            //}
            for (int i = 0; i < 10; i++)
            {
                lightINLabel[i].BackColor = labMatrix[matID] == i ? Color.Green : Color.Black;
                lightOUTLabel[i].Text = outneuron[i].ToString("p1");
                lightOUTLabel[i].BackColor = 255 * outneuron[i] > 0 ? Color.FromArgb(255, (int)((float)255 * outneuron[i]), 0, 0) : Color.FromArgb(255, 0, 0, (int)((float)255 * outneuron[i]));
            }
            for (int i = 0; i < 28*28; i++)
            {
                Image[i % 28, i / 28].BackColor = Color.FromArgb(255, imgMatrix[matID][i / 28, i % 28],
                                                                      imgMatrix[matID][i / 28, i % 28],
                                                                      imgMatrix[matID][i / 28, i % 28]);
            }
            for (int i = 0; i < 10; i++)
                output += "( " + (i).ToString() + " ):   " + outneuron[i].ToString() + Environment.NewLine;
            output += Environment.NewLine + Environment.NewLine +
                "Error: " + UnperturbedError.ToString() + Environment.NewLine +
                TrainedData.ToString();
            _lbout.Text = output;
        }
        private void ForwardNetwork(int matID)
        {
            Matrix<double> outputs = DenseMatrix.OfColumnVectors(ConvertToVector(imgMatrix[matID]));
            for (int i = 1; i < structure.Length; i++)
            {
                outputs = Sigmoid(Weight[i-1].Multiply(outputs));
            }
            for (int i = 0; i < 10; i++)
                outneuron[i] = outputs[i, 0];
        }
        private float FindError(int matID)
        {
            float error = 0;
            for (int i = 0; i < 10; i++)
            {
                error += i == labMatrix[matID] ?
                    (float)Math.Abs(outneuron[i] - 1) :
                    (float)Math.Abs(outneuron[i]);
            }
            return error;
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
