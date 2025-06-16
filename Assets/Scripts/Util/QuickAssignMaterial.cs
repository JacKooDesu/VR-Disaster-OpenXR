using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickAssignMaterial : MonoBehaviour
{
    public GameObject prefab;
    public Transform target;
    [ContextMenu("指定")]
    void Assign()
    {
        foreach (Transform t in target)
        {
            var n = t.name;

            if (!prefab.transform.Find(n))
                continue;

            var origin = prefab.transform.Find(n);
            var renderer = t.GetComponent<Renderer>();
            renderer.sharedMaterials = origin.GetComponent<Renderer>().sharedMaterials;

            // Childs field
            var childRenderers = t.GetComponentsInChildren<Renderer>();
            var childOrigins = origin.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < childRenderers.Length; ++i)
                childRenderers[i].sharedMaterials = childOrigins[i].sharedMaterials;
        }
    }
}
