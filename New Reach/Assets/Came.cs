using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Came : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        transform.position = new Vector3(0, (float)(0 - (0.377 * GameManager.instance.current_player.height) + 0.1), 0);
    }
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
