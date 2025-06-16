using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public Transform fireParent;

    public float extinguishTime = 3f;
    public float hasExtinguishTime = 0f;

    public float waitTime = .5f;
    public float hasWait = 0f;


    private void OnParticleCollision(GameObject other)
    {
        if (other.name == "Powder")
        {
            print("extinguishing");
            if (hasExtinguishTime < extinguishTime)
            {
                hasExtinguishTime += Time.deltaTime;
                hasWait = 0;
            }
            else
            {
                GetComponent<ParticleSystem>().Stop();
                GameHandler.Singleton.StageFinish();
                GetComponent<BoxCollider>().enabled = false;
            }
        }
    }

    private void Update()
    {
        hasWait += Time.deltaTime;
    }
}
