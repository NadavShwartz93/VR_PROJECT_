using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class BubblePosition
{
    private static BubblePosition instance = null;
    private double[] weights { get; set; }
    private bool firstPredicte { get; set; }
    private Random random { get; set; }

    private BubblePosition()
    {
        //Initialize class fields variables.
        instance = this;
        firstPredicte = true;
        random = new Random();
    }

    public static BubblePosition getInstance()
    {
        if (instance == null)
            instance = new BubblePosition();

        return instance;
    }

    public void calculateBubblePosition(int[] predictedClass)
    {
        if (firstPredicte)
            weights = getPredictedArray(predictedClass.Length);
        else
        {
            var v = getPredictedArray(predictedClass.Length);

            //Insert the result into weights array.
            multiplyVectors(v, weights);
        }

        calcPosition(predictedClass);

    }

    private void calcPosition(int[] predictedClass)
    {
        //Section 1
        List<Items<string>> initial = itemsToList(predictedClass);

        //Section 2
        var converted = new List<Items<string>>(initial.Count);

        var sum = 0.0;

        for (int i = 1; i < initial.Count; i++)
        {
            sum += initial[i].Probability;
            converted.Add(new Items<string> { Probability = sum, Item = initial[i].Item });
        }

        //The first element in the list has the biggest Probability,
        //so this element got probability of 1.
        converted.Add(new Items<string> { Probability = 1.0, Item = initial.First().Item });


        //Section 3
        for (int j = 0; j < 50; j++)
        {
            var probability = random.NextDouble();
            //var selected = converted.SkipWhile(i => i.Probability < probability).FirstOrDefault();
            var selected = converted.FirstOrDefault(i => i.Probability >= probability);
            Console.WriteLine("{0}.Selected class = {1}", j, selected.Item);
        }
    }

    private List<Items<string>> itemsToList(int[] predictedClass)
    {
        List<Items<string>> initial = new List<Items<string>>();
        for (int i = 0; i < weights.Length; i++)
        {
            initial.Add(new Items<string>
            {
                Probability = weights[i],
                Item = predictedClass[i].ToString()
            });
        }

        return initial;
    }

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