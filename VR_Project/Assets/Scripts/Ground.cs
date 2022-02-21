using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    // Start is called before the first frame update

    private void Awake()
    {
        
        transform.position = new Vector3(0, (float)(0 - (0.377 * GameManager.instance.current_player.height)/10), 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
