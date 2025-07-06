using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputHandler : MonoBehaviour
{
    static InputHandler singleton = null;
    public static InputHandler Singleton
    {
        get
        {
            singleton = FindAnyObjectByType(typeof(InputHandler)) as InputHandler;

            if (singleton == null)
            {
                GameObject g = new GameObject("InputHandler");
                singleton = g.AddComponent<InputHandler>();
            }

            return singleton;
        }
    }

    public bool isTouch = false;

    public bool isHolding = false;
    public bool isClick = false;

    public float clickTime = 1f;
    public float holdTime = 0;

    public Transform aimer;
    public float aimerMaxSize;
    public float aimerMinSize;

    int mouseBtn = 0;

    private void Update()
    {
        if (Input.GetMouseButton(mouseBtn))
        {
            if (!isTouch)
            {
                StartCoroutine(TouchingCounter());
                print("click");
            }

        }
        if (Input.GetMouseButtonUp(mouseBtn))
        {
            isHolding = false;
            isTouch = false;
            holdTime = 0;
        }
    }

    IEnumerator TouchingCounter()
    {
        isTouch = true;

        isClick = false;
        isHolding = false;
        while (true)
        {
            if (Input.GetMouseButtonUp(mouseBtn))
                break;

            holdTime += Time.deltaTime;
            if (holdTime >= clickTime)
            {
                isHolding = true;
                Debug.Log("hold");
                break;
            }
            yield return null;
        }

        if (holdTime < clickTime)
        {
            isClick = true;
            //Debug.Log("click");
        }

        while (isTouch)
            yield return null;

        holdTime = 0;
        yield return null;
    }
}
