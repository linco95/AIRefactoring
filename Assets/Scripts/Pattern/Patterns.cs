using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NeuralNetwork;

public class Patterns {
    List<Pattern> trainingSet = new List<Pattern>();
    List<Pattern> testSet = new List<Pattern>();

    public Patterns() {
        initiateTestSet();
        initiateTrainingSet();
    }

    private void initiateTrainingSet() {
        trainingSet.Add(new Pattern(
           new List<double>(){
            1, 0, 0, 1,
            0, 1, 1, 0,
            0, 1, 1, 0,
            1, 0, 0, 1
       },
       new Vector2(0.0f, 1.0f)));
        trainingSet.Add(new Pattern(
new List<double>(){
            1, 1, 0, 1,
            0, 1, 1, 1,
            1, 1, 1, 0,
            1, 0, 1, 1
},
new Vector2(0.0f, 1.0f)));
        trainingSet.Add(new Pattern(
     new List<double>(){
            0, 1, 1, 0,
            1, 0, 0, 1,
            1, 0, 0, 1,
            0, 1, 1, 0
 },
 new Vector2(1.0f, 0.0f)));
 //       trainingSet.Add(new Pattern(
 //    new List<double>(){
 //           0, 1, 1, 0,
 //           0, 1, 0, 1,
 //           1, 0, 0, 1,
 //           0, 1, 1, 0
 //},
 //new Vector2(0.75f, 0.0f)));
        trainingSet.Add(new Pattern(
     new List<double>(){
            0, 1, 1, 1,
            1, 0, 0, 1,
            1, 0, 0, 1,
            0, 1, 1, 0
 },
 new Vector2(0.875f   , 0)));
        trainingSet.Add(new Pattern(
    new List<double>(){
            1, 0, 0, 1,
            0, 0, 1, 0,
            0, 1, 1, 0,
            1, 0, 0, 1
},
new Vector2(0, 0.875f)));
        trainingSet.Add(new Pattern(
    new List<double>(){
            1, 0, 0, 1,
            0, 1, 1, 0,
            0, 1, 0, 1,
            1, 0, 0, 1
},
new Vector2(0.0f, 0.75f)));
        trainingSet.Add(new Pattern(
new List<double>(new double[16]),
new Vector2(0.0f, 0.0f)));
    }


    private void initiateTestSet() {
        System.Random rand = NeuralNet.Random;
        var l = new List<double>();
        for (int i = 0; i < 16; i++) {
            l.Add(rand.Next(0, 2));
        }
        testSet.Add(new Pattern(
l,
new Vector2(0.375f, 0)));   
        testSet.Add(new Pattern(
           new List<double>(){
            1, 0, 0, 0,
            0, 1, 1, 0,
            0, 0, 1, 0,
            1, 0, 0, 1
       },
       new Vector2(0, 0.75f)));
        testSet.Add(new Pattern(
     new List<double>(){
            0, 1, 1, 1,
            1, 0, 0, 0,
            1, 0, 0, 1,
            1, 1, 1, 0
 },
 new Vector2(0.75f, 0)));
        testSet.Add(new Pattern(
     new List<double>(){
            1, 1, 1, 0,
            1, 0, 0, 1,
            1, 0, 0, 1,
            0, 1, 0, 0
 },
 new Vector2(0.75f, 0)));
        testSet.Add(new Pattern(
     new List<double>(){
            1, 1, 1, 1,
            1, 0, 0, 1,
            1, 0, 0, 1,
            1, 1, 1, 1
 },
 new Vector2(1.0f, 0)));
        testSet.Add(new Pattern(
    new List<double>(){
            1, 0, 1, 1,
            1, 0, 1, 0,
            0, 1, 1, 0,
            1, 0, 0, 1
},
new Vector2(0, 0.625f)));
        testSet.Add(new Pattern(
    new List<double>(){
            1, 0, 0, 1,
            1, 1, 0, 1,
            0, 0, 1, 0,
            1, 1, 0, 1
},
new Vector2(0.375f, 0)));

    }


    public List<DataSet> getDataSet(bool training) {
        // put each lists lists in array and put answers as array instead
        //return new DataSet(trainingSet.SelectMany(o => o.pixels.ToArray()).ToArray(), trainingSet.SelectMany(o => new double[] { o.answer.x, o.answer.y }).ToArray());
        List<DataSet> retVal = new List<DataSet>();
        // TODO: Linq halp meh?
        (training ? trainingSet : testSet).ForEach(l => retVal.Add(l.getDataSet()));
        return retVal;
    }


    private struct Pattern {
        public List<double> pixels;
        public Vector2 answer;
        public Pattern(List<double> pixels, Vector2 answer) {
            this.pixels = pixels;
            this.answer = answer;
            answer = new Vector2((float)System.Math.Round(answer.x), (float)System.Math.Round(answer.y));
        }

        public DataSet getDataSet() {
            return new DataSet(pixels.ToArray(), new double[2] { answer.x, answer.y });
        }

        public override bool Equals(object obj) {
            if (!(obj is Pattern))
                return false;
            Pattern other = (Pattern)obj;
            return pixels.SequenceEqual(other.pixels) &&  answer == other.answer;
        }
        public override int GetHashCode() {
            return answer.GetHashCode() * 13 + pixels.GetHashCode() * 27;
        }
    }
}



