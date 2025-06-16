using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPanel : MonoBehaviour
{
    public ObjectSwitcher switcher;

    [Header("UI設定")]
    public Text titleTextObj;
    public Text contentTextObj;

    public Color selectColor, unSelectColor;


    [System.Serializable]
    public class Setting
    {
        public string title;
        [TextArea(2, 10)]
        public string content;
        public GameObject targetImage;
    }

    public Setting[] settings;

    private void Awake()
    {
        switcher.OnSwitch += (current, old) =>
        {
            settings[old].targetImage.SetActive(false);
            if (switcher.OldButtonSelector)
                switcher.OldButtonSelector.GetComponentInChildren<Text>().color = unSelectColor;

            settings[current].targetImage.SetActive(true);
            titleTextObj.text = settings[current].title;
            contentTextObj.text = settings[current].content;

            if (switcher.CurrentButtonSelector)
                switcher.CurrentButtonSelector.GetComponentInChildren<Text>().color = selectColor;
        };
    }
}
