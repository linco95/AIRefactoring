using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NeuralNetwork;
using NeuralNetwork.Genetic;

public class xorcNet : MonoBehaviour {
    NeuralNet net;
    GeneticNeuralNet genNet;
    Patterns data = new Patterns();
    const int EPOKER = 256;
    List<KeyValuePair<List<double>, List<double>>> _results = new List<KeyValuePair<List<double>, List<double>>>();
    public List<KeyValuePair<List<double>, List<double>>> results { get { return _results; } private set { _results = value; } }


    // Use this for initialization
    void Start () {
        net = new NeuralNet(16, 6, 2);
        genNet = new GeneticNeuralNet(16, 4, 2, 100);
        List<DataSet> sets = data.getDataSet(false);
        //net.Train(data.getDataSet(true), EPOKER);
        genNet.Train(data.getDataSet(true), 10, 50);
        net = genNet;
        sets.ForEach(o => Compute(o.Values));
	}
	
    private void Compute(double[] values) {
        //int width = 4;
        //string outputstr = "";
        //for(int i = 0; i < values.Length; ++i) {
        //    if (i % width == 0) {
        //        outputstr += '\n';
        //    }
        //    outputstr += values[i] + " ";
        //}
        //outputstr += "\n[" + result[0] + ", " + result[1] + "]\n";
        //outputstr += (result[0] > 0.5f ? "It is a circle!!!" : "") + '\n';
        //outputstr += (result[1] > 0.5f ? "It is a cross!!!" : "") + '\n';
        //outputstr += (result[0] > result[1] ? "More circle than cross." : "More cross than circle") + '\n'; 
        //print(outputstr);
        var result = net.Compute(values);
        results.Add(new KeyValuePair<List<double>, List<double>>(new List<double>(values), new List<double>(result)));
    }

    // Update is called once per frame
    void Update () {
		
	}
}
