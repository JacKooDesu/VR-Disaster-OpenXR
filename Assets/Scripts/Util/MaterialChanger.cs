using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
public class MaterialChanger
{
    [Header("主物件")]
    public Transform parent;
    public bool skyboxInclude = false;
    static Color skyboxOrigin;
    List<Renderer> renderers = new List<Renderer>();
    [System.Serializable]
    public class AvoidRendererSetting
    {
        public Transform target;
        public bool containChild = false;
        public bool outline;
    }
    [Header("例外")]
    public List<AvoidRendererSetting> avoidSettings = new List<AvoidRendererSetting>();
    List<Renderer> avoidRenderers = new List<Renderer>();
    static Dictionary<int, Material[]> originMats = new Dictionary<int, Material[]>();

    [Header("目標顏色")]
    public Color targetColor = new Color(.2f, .2f, .2f, 1f);
    public bool effectAlpha = false;

    bool hasSetup = false;

    void Setup()
    {
        foreach (var avoidSetting in avoidSettings)
        {
            if (avoidSetting.outline)
            {
                var outline = avoidSetting.target.gameObject.AddComponent<Outline>();
                outline.OutlineColor = Color.red;
                outline.OutlineWidth = 10f;
            }


            if (avoidSetting.target.GetComponent<Renderer>() != null)
                avoidRenderers.Add(avoidSetting.target.GetComponent<Renderer>());

            if (avoidSetting.containChild)
                avoidRenderers.AddRange(avoidSetting.target.GetComponentsInChildren<Renderer>());
        }

        foreach (var r in parent.GetComponentsInChildren<Renderer>())
        {
            if (avoidRenderers.Contains(r))
                continue;

            if (!r.gameObject.activeInHierarchy)
                continue;

            renderers.Add(r);
        }

        if (skyboxInclude && RenderSettings.skybox.HasProperty("_Tint"))
        {
            skyboxOrigin = RenderSettings.skybox.GetColor("_Tint");
            Material tempSkybox = new Material(RenderSettings.skybox);
            RenderSettings.skybox = tempSkybox;
        }
        else
        {
            skyboxInclude = false;
        }


        hasSetup = true;
    }

    public void ChangeColor(float t = 1f)
    {
        if (!hasSetup)
            Setup();

        foreach (var r in renderers)
        {
            int id = r.GetInstanceID();
            int length = r.sharedMaterials.Length;

            if (!originMats.ContainsKey(id))
            {
                Material[] mats = new Material[r.sharedMaterials.Length];
                for (int i = 0; i < length; ++i)
                    mats[i] = Object.Instantiate(r.sharedMaterials[i]);
                originMats.Add(r.GetInstanceID(), mats);
            }

            Material[] tempMats = new Material[length];
            for (int i = 0; i < length; ++i)
                tempMats[i] = Object.Instantiate(r.sharedMaterials[i]);
            r.sharedMaterials = tempMats;

            DOTween.Kill(id);
            foreach (var m in r.sharedMaterials)
            {
                if (!m.HasProperty("_Color"))
                    continue;

                var tempColor = targetColor;
                if (!effectAlpha)
                    tempColor.a = m.GetColor("_Color").a;

                m.DOColor(tempColor, "_Color", t).SetId(id);
            }
        }

        if (skyboxInclude)
        {
            RenderSettings.skybox.DOColor(targetColor, "_Tint", t);
        }
    }

    public void BackOriginColor(float t=1f)
    {
        if (!hasSetup)
            Setup();

        for (int i = 0; i < renderers.Count; ++i)
        {
            var renderer = renderers[i];
            if (renderer == null)
                continue;
            int id = renderer.GetInstanceID();

            var mats = originMats[id];
            DOTween.Kill(id);
            for (int j = 0; j < mats.Length; ++j)
            {
                var m = renderer.sharedMaterials[j];
                var originM = mats[j];

                if (!m.HasProperty("_Color"))
                    continue;

                var tween = m.DOColor(originM.color, t).SetId(id);

                if (j == mats.Length - 1)
                    tween.OnComplete(() =>
                    {
                        renderer.sharedMaterials = mats;
                    });
            }
        }

        foreach (var avoidSetting in avoidSettings)
        {
            if (avoidSetting.outline)
                Component.Destroy(avoidSetting.target.GetComponent<Outline>());
        }

        if (skyboxInclude)
        {
            RenderSettings.skybox.DOColor(skyboxOrigin, "_Tint", t);
        }
    }
}