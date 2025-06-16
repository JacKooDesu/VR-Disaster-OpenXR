using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MissionSetting", menuName = "Project/MissionSetting", order = 0)]
public class MissionSetting : ScriptableObject
{
    public string missionName;
    [TextArea(2, 10)]
    public string defaultHint;

    [System.Serializable]
    public class Setting
    {
        public string name;
        public int score;
        public string desc;

        [TextArea(2, 10)]
        public string hint;
    }

    public List<Setting> settings = new List<Setting>();

    public Sprite icon;
}
