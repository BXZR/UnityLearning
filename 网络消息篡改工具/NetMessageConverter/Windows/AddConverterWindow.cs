using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Magic.GameEditor
{

#if UNITY_EDITOR
    class AddConverterWindow : EditorWindow
    {

        public static AddConverterWindow instance = null;

        public static void ShowWindow()
        {
            Rect rect = new Rect((Screen.currentResolution.width - 320) / 2, (Screen.currentResolution.height - 240) / 2, 600, 645);
            AddConverterWindow window = (AddConverterWindow)EditorWindow.GetWindowWithRect<AddConverterWindow>(rect, true, "AddConverter", true);
            window.Show();
            AddConverterWindow.theConverterForEdit = new NetMessageConverter();

            instance = window;
        }


        public void CloseWindow()
        {
            this.Close();
        }


        Vector2 scrollPos2 = Vector2.zero;
        static NetMessageConverter theConverterForEdit  = new NetMessageConverter();

        void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("操作类型");
            //所有的操作类型
            //scrollPos2 = EditorGUILayout.BeginScrollView(scrollPos2, GUILayout.Width(200), GUILayout.Height(120));
            foreach (var value in Enum.GetValues(typeof(NetMessageConvertType)))
            {
                string strName = ((NetMessageConvertType)value).MemberStr();
                //仅仅是表现效果-----------------------------------------------------
                if (theConverterForEdit.convertType == (NetMessageConvertType)value)
                {
                    GUI.color = Color.yellow;
                }

                if (GUILayout.Button(strName, GUILayout.MaxWidth(190)))
                {
                    Debug.Log("-->" + value);
                    theConverterForEdit = theConverterForEdit.MakeNewConverter((NetMessageConvertType)value);
                }

                //仅仅是表现效果-----------------------------------------------------
                if (theConverterForEdit.convertType == (NetMessageConvertType)value)
                {
                    GUI.color = Color.white;
                }
            }
            //EditorGUILayout.EndScrollView();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            //最外层的横向布局------------------------------------------------------------------------//
            theConverterForEdit.DrawSelectMessageGUI();
            theConverterForEdit.DrawParmGUI();
            //最外层的横向布局------------------------------------------------------------------------//
            GUILayout.EndHorizontal();
            theConverterForEdit.DrawOperateGUI();
        }
    }
#endif
}
