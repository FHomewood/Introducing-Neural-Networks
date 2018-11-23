using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Accord.IO;

namespace sol
{
    public partial class NeuralNet : Form
    {
        //custom variables
        int _panelsize = 16;
        float _learningRate = 0.05f;
        float h = 0.001f;

        Panel[,] Image = new Panel[28, 28];
        Label[] lightINLabel = new Label[10];
        Label[] lightOUTLabel = new Label[10];
        byte[][,] imgMatrix;
        byte[] labMatrix;
        
        Random rnd = new Random();
        Neuron[,] neuron = new Neuron[4,28*28];
        int TrainedData = 0;
        int[] structure = new int[] { 28 * 28, 16, 16, 10 };
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
            this.Size = new Size(20 + 29 * _panelsize, 50 + 29 * _panelsize + 29 * _panelsize / 10 + 29 * _panelsize / 10 / 2);
            CreateNetwork(structure);
        }
        public void CreateNetwork(int[] structure)
        {
            for(int l = 0; l < structure.Length;l++)
                for (int i = 0; i < structure[l]; i++)
                {
                    if (l == 0)
                        neuron[l,i] = (new Neuron(rnd, l, i, structure[l + 1], 0));
                    else if (l == structure.Length-1)
                        neuron[l,i] = (new Neuron(rnd, l, i, 0, structure[l - 1]));
                    else
                        neuron[l,i] = (new Neuron(rnd, l, i, structure[l + 1], structure[l - 1]));
                }
        }
        private void Update(object sender, EventArgs e)
        {
            TrainedData++;
            int matID = rnd.Next(0, 10000);
            string output = "";
            RunNetwork();
            float UnperturbedError = FindError(matID);
            float newError = FindError(matID);

            for (int x = 0; x < neuron.GetLength(0); x++)
                for (int y = 0; y < neuron.GetLength(1); y++)
                    if (neuron[x, y] != null)
                    {
                        float perturbedError = FindError(matID);
                        neuron[x, y].Bias += h;
                        RunNetwork();
                        newError = FindError(matID);
                        neuron[x, y].Bias -= _learningRate * neuron[x, y].Bias*(perturbedError - newError) / h;

                        for (int i = 0; i < neuron[x, y].weight.Length; i++)
                        {
                            perturbedError = FindError(matID);
                            neuron[x, y].weight[i] += h;
                            RunNetwork();
                            newError = FindError(matID);
                            neuron[x, y].weight[i] -= _learningRate * neuron[x, y].weight[i] * (perturbedError - newError) / h;
                        }
                    }
            for (int i = 0; i < 10; i++)
            {
                lightINLabel[i].BackColor = labMatrix[matID] == i? Color.Green: Color.Black;
                lightOUTLabel[i].Text = neuron[3, i].Value.ToString("p1");
                lightOUTLabel[i].BackColor = 255 * neuron[3, i].Value > 0 ? Color.FromArgb(255, (int)((float)255 * neuron[3, i].Value), 0, 0) : Color.FromArgb(255, 0, 0, (int)((float)255 * neuron[3, i].Value));
            }
            for (int i = 0; i < 28 * 28; i++)
            {
                Image[i % 28, i / 28].BackColor = Color.FromArgb(255- imgMatrix[matID][i / 28, i % 28], imgMatrix[matID][i / 28, i % 28], imgMatrix[matID][i / 28, i % 28], imgMatrix[matID][i / 28, i % 28]);
                neuron[0, i].Set(imgMatrix[matID][i / 28, i % 28]);
            }
            for (int i = 0; i < 10; i++)
                output += "( " + (i).ToString() + " ):   " + neuron[3, i].Value.ToString() + Environment.NewLine;
            output += Environment.NewLine + Environment.NewLine +
                "Error: " + UnperturbedError.ToString() + Environment.NewLine +
                TrainedData.ToString();
            _lbout.Text = output;
        }
        private void RunNetwork()
        {
            for (int i = 0; i < neuron.GetLength(0); i++)
                for (int j = 0; j < neuron.GetLength(1); j++)
                    if (neuron[i, j] != null)
                    {
                        neuron[i, j].Update();
                        neuron[i, j].Output(neuron);
                    }
        }
        private float FindError(int matID)
        {
            float error = 0;
            for (int i = 0; i < 10; i++)
            {
                error += i == labMatrix[matID] ?
                    (float)Math.Abs(neuron[3, i].Value - 1) :
                    (float)Math.Abs(neuron[3, i].Value);
            }
            return error;
        }

        private void NeuralNet_Load(object sender, EventArgs e)
        {

        }
    }
}
