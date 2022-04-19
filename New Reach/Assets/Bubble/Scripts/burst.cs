using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class burst : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

  void onTriggerEnter(Collider other)
    {
        //Debug.Log(gameObject.name + " was triggred by " + other.gameObject.name);
        Destroy(other);
    }


}
