using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ResultUI : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject missionContent;

    [Header("UI綁定")]
    public Text missionHeader;
    public Text congratulation;
    public Text score;

    public Transform missionContentParent;
    public Text missionHint;
    public Image missionIcon;

    public List<MissionSetting> missionSettings = new List<MissionSetting>();

    public void Start()
    {
        if (PlayerData.current == null)
            return;
        switch (PlayerData.current.name)
        {
            case "Earthquake":
                InitUI(missionSettings[0]);
                break;

            case "FireTruck":
                InitUI(missionSettings[1]);
                break;

            case "Flood":
                InitUI(missionSettings[2]);
                break;

            default:
                InitUI(missionSettings[0]);
                break;
        }
    }

    void InitUI(MissionSetting mSetting)
    {
        // var pData = GameHandler.playerData;
        var mData = PlayerData.current;

        missionHeader.text = mSetting.missionName;
        congratulation.text = $"恭喜你，\n完成{mSetting.missionName}任務！";

        score.text = mData.score.ToString();
        string hint = string.Empty;

        for (int i = 0; i < mSetting.settings.Count; ++i)
        {
            var stgSetting = mSetting.settings[i];
            var stgData = mData.stgDatas[i];

            if (stgSetting.desc == string.Empty)
                continue;
            var go = Instantiate(missionContent, missionContentParent);
            go.transform.GetComponentInChildren<Text>().text = stgSetting.desc;
            bool getFullScore = stgSetting.score == stgData.score;
            go.transform.Find("Checked").gameObject.SetActive(getFullScore);
            go.transform.Find("UnChecked").gameObject.SetActive(!getFullScore);

            if (!getFullScore && stgSetting.hint != string.Empty)
                hint += $"{stgSetting.hint}\n";
        }


        hint += hint == string.Empty ? mSetting.defaultHint : "再來挑戰看看！";


        missionHint.text = hint;

        missionIcon.sprite = mSetting.icon;
    }
}

