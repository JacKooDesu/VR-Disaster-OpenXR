using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceToPlayer : MonoBehaviour
{
    public enum Axis
    {
        x, y, z, none
    }
    public Axis constraint = Axis.y;
    public bool reverse = true; // 2d object will back face to player
    void LateUpdate()
    {
        if (GameHandler.Singleton.player.head == null)
            return;

        var headPos = GameHandler.Singleton.player.head.position;
        var vec = headPos - transform.position;
        vec = reverse ? -vec : vec;

        vec.x = constraint == Axis.x ? 0 : vec.x;
        vec.y = constraint == Axis.y ? 0 : vec.y;
        vec.z = constraint == Axis.z ? 0 : vec.z;
        if (vec == Vector3.zero)
            return;
        transform.rotation = Quaternion.LookRotation(vec);
    }
}
