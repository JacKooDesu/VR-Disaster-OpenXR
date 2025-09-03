using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// 使用泛型存檔
public static class FileManager<T>
{
    public static string Save(string fileName, T target, string dir)
    {
#if UNITY_EDITOR
        dir.Insert(0, ".");
#endif
        var jsonData = JsonUtility.ToJson(target, true);    // 物件序列化json字串
        var filePath = $"{Application.dataPath}/{dir}";     // 取得資料夾路徑

        try
        {
            Directory.CreateDirectory(filePath);        // 確認資料夾
            File.WriteAllText($"{filePath}/{fileName}.sav", jsonData);  // json寫入
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error saving file: {e.Message}");
        }

        return jsonData;
    }

    public static void Load(string path, string name, T target)
    {
#if UNITY_EDITOR
        path.Insert(0, ".");
#endif
        var filePath = $"{Application.dataPath}/{path}/{name}.sav";
        var deserializeData = (string)(null);   // json 解譯字串

        try
        {
            deserializeData = File.ReadAllText(filePath);
        }
        catch (System.IO.FileNotFoundException)
        {
            Debug.Log("FileNotFoundException");
            return;
        }
        catch (System.IO.DirectoryNotFoundException)
        {
            Debug.Log("DirectoryNotFoundException");
            return;
        }

        JsonUtility.FromJsonOverwrite(deserializeData, target);     // 複寫target
    }
}