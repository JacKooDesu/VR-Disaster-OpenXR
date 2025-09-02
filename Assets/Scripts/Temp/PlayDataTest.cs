
using System.Collections.Generic;
using UnityEngine;

public class PlayDataTest : MonoBehaviour
{
    public List<MissionSetting> Settings { get; private set; } = new();
    [field: SerializeField]
    public List<string> PlayerNames { get; private set; } = new();
    [SerializeField]
    bool _autoSend;

    [ContextMenu("Test Build Full Data")]
    void Test()
    {
        var origin = GameHandler.playerData;

        foreach (var p in PlayerNames)
        {
            GameHandler.playerData = new(p);
            foreach (var mission in Settings)
            {
                GameHandler.playerData.SetMissionData(mission.missionName);
                foreach (var stgSetting in mission.settings)
                {

                    var stgData = new PlayerData.MissionData.StgData();
                    stgData.stgName = stgSetting.name;
                    stgData.score = stgSetting.score;
                    stgData.time = Random.Range(0, 1000f);

                    GameHandler.playerData.SetStageData(stgData);
                }
            }

            if (_autoSend)
            {
                GameHandler.Singleton.SavePlayerData();
            }
            else
            {
                return;
            }
        }

        GameHandler.playerData = origin;
    }
}
