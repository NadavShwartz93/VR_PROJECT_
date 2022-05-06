using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This class was written by Nadav Shwarz and Gal Sherman.
/// This class set the next bubble location, based on the user's results.
/// In edition, this class calculate the weight of the Areas.
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

    public static BubblePosition GetInstance()
    {
        if (instance == null)
            instance = new BubblePosition();

        return instance;
    }

    public void CalculateBubblePosition(int[] predictedClass = null)
    {
        int predClass = -1;
        if (Globals.isPredicted == false)
        {
            predClass = predictedClass[0];
            if (predClass == 0)
                weights = GetPredictedArray(Globals.numOfAreas);
            else if (predClass == 1 || predClass == 2)
            {
                weights = GetPredictedArray(Globals.numOfAreas / 2);
            }
        }
        else 
        {
            var floatArr = Globals.GetRow(Globals.matrixOfRecommendation, 0);
            weights = Globals.floatToDouble(floatArr);
        }

        // Update the GameManager classNumber field about the selected class for the next bubble. 
        if(predictedClass != null)
            GameManager.instance.areaNumber = CalcPosition(predClass);
        else
            GameManager.instance.areaNumber = CalcPosition();

        //Increment the array in the 0 place by one in order to find the probability (weights).
        Globals.numOfApperance[GameManager.instance.areaNumber]++;
    }

    private int CalcPosition(int predClass = -1)
    {
        //Section 1
        List<Items<int>> initial = ItemsToList(predClass);

        //Section 2
        var converted = new List<Items<int>>(initial.Count);

        var sum = 0.0;
        double maxProbability = getMaxProbability(initial);
        int maxItem = 0;

        for (int i = 0; i < initial.Count; i++)
        {
            if (initial[i].Probability == maxProbability)
            {
                maxItem = i;
            }
            else
            {
                sum += initial[i].Probability;
                converted.Add(new Items<int> { Probability = sum, 
                    Item = initial[i].Item });
            }
        }

        //The first element in the list has the biggest Probability,
        //so this element got probability of 1.
        converted.Add(new Items<int> { Probability = 1.0, Item = maxItem });


        //Section 3
        var probability = UnityEngine.Random.Range(0, (float)1.01);
        var selected = converted.FirstOrDefault(i => i.Probability >= probability);
        Debug.Log("Selected area = " + selected.Item);


        return selected.Item;
    }

    private double getMaxProbability(List<Items<int>> initial)
    {
        double max = initial.First().Probability;

        foreach (var item in initial)
        {
            if (item.Probability > max)
                max = item.Probability;
        }

        return max;
    }

    private List<Items<int>> ItemsToList(int predClass)
    {
        List<Items<int>> initial = new List<Items<int>>();

        int index = 0;
        switch (predClass)
        {
            case 1:
                for (int i = 0; i < Globals.numOfAreas; i++)
                {
                    if(i > 3 && i < Globals.numOfAreas)
                        initial.Add(new Items<int>
                        {
                            Probability = 0,
                            Item = i
                        });
                    else
                    {
                        initial.Add(new Items<int>
                        {
                            Probability = weights[index++],
                            Item = i
                        });
                    }
                }
                break;
            case 2:
                for (int i = 0; i < Globals.numOfAreas; i++)
                {
                    if (i >= 0 && i < Globals.numOfAreas/2)
                        initial.Add(new Items<int>
                        {
                            Probability = 0,
                            Item = i
                        });
                    else
                    {
                        initial.Add(new Items<int>
                        {
                            Probability = weights[index++],
                            Item = i
                        });
                    }
                }
                break;
            default:
                for (int i = 0; i < Globals.numOfAreas; i++)
                {
                    initial.Add(new Items<int>
                    {
                        Probability = weights[i],
                        Item = i
                    });
                }
                break;
        }
        

        return initial;
    }

    /// <summary>
    /// Return probability between 0 to 1. 
    /// Inside array.
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    private double[] GetPredictedArray(int size)
    {
        double min = 0;
        double max = 1;
        double sum = 0;

        double[] arr = new double[size];

        //Get the first place randomly.

        //var randPlace = UnityEngine.Random.Range(0, size);
        var randPlace = random.Next(0, size);
        arr[randPlace] = UnityEngine.Random.Range((float)min, (float)max);
        max -= arr[randPlace];
        sum += arr[randPlace];

        for (int i = 0; i < size; i++)
        {
            if (i == randPlace)
                continue;

            if (i == size - 1)
                arr[i] = 1 - sum;
            else
            {
                //arr[i] = UnityEngine.Random.Range((float)min, (float)max);
                arr[i] = UnityEngine.Random.Range((float)0, (float)1);
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
    private void MultiplyVectors(double[] v1, double[] v2)
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