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
    class EditConverterWindow : EditorWindow
    {
        private static EditConverterWindow instance;
        private NetMessageConverter theConverter;
        private NetMessageConverter theEditConverter;
        public static void ShowWindow(NetMessageConverter theConverterIn)
        {
            Rect rect = new Rect((Screen.currentResolution.width - 320) / 2, (Screen.currentResolution.height - 240) / 2, 310, 540);
            EditConverterWindow window = (EditConverterWindow)EditorWindow.GetWindowWithRect<EditConverterWindow>(rect, true, "NetMessageConvert Edit", true);
            window.Show();
            window.theConverter = theConverterIn;
            window.theEditConverter = theConverterIn.MakeNewConverter(theConverterIn.convertType);
            instance = window;
        }

        void OnGUI()
        {
            theEditConverter.DrawParmGUI();
            if (GUILayout.Button("Save"))
            {
                theConverter.CopyConfig(theEditConverter);
            }
        }
    }
#endif
}
