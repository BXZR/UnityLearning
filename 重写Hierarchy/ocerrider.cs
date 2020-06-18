using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ocerrider  
{
    [InitializeOnLoadMethod]
    static void RegisterDrawCallBack()
    {
        EditorApplication.hierarchyWindowItemOnGUI = (SelectionId, SelectionRect) =>
        {
            // 文字颜色定义 
            var textColor = Color.green;
            GUIStyle style = null;
            Vector2 offset = Vector2.one;

            GameObject go = EditorUtility.InstanceIDToObject(SelectionId) as GameObject;
            if (go == null) return;

            var com = go.GetComponent<Text>();
            if (com != null && com.enabled)
            {
                style = LabelStyle(textColor);
            }

            Rect offsetRect = new Rect(SelectionRect.position + offset, SelectionRect.size);
            // 绘制Label来覆盖原有的名字
            if (style != null && go.activeInHierarchy)
            {
                EditorGUI.DrawRect(SelectionRect, Color.gray);
                EditorGUI.LabelField(offsetRect, go.name, style);
            }
        };
    }

    static GUIStyle LabelStyle(Color textColor)
    {
        GUIStyle theStyle = new GUIStyle();
        theStyle.normal.textColor = textColor;
        return theStyle;
    }
}
