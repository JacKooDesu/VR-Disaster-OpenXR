using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitQuicker : MonoBehaviour
{
    public OrbitQuickSetting setting;

    [ContextMenu("做呢")]
    public void DoIt()
    {
        setting.Orbit();
    }
}

[System.Serializable]
public class OrbitQuickSetting
{
    public Transform parent;    // 父物件
    public Transform[] transforms;
    public int layerCount;
    public float layerHeight;
    public float radius;

    public void Orbit()
    {
        int itemEveryLayer = transforms.Length / layerCount + 1;
        for (int i = 0; i < layerCount; ++i)
        {
            for (int j = 0; j < itemEveryLayer; ++j)
            {
                if ((i * itemEveryLayer) + j >= transforms.Length)
                    return;
                GameObject g = new GameObject("Temp");
                g.transform.SetParent(parent);
                transforms[(i * itemEveryLayer) + j].SetParent(g.transform);
                g.transform.localPosition = new Vector3(0, layerHeight * (float)i, 0);
                transforms[(i * itemEveryLayer) + j].localPosition = Vector3.forward * radius;
                g.transform.localEulerAngles = Vector3.up * 360f * ((float)j / (float)(itemEveryLayer - 1));

                transforms[(i * itemEveryLayer) + j].SetParent(parent);

                MonoBehaviour.DestroyImmediate(g);
            }
        }
    }
}
