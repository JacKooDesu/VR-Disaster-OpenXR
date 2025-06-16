using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
public class CameraCapture : MonoBehaviour
{
    new Camera camera;

    public Vector2Int size;
    public int depth;

    public string folderName;

    [ContextMenu("Capture")]
    void Capture()
    {
        if (transform.childCount == 0)
        {
            Debug.Log("無子物件可拍攝");
            return;
        }

        string path = $"{Application.dataPath}/{folderName}";

        if (!System.IO.Directory.Exists(path))
            System.IO.Directory.CreateDirectory(path);

        path += $"/{transform.GetChild(0).gameObject.name}.png";

        camera = camera ?? GetComponent<Camera>();

        RenderTexture texture = new RenderTexture(size.x, size.y, depth);
        camera.targetTexture = texture;
        Texture2D screenshot = new Texture2D(size.x, size.y, TextureFormat.RGBA32, false);
        camera.Render();
        RenderTexture.active = texture;
        screenshot.ReadPixels(new Rect(0, 0, size.x, size.y), 0, 0);
        camera.targetTexture = null;
        RenderTexture.active = null;

        if (Application.isEditor)
            DestroyImmediate(texture);
        else
            Destroy(texture);

        byte[] pixels = screenshot.EncodeToPNG();

        System.IO.File.WriteAllBytes(path, pixels);
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
}
