using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        InitUI(missionSettings.Find(
            x => x.name == PlayerData.current.name));
    }

    void InitUI(MissionSetting mSetting)
    {
        // var pData = GameHandler.playerData;
        var mData = PlayerData.current;

        missionHeader.text = mSetting.missionName;
        congratulation.text = $"恭喜你，\n完成{mSetting.missionName}任務！";

        score.text = mData.score.ToString();
        string hint = string.Empty;


        var arr = mData.stgDatas
            .Zip(
                mSetting.settings,
                (pData, stgData) => new { pData, stgData });
        foreach (var item in arr)
        {
            var stgSetting = item.stgData;
            var stgData = item.pData;

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

