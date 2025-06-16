using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintObejctCamera : MonoBehaviour
{
    public Camera cam;
    public Canvas canvas;
    public GameObject hintUiPrefab;

    [Header("設定")]
    public float limitTime; // 超過這個時長就顯示UI

    public List<HintObject> hintObjects = new List<HintObject>();
    public List<HintUI> hintUIs = new List<HintUI>();

    private void OnEnable()
    {
        if (Application.isEditor)
            cam.enabled = false;
    }
    // private void Start()
    // {
    //     hintObjects.AddRange(FindObjectsOfType<HintObject>());

    //     foreach (var ho in hintObjects)
    //         InitUi(ho);
    // }

    // void InitUi(HintObject ho)
    // {
    //     var ui = Instantiate(hintUiPrefab, canvas.transform).GetComponent<HintUI>();

    //     ui.Setup(ho, this);
    //     ho.ui = ui;
    // }
}
