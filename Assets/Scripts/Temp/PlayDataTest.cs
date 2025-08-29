
using System.Collections.Generic;
using UnityEngine;

public class PlayDataTest : MonoBehaviour
{
    public List<MissionSetting> Settings { get; private set; } = new();

    [ContextMenu("Test Build Full Data")]
    void Test()
    {
        foreach (var mission in Settings)
        {
            GameHandler.playerData.SetMissionData(mission.name);
            foreach (var stgSetting in mission.settings)
            {

                var stgData = new PlayerData.MissionData.StgData();
                stgData.stgName = stgSetting.name;
                stgData.score = stgSetting.score;
                stgData.time = Random.Range(0, 1000f);

                GameHandler.playerData.SetStageData(stgData);
            }
        }
    }
}
