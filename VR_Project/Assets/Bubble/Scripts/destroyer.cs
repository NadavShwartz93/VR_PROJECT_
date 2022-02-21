using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyer : MonoBehaviour
{

    public float lifeTime = 10f;
    public ParticleSystem noise;
    // Update is called once per frame

    private void OnEnable()
    {
        //lifeTime=10f;
        noise.Play();
    }

    private void OnDisable()
    {

        noise.Stop();
        noise.Clear();
    }

    // void Update()
    // {
    // 	if (lifeTime > 0 && gameObject.active)
    // 	{
    // 		lifeTime -= Time.deltaTime;
    // 		if (lifeTime <= 0)
    // 		{
    // 			Destruction();
    // 		}
    // 	}

    // 	if (this.transform.position.y <= -20)
    // 	{
    // 		Destruction();
    // 	}
    // }

    // void OnCollisionEnter(Collision coll)
    // {
    // 	if (coll.gameObject.name == "destroyer")
    // 	{
    // 		Destruction();
    // 	}
    // }

    // void Destruction()
    // {
    // 	//if(noise==null) return;
    // 	noise.Emit(60);

    // 	//Destroy(this.gameObject);

    // }
}