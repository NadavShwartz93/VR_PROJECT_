using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// This class was written by Nadav Shwarz and Gal Sherman.
/// This class set the next bubble location, based on the user's results.
/// </summary>
class BubblePosition
{
    private static BubblePosition instance = null;
    private double[] weights { get; set; }
    private bool firstPredicte { get; set; }
    private System.Random random { get; set; }


    private BubblePosition()
    {
        //Initialize class fields variables.
        instance = this;
        firstPredicte = true;
        random = new System.Random();
    }

    public static BubblePosition getInstance()
    {
        if (instance == null)
            instance = new BubblePosition();

        return instance;
    }

    public void calculateBubblePosition(int[] predictedClass)
    {
        int predClass = predictedClass[0];

        if (predClass == 0)
            weights = getPredictedArray(Globals.numOfAreas);
        else if (predClass == 1) { 
            weights = getPredictedArray(Globals.numOfAreas/2);
            double[] tempArr = new double[Globals.numOfAreas / 2];
            weights.Concat(tempArr);
        }
        else if (predClass == 2)
        {
            var tempWeights= getPredictedArray(Globals.numOfAreas / 2);
            double[] tempArr = new double[Globals.numOfAreas / 2];
            tempArr.Concat(tempWeights);
            weights = tempArr;
        }

        // Update the GameManager classNumber field about the selected class for the next bubble. 
        GameManager.instance.areaNumber = calcPosition();

        //Increment the array in the 0 place by one in order to find the probability (weights).
        Globals.numOfApperancce[GameManager.instance.areaNumber]++;
    }

    private int calcPosition()
    {
        //Section 1
        List<Items<int>> initial = itemsToList();

        //Section 2
        var converted = new List<Items<int>>(initial.Count);

        var sum = 0.0;

        for (int i = 1; i < initial.Count; i++)
        {
            sum += initial[i].Probability;
            converted.Add(new Items<int> { Probability = sum, Item = initial[i].Item });
        }

        //The first element in the list has the biggest Probability,
        //so this element got probability of 1.
        converted.Add(new Items<int> { Probability = 1.0, Item = initial.First().Item });


        //Section 3
        var probability = UnityEngine.Random.Range(0, (float)1.01);
        var selected = converted.FirstOrDefault(i => i.Probability >= probability);
        Debug.Log("Selected area = " + selected.Item);

        
        return selected.Item;
    }

    private List<Items<int>> itemsToList()
    {
        List<Items<int>> initial = new List<Items<int>>();
        for (int i = 0; i < Globals.numOfAreas; i++)
        {
            initial.Add(new Items<int>
            {
                Probability = weights[i],
                Item = i
            });
        }

        return initial;
    }

    /// <summary>
    /// Return probability between 0 to 1. Inside array.
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    private double[] getPredictedArray(int size)
    {
        double min = 0;
        double max = 1;
        double sum = 0;

        double[] arr = new double[size];
        for (int i = 0; i < size; i++)
        {
            if (i == size - 1)
                arr[i] = 1 - sum;
            else
            {
                arr[i] = GetRandomNumberInRange(min, max);
                max -= arr[i];
                sum += arr[i];
            }
        }

        // Sort array in ascending order.
        Array.Sort(arr);

        // reverse array
        Array.Reverse(arr);

        return arr;
    }


    /// <summary>
    /// This method multiply to given vectors.
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    private void multiplyVectors(double[] v1, double[] v2)
    {
        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] = v1[i] * v2[i];
        }
    }

    /// <summary>
    /// This method generate random number of type double from a given range. 
    /// </summary>
    /// <param name="minNumber"></param>
    /// <param name="maxNumber"></param>
    /// <returns></returns>
    private double GetRandomNumberInRange(double minNumber, double maxNumber)
    {
        return random.NextDouble() * (maxNumber - minNumber) + minNumber;
    }

    private class Items<T>
    {
        /// <summary>
        /// This class field represent the probability of the class.
        /// </summary>
        public double Probability { get; set; }

        /// <summary>
        /// This class field represent the class number.
        /// </summary>
        public T Item { get; set; }
    }
}