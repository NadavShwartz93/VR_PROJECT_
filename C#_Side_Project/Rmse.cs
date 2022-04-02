using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Rmse
{
    private static Rmse instance = null;
    private float rmse;
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
    /*The meaning of the values:
    R -  denotes the number of records in the matrix of neighbors.
    n - denotes the vector dimension.
    Fa, i - denotes the game setting feature.
    Pi - denotes the game setting feature that has been predicted.
    */

    //This is just Conspectus with simple example!
    public float RmseCalc()

    {
        int R = 5, n = 3, arrSize = 3; // n must be smaller or equal to arrSize
        double finalRes, s1 = 0, s2 = 0;
        double[] Fa = new double[arrSize]; //Fai
        double[] P = new double[arrSize];   //Pi

        //Be carfull there is small p and big P
        for (int p = 1; p < R; p++)
        {
            for (int i = 1; i < n; i++)
            {
                s2 = Fa[i] - P[i];
            }
            s1 = Math.Sqrt(1 / n * s2);
        }
        finalRes = 1 / R * s1;

        return (float)finalRes;
    }


}
