#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class DebugBootstrap : ScriptableObject
{
    const string SIMULATOR_PATH = "Assets/XR Interaction Toolkit/3.0.8/XR Device Simulator/XR Device Simulator.prefab";
    const string NETWORKMANAGER_PATH = "Assets/Prefabs/NetworkManager.prefab";

    const string MISSION_SETTING_PATH = "Assets/Mission Settings/";
    static GameObject _simulator;

    [UnityEngine.RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        if (_simulator is null)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(SIMULATOR_PATH);
            _simulator = Instantiate(prefab);
            DontDestroyOnLoad(_simulator);
        }

        if (FMNetworkManager.instance is null || FindFirstObjectByType<FMNetworkManager>() == null)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(NETWORKMANAGER_PATH);
            Instantiate(prefab);
        }

        {
            var go = new GameObject("Play Data Tester");
            var com = go.AddComponent<PlayDataTest>();

            foreach (var a in System.IO.Directory.GetFiles(MISSION_SETTING_PATH))
            {
                if (a.EndsWith("asset") &&
                   AssetDatabase.LoadAssetAtPath<MissionSetting>(a) is MissionSetting setting)
                {
                    com.Settings.Add(setting);
                }
            }

            DontDestroyOnLoad(go);
        }

        Debug.Log("DebugBootstrap initialized");
    }
}
#endif