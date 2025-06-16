using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FindTable : Stage
{
    public GameObject dchUI;
    public MaterialChanger tableHint;
    public override void OnBegin()
    {
        base.OnBegin();

        tableHint.ChangeColor();

        // UI.TurnOn();

        JacDev.Audio.Earthquake audio = (JacDev.Audio.Earthquake)GameHandler.Singleton.audioHandler;
        audio.PlaySound(audio.goUnderTable);
        audio.currentPlayingSound = null;

        FindObjectOfType<HintCanvas>().SetHintText("找到桌椅避難掩護！", true);
        var tp = FindStageObject<TeleportPoint>();
        tp.onTeleportAction.AddListener(() =>
        {
            isFinish = true;
            Destroy(tp.gameObject);
        });

        FindObjectOfType<NavMeshSurface>().BuildNavMesh();
        GameHandler.Singleton.player.PathFinding(tp.transform.position);

        dchUI.SetActive(true);
    }

    public override void OnFinish()
    {
        base.OnFinish();
        tableHint.BackOriginColor();
        GameHandler.Singleton.player.line.gameObject.SetActive(false);
    }
}
