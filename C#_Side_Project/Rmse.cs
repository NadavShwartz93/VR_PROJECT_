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
    public float CalcRMSE()
    {
        int R;
        int vectorDimension = 2;//numberof cols
                                //for()
                                //for()
        return (float)0.0;//stam value
    }
}
