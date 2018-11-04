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
        float bias;
        float[] weight;
        public Neuron(Random rnd, int layer, int id, int outputs)
        {
            bias = (float)rnd.NextDouble();
            this.layer = layer;
            this.id = id;
            weight = new float[outputs];
            for (int w = 0; w < weight.Length;w++) weight[w] = (float)rnd.NextDouble();
        }
    }
}
