using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class PlayerData    // still in progress
{
    public string stuID;

    [System.Serializable]
    public class MissionData
    {
        public string name;
        public float time;

        public int score
        {
            get
            {
                int result = 0;
                foreach (var stg in stgDatas)
                    result += stg.score;

                return result;
            }
        }
        public bool complete;

        public List<StgData> stgDatas = new List<StgData>();

        [System.Serializable]
        public class StgData
        {
            public int score;
            public float time;
            public string stgName;
        }
    }

    public List<MissionData> missionDatas;

    public static MissionData current;

    //Constructor
    public PlayerData()
    {
        stuID = DateTime.Now.ToString("MM-dd-yyyy");
        missionDatas = new List<MissionData>();
    }

    public void SetStageData(MissionData.StgData stgData)
    {
        current.stgDatas.Add(stgData);
    }

    public MissionData SetMissionData(string name)
    {
        var mData = new MissionData();
        if (missionDatas.FindIndex((m) => m.name == name) != -1)
        {
            mData = missionDatas.Find(m => m.name == name);
        }
        else
        {
            mData.name = name;
            missionDatas.Add(mData);
        }
        current = mData;

        mData.stgDatas = new List<MissionData.StgData>();

        return mData;
    }

    public MissionData GetMissionData(string name)
    {
        for (int i = 0; i < missionDatas.Count; ++i)
        {
            if (missionDatas[i].name == name)
                return missionDatas[i];
        }

        return null;
    }

    public int GetAllScore(string name)
    {
        int score = 0;
        var mData = missionDatas.Find((m) => m.name == name);

        foreach (var s in mData.stgDatas)
            score += s.score;

        return score;
    }
}
