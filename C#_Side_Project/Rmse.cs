using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp;

namespace ConsoleApp
{
    class Rmse
    {
        private static Rmse instance = null;
        private Rmse()
        {
            instance = this;

        }
        public static Rmse Get_instance()
        {
            if (instance == null)
            {
                instance = new Rmse();
            }
            return instance;
        }
    }
}

