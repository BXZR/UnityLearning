using Magic.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Magic.GameEditor
{
#if UNITY_EDITOR
    class ConvertOperateWindow : EditorWindow
    {
        private static ConvertOperateWindow instance;

        [MenuItem("[Framework]/Net Message Convert", false, 99)]
        static void ShowWindow()
        {
            Rect rect = new Rect((Screen.currentResolution.width - 320) / 2, (Screen.currentResolution.height - 240) / 2, 450, 620);
            ConvertOperateWindow window = (ConvertOperateWindow)EditorWindow.GetWindowWithRect<ConvertOperateWindow>(rect, true, "NetMessageConvert", true);
            window.Show();

            instance = window;
        }


        public static void Refresh()
        {
            if (instance == null)
                return;
            instance.Close();
            ShowWindow();
        }

        Vector2 scrollPos = Vector2.zero;
        private List<NetMessageConverter> toDelete = new List<NetMessageConverter>();


        private void SavePlan()
        {
            try
            {
                BinaryFormatter binFormat = new BinaryFormatter();//创建二进制序列化器
                string initPath = GetInitPath();
                string path = DialogSaveFileName(initPath);
                if (File.Exists(path) == false)
                {
                    //EditorUtility.DisplayDialog("Log", "未选择文件", "known");
                    return;
                }
                Stream fStream = File.Create(path);
                binFormat.Serialize(fStream, NetMessageConvertPlan.Instance);
                fStream.Close();
            }
            catch (Exception E)
            {
                EditorUtility.DisplayDialog("Error", "保存方案失败，具体内容请看Log", "known");
                Debug.LogError("Save NetConvert Plan Error: \n" + E.ToString());
            }
        }


        private void LoadPlan()
        {
            try
            {
                BinaryFormatter binFormat = new BinaryFormatter();//创建二进制序列化器
                string initPath = GetInitPath();
                string path = DialogGetFileName(initPath);
                if (File.Exists(path) == false)
                {
                    //EditorUtility.DisplayDialog("Log", "未选择文件", "known");
                    return;
                }
                byte[] buffer = File.ReadAllBytes(path);
                using (MemoryStream mStream = new MemoryStream())
                {
                    mStream.Write(buffer, 0, buffer.Length);
                    mStream.Flush();
                    mStream.Seek(0, SeekOrigin.Begin);
                    NetMessageConvertPlan operateFromFile = (NetMessageConvertPlan)binFormat.Deserialize(mStream);
                    NetMessageConvertPlan.SetOperater(operateFromFile);
                }
            }
            catch (Exception E)
            {
                EditorUtility.DisplayDialog("Error", "加载方案失败，具体内容请看Log", "known");
                Debug.LogError("Load NetConvert Plan Error: \n"+ E.ToString());
            }
        }

        private string GetInitPath()
        {
            return Application.dataPath + "/../../";
        }

        public string DialogGetFileName(string initPath, string filter = "数据文件(.txt)| *.txt")
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.InitialDirectory = initPath;
            ofd.Filter = filter;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return ofd.FileName;
            }
            return initPath;
        }

        public string DialogSaveFileName(string initPath, string filter = "数据文件(.txt)| *.txt")
        {
            System.Windows.Forms.SaveFileDialog ofd = new System.Windows.Forms.SaveFileDialog();
            ofd.InitialDirectory = initPath;
            ofd.Filter = filter;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return ofd.FileName;
            }
            return initPath;
        }


        void OnGUI()
        {

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("加载方案"))
            {
                LoadPlan();
            }
            if (GUILayout.Button("保存方案"))
            {
                SavePlan();
            }
            if (GUILayout.Button("增加监听协议"))
            {
                AddConverterWindow.ShowWindow();
            }

            GUILayout.EndHorizontal();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(450), GUILayout.Height(570));

            foreach (var value in Enum.GetValues(typeof(NetMessageConvertType)))
            {
                List<NetMessageConverter> theConverters = NetMessageConvertPlan.Instance.GetConvertersWithType((NetMessageConvertType)value);

                for (int i = 0; theConverters != null && i < theConverters.Count; i++)
                {
                    NetMessageConverter theConverter = theConverters[i];

                    GUILayout.BeginHorizontal();
                    theConverter.isEnabled = GUILayout.Toggle(theConverter.isEnabled, "启用", GUILayout.Width(40));
                    GUILayout.Label(theConverter.GetInformation());
                    if (GUILayout.Button("编辑", GUILayout.Width(50)))
                    {
                        EditConverterWindow.ShowWindow(theConverter);
                    }
                    if (GUILayout.Button("删除", GUILayout.Width(50)))
                    {
                        toDelete.Add(theConverter);
                    }
                    GUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndScrollView();
            NetMessageConvertPlan.SaveMsgToFile = GUILayout.Toggle(NetMessageConvertPlan.SaveMsgToFile, "消息处理方案将收发的消息保存到文件");

            //循环显示之后再清理被删除的项
            if (toDelete.Count != 0)
            {
                for (int i = 0; i < toDelete.Count; i++)
                {
                    NetMessageConvertPlan.Instance.DeleteConverter(toDelete[i].convertType, toDelete[i]);
                }
                toDelete.Clear();
            }
        }
    }
#endif
}
