using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class CollaborativeFiltering
    {
        private static CollaborativeFiltering instance = null;
        private CollaborativeFiltering()
        {
            instance = this;

        }
        public static CollaborativeFiltering Get_instance()
        {
            if (instance == null)
            {
                instance = new CollaborativeFiltering();
            }
            return instance;
        }
    }
}

