using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sol
{
    class Neuron
    {
        int layer, id;
        float val, bias;
        public float[] weight;
        float[] input;
        public Neuron(Random rnd, int layer, int id, int outNo, int inNo)
        {
            bias = (float)rnd.NextDouble();
            this.layer = layer;
            this.id = id;
            weight = new float[outNo];
            input = new float[inNo];
            for (int w = 0; w < weight.Length;w++) weight[w] = (float)rnd.NextDouble();
        }
        public void Update()
        {
            val = Eval();
        }
        public void Set(byte val)
        {
            this.val = sig((float)val);
        }
        private float Eval()
        {
            float _output = 0;
            for (int i = 0; i < input.Length; i++) _output += input[i];
            return sig(_output + bias);
        }
        public void Output(Neuron[,] neuron)
        {
            for (int i = 0; i < weight.Length; i++)
                neuron[layer + 1, i].input[id] = val * weight[i];
        }
        private float sig(float x)
        {
            return (float)(1 / (1 + Math.Exp(-x)));
        }
        public float Value
        {
            get { return val; }
            set { val = value; }
        }
        public float Bias
        {
            get { return bias; }
            set { bias = value; }
        }
    }
}
