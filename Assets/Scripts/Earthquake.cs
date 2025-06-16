using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class Earthquake : MonoBehaviour
{
    float acc;

    public GameObject floor;
    Rigidbody rb;

    public bool isQuaking;

    public float speed = 1f;
    public float currentSpeed = 0f;
    public float amount = 5f;
    public float currentAmount = 0f;

    public float randMin = -1f;
    public float randMax = +1f;

    private void Start()
    {
        if (floor == null)
            floor = gameObject;

        BindRigidbody();

        // StartCoroutine(StartQuake(20));
        //rb.isKinematic = true;
    }

    void BindRigidbody()
    {
        rb = floor.GetComponent<Rigidbody>();
    }

    public void SetQuake(float t)
    {
        StartQuake(t).Forget();
    }

    async UniTask StartQuake(float t)
    {
        BindRigidbody();

        float f = 0f;

        float counter = 0;
        isQuaking = true;

        rb.isKinematic = false;

        Vector3 forceDir = new Vector3(1, 0, 1);

        while (isQuaking)
        {
            if (counter < t)
            {
                currentAmount = Mathf.Lerp(currentAmount, amount, .01f);
                counter += Time.deltaTime;
            }
            else
            {
                currentAmount = Mathf.Lerp(currentAmount, 0, .05f);
                if (currentAmount <= amount * 0.05f)
                    isQuaking = false;
            }

            float randX = Random.Range(randMin, randMax);
            float randY = Random.Range(randMin, randMax);

            Vector3 force = new Vector3(
                Mathf.Sin(Time.time * speed) * currentAmount + randX,
                0,
                Mathf.Cos(Time.time * speed) * currentAmount + randY
                );

            //transform.Translate(force * Time.deltaTime);
            rb.linearVelocity = force;
            //rb.AddForce(force,ForceMode.Acceleration);
            // print(force);

            await UniTask.Yield();
        }

        rb.isKinematic = true;
    }


}
