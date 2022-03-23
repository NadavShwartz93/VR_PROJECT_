using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class runner
{
    public static Dataset dataset = Dataset.Get_instance();

    public static string path = @"E:\Program files\Unity_project\VR_PROJECT_\VR_Anat_results";
    public static void Main(string[] args)
    {
        //Dataset.Get_instance().New_PatientDetailes("280", "left", 158, (float)0.43799999356269839, "yes", 180);
        /*Dataset.Get_instance().New_data_line(1, (float)0.1, (float)0.9008626, (float)0.1519834, (float)0.02166149, 1, 51,
            (float)0.8, (float)0.4, (float)0.5, "Front-Bottom-Center");
        dataset.read_file(path+@"\yogi\r1\"+"PatientResults.csv");*/
        //dataset.read_file("PatientResults.csv");
        //Dataset.Get_instance().read_file("PatientResults.csv");
        //Dataset.Get_instance().Write_data_to_file();
        //Kmeans km = new Kmeans(numberColumns);

        Kmeans.Get_instance().Train(10);

        KNN.Get_instance().start();

        CollaborativeFiltering.Get_instance().getInputs();

        Console.WriteLine("Process finished!\n");

        Console.ReadKey();
    }
}