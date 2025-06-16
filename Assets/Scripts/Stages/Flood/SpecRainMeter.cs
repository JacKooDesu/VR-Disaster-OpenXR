using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class SpecRainMeter : Stage
{
    public InteracableObject interactArea;
    public UIQuickSetting ui;

    [Header("UI設定")]
    public Button[] selections;
    public string[] selectionHints;
    public Text hintText;
    public Color mainColor;
    public Button ConfirmBtn;
    int currentSelectIndex;

    public override void OnBegin()
    {
        // interactArea.Interactable = true;
        // interactArea.interactableOutline = false;
        // interactArea.GetComponent<Outline>().enabled = true;
        // interactArea.onHoverEvent.AddListener(() => ShowUI());

        // GameHandler.Singleton.player.PathFinding(interactArea.transform.position);

        base.OnBegin();
        BindButtons();
        onGetToTarget += ShowUI;
    }

    void BindButtons()
    {
        JacDev.Audio.Flood a = (JacDev.Audio.Flood)GameHandler.Singleton.audioHandler;
        var audios = new AudioClip[]{
            a.specWaterMeter500,
            a.specWaterMeter350,
            a.specWaterMeter200};
        AudioSource audioSource = null;

        for (int i = 0; i < selections.Length; ++i)
        {
            int x = i;
            var btn = selections[i];
            var s = selectionHints[i];

            btn.onClick.AddListener(
                () =>
                {
                    foreach (var b in selections)
                        b.GetComponent<Image>().color = new Color(0, 0, 0, 0);

                    btn.GetComponent<Image>().color = mainColor;

                    hintText.text = s;

                    currentSelectIndex = x;

                    ConfirmBtn.interactable = true;

                    if (audioSource != null)
                        audioSource.Stop();
                    audioSource = a.PlaySound(audios[x]);
                }
            );
        }

        ConfirmBtn.interactable = false;
        ConfirmBtn.onClick.AddListener(() =>
        {
            isFinish = true;
            ui.TurnOff();
            audioSource.Stop();
        });
    }

    void ShowUI()
    {
        // interactArea.Interactable = false;
        ui.TurnOn();

        // JacDev.Audio.Flood a = (JacDev.Audio.Flood)GameHandler.Singleton.audioHandler;
        // var sound = a.PlaySound(a.specWaterMeter);

        // onFinishEvent += ()=>Destroy(sound.gameObject);
    }

    public override void OnFinish()
    {
        base.OnFinish();

        // ((JacDev.Audio.Flood)GameHandler.Singleton.audioHandler).StopAll();

        if (currentSelectIndex != 1)
            score = 0;
    }
}
