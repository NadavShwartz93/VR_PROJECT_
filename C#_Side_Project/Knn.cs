using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Knn
    {
        private static Knn instance = null;
        private Knn()
        {
            instance = this;

        }
        public static Knn Get_instance()
        {
            if (instance == null)
            {
                instance = new Knn();
            }
            return instance;
        }
    }
}
