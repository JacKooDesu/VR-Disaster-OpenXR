using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningDetect : MonoBehaviour
{
    public Player player;
    private void OnTriggerEnter(Collider other)
    {
        player.ShowWarning();
    }
}
