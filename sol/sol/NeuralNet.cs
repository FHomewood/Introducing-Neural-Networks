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
        public NeuralNet()
        {
            InitializeComponent();
            //define network set up
            reader = new IdxReader("C:\\Users\\Chikan\\Downloads\\MNIST\\t10k-images.idx3-ubyte");
            matrix = reader.ReadToEndAsMatrices<byte>();
        }

        private void Update(object sender, EventArgs e)
        {
            int matID = rnd.Next(0, 10000);
            string output = "";
            for (int i = 0; i < 28 * 28; i++)
            {
                output += i % 28 == 0 && i > 0? Environment.NewLine : "";
                output += " "+string.Format("{0:000}", matrix[matID][i / 28, i % 28]);
            }
            _rtbOutput.Text = output;
        }
    }
}
