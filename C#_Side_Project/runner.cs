using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts;

namespace ConsoleApp
{
    class runner
    {
        public static Dataset dataset = Dataset.get_instance();

        public static void Main(string[] args)
        {
            dataset.new_PatientDetailes("204110290", "right", 182, (float)0.5, "yes", 60);
            dataset.new_data_line(1, (float)0.1, (float)0.9008626, (float)0.1519834, (float)0.02166149, 1, 51,
                (float)0.8, (float)0.4, (float)0.5, "Front-Bottom-Center");
            //dataset.read_file("PatientResults.csv");
            dataset.write_data_to_file();

            Console.WriteLine("Process finished!\n");
        }
    }
}
