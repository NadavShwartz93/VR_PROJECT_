using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript22 : MonoBehaviour
{
    //members
    public int m_var1;
    private GameManager m_algorithm;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("This is a start!");
    }

    private void Awake()
    {
        
    }
    private void doStuff()
    {

    }
    // Update is called once per frame
    void Update()
    {
        doStuff();
        for (int i = 0; i<10;i++)
        {
            //Debug.Log("Hello World!");
        }
    }
}
