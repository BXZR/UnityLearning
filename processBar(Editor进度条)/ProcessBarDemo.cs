using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ProcessBarDemo : EditorWindow
{

    [MenuItem("Tools/进度条")]
    private static void ShowSlider()
    {
        int index = 0;
        int total = 500;
        EditorApplication.update = delegate ()
        {
            bool isCancle = EditorUtility.DisplayCancelableProgressBar("执行进度条示例中...", "正在走进度条", (float)index / total);
            ++index;
            if (isCancle || index >= total)
            {
                EditorUtility.ClearProgressBar();
                EditorApplication.update = null;
            }
        };
    }
}
