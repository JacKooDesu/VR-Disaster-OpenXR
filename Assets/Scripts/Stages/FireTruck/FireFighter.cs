using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFighter : Stage
{
    public float minTime;
    public float maxTime;

    public UIQuickSetting ui1;
    public UIQuickSetting ui2;
    public UIQuickSetting ui3;

    public Transform fireSpot;

    public ParticleSystem powder;
    public GameObject Extinguisher;

    bool hasAim = false;

    public override void OnBegin()
    {
        //base.OnBegin();

        ui1.TurnOn();
        StartCoroutine(GameHandler.Singleton.Counter(3f, 3f, delegate
        {
            ui1.TurnOff();
        }));

    }

    public override void OnUpdate()
    {
        if (!hasAim)
        {
            Transform origin = GameHandler.Singleton.cam.transform;

            RaycastHit hit;
            if (origin != null)
            {
                Ray ray = new Ray(origin.position, origin.forward);
                if (Physics.Raycast(ray, out hit, 10f))
                {
                    print(hit.transform.name);
                    if (hit.transform == fireSpot)
                    {
                        AimAndStrafy();
                        hasAim = true;
                    }
                }
            }
        }

    }

    void AimAndStrafy()
    {
        ui1.TurnOff();
        ui2.TurnOn();
        StartCoroutine(GameHandler.Singleton.Counter(3f, 3f, delegate
        {
            ui2.TurnOff();
            ui3.TurnOn();
        }));

        StartCoroutine(GameHandler.Singleton.Counter(6f, 6f, delegate { ui3.TurnOff(); }));
        StartCoroutine(PlayExtinguisher());
    }

    IEnumerator PlayExtinguisher()
    {
        JacDev.Audio.FireTruck audio = (JacDev.Audio.FireTruck)GameHandler.Singleton.audioHandler;
        while (!isFinish)
        {
            if (Input.GetMouseButton(0))
            {
                if (!powder.isPlaying)
                {
                    powder.GetComponent<ParticleSystem>().Play();
                }

                audio.PlayAudio(audio.extinguisher, false, transform);
            }
            //sprayParticle.gameObject.SetActive(true);
            else
            {
                powder.GetComponent<ParticleSystem>().Stop();
                if (GetComponentInChildren<AudioSource>())
                    Destroy(GetComponentInChildren<AudioSource>().gameObject);
            }
            yield return null;
        }
    }


    public override void OnFinish()
    {
        base.OnFinish();
        powder.GetComponent<ParticleSystem>().Stop();
        Extinguisher.SetActive(false);

    }


}
