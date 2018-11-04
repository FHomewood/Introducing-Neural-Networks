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
        IdxReader reader;
        byte[][,] matrix;
        Random rnd = new Random();
        Neuron[,] neuron = new Neuron[4,28*28];
        int[] structure = new int[] { 28 * 28, 16, 16, 10 };
        public NeuralNet()
        {
            InitializeComponent();
            //define network set up
            reader = new IdxReader("C:\\Users\\Chikan\\Downloads\\MNIST\\train-images.idx3-ubyte");
            matrix = reader.ReadToEndAsMatrices<byte>();
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
                    else neuron[l,i] = (new Neuron(rnd, l, i, structure[l + 1], structure[l - 1]));
                }
        }
        private void Update(object sender, EventArgs e)
        {
            int matID = rnd.Next(0, 10000);
            string output = "";
            for (int i = 0; i < 28 * 28; i++)
            {
                output += i % 28 == 0 && i > 0? Environment.NewLine : "";
                output += " "+string.Format("{0:000}", matrix[matID][i / 28, i % 28]);
                neuron[0, i].Set(matrix[matID][i / 28, i % 28]);
            }
            _rtbOutput.Text = output;
            for (int i = 0; i < neuron.GetLength(0); i++)
                for (int j = 0; j < neuron.GetLength(1); j++)
                    if (neuron[i,j] != null)
                    { neuron[i,j].Update(); neuron[i,j].Output(neuron); }
            output = "";
            for (int i = 0; i < 10; i++)
                output += "( " + (i).ToString() + " ):   " + neuron[3, i].val.ToString() + Environment.NewLine;
            _lbout.Text = output;
        }
    }
}
