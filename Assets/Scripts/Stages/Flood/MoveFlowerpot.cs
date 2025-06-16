using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MoveFlowerpot : Stage
{
    public Plant[] plants;

    public override void OnBegin()
    {
        base.OnBegin();
        foreach (Plant p in plants)
        {
            p.GetComponent<Outline>().enabled = true;
        }

        JacDev.Audio.Flood a = (JacDev.Audio.Flood)GameHandler.Singleton.audioHandler;
        AudioSource broadcast = a.PlaySound(a.broadcast1);

        GameHandler.Singleton.Counter(
            3f,
            delegate
            {
                broadcast.volume = .4f;
                a.PlaySound(a.plantDown);
            }
        ).Forget();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        foreach (Plant p in plants)
        {
            if (!p.hasMoved)
                return;
        }

        GameHandler.Singleton.StageFinish();
    }

    public override void OnFinish()
    {
        base.OnFinish();

        foreach (var p in plants)
            if (p.isBroken) SubScore(5);
    }
}
