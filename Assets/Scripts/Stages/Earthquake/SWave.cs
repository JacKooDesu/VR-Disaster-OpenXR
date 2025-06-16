using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;
using DG.Tweening;

public class SWave : Stage
{
    public Earthquake earthquake;

    public GameObject roof;
    Transform tempRoof;
    public int breakChance = 60;

    public Transform tableTop;

    public UIQuickSetting waringHUD;

    public Animator elevator;

    public Material transparentMat;

    public override void OnBegin()
    {
        base.OnBegin();
        DOTween.To(() => RenderSettings.ambientIntensity, x => RenderSettings.ambientIntensity = x, .2f, .5f);
        GameHandler.Singleton.player.SetCanMove(false);
        earthquake.SetQuake(20f);

        StartCoroutine(GameHandler.Singleton.Counter(
            20, 20,
            delegate
            {
                isFinish = true;
            }
        ));

        BreakRoof();
        StartCoroutine(MakeFog(5f, 40f));

        JacDev.Audio.Earthquake audio = (JacDev.Audio.Earthquake)GameHandler.Singleton.audioHandler;
        audio.ClearSpeaker();
        audio.PlayAudio(audio.SWave, false);
    }

    public override void OnUpdate()
    {
        var head = GameHandler.Singleton.player.head;
        if (head.position.y > tableTop.position.y)
        {
            waringHUD.TurnOn();
        }
        else
        {
            waringHUD.TurnOff();
        }
    }

    public override void OnFinish()
    {
        //base.OnFinish();
        elevator.SetTrigger("Broken");
        // warningHUD is no longer exist in cardboard
        //waringHUD.GetComponent<UIQuickSetting>().TurnOff();
        // GameHandler.Singleton.cam.GetComponent<BlurOptimized>().enabled = false;

        // tweener.MoveNextPoint();

        foreach (Transform t in tempRoof)
        {
            var originMats = t.GetComponent<Renderer>().sharedMaterials;
            var length = originMats.Length;
            Material[] mats = new Material[length];
            for (int i = 0; i < length; ++i)
            {
                mats[i] = Object.Instantiate(transparentMat);
                mats[i].color = originMats[i].color;
            }

            t.GetComponent<Renderer>().sharedMaterials = mats;
        }

        var matChanger = new MaterialChanger
        {
            parent = tempRoof,
            effectAlpha = true,
            targetColor = new Color(.2f, .2f, .2f, .3f)
        };
        matChanger.ChangeColor();

        waringHUD.TurnOff();
    }

    void BreakRoof()
    {
        tempRoof = new GameObject("TempRoof").transform;
        tempRoof.SetParent(roof.transform.parent);
        foreach (Transform t in roof.transform)
        {
            if (Random.Range(0, 100) < breakChance)
            {
                t.GetComponent<Rigidbody>().isKinematic = false;
                t.SetParent(tempRoof);
                // t.GetComponent<CustomInteractable>().fishRodInteract = true;
            }
        }
    }

    public IEnumerator MakeFog(float time, float fogEnd)
    {
        float t = 0;
        float maxDistance = 1000;
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogEndDistance = maxDistance;

        while (t < time)
        {
            t += Time.deltaTime;

            RenderSettings.fogEndDistance -= ((maxDistance - fogEnd) / time * Time.deltaTime);

            yield return null;
        }
    }
}
