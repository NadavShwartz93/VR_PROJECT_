using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class Runner
{
    public static Dataset dataset = Dataset.Get_instance();

    public static void Main(string[] args)
    {
        Kmeans.Get_instance().Train(Globals.numOfTrainingIteration);

        KNN.Get_instance().start();

        CollaborativeFiltering.Get_instance().calculate_CF();

        Console.WriteLine("Process finished!\n");

        Console.ReadKey();
    }
}