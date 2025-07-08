#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class DebugBootstrap : ScriptableObject
{
    const string SIMULATOR_PATH = "Assets/XR Interaction Toolkit/3.0.8/XR Device Simulator/XR Device Simulator.prefab";
    static GameObject _simulator;

    [UnityEngine.RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(SIMULATOR_PATH);
        if (_simulator is null)
        {
            _simulator = Instantiate(prefab);
            DontDestroyOnLoad(_simulator);
        }

        Debug.Log("DebugBootstrap initialized");
    }
}
#endif