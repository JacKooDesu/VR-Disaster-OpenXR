using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMoveTemper : MonoBehaviour
{
    public Transform father;
    public GameObject prefab;

    private void Start() {
        GameObject g = new GameObject();
        g.transform.SetParent(transform);
        g.transform.localPosition = Vector3.zero;

        for(int i=0;i<father.childCount;++i){
            GameObject c = Instantiate(prefab);
            Transform t = father.GetChild(i);
            c.transform.SetParent(father);

            c.transform.localPosition = t.localPosition;
            c.transform.rotation = t.rotation;
            c.transform.localScale = t.localScale;
            c.name = prefab.name + (i+1).ToString();
            
            c.transform.SetParent(g.transform);
        }
    }
}
