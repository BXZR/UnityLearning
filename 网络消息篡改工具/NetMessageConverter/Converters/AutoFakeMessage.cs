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
    //根据客户端发送的消息直接模拟返回
    [System.Serializable]
    class AutoFakeMessage : NetMessageConverter
    {
        #region 界面绘制
        public override string GetLabelInformation()
        {
            return "当接收一个指定的消息的时候向客户端发送消息";
        }

        [NonSerialized]
        Vector2 scrollPosForSelect = Vector2.zero;
        [NonSerialized]
        Vector2 scrollPosForChilds = Vector2.zero;
        int deleteIndex = -1;

        string aimTypeFilter = "";
        string childTypeFilter = "";

        public override void DrawParmGUI()
        {
            deleteIndex = -1;

            GUILayout.BeginVertical();

            GUI.skin.label.normal.textColor = new Color(0, 0, 255, 1.0f);
            GUILayout.Label(GetLabelInformation());
            GUI.skin.label.normal.textColor = new Color(0, 0, 0, 1.0f);

            //选择消息
            GUILayout.BeginHorizontal(GUILayout.Width(300));
            GUILayout.Label("消息类型");
            aimTypeFilter = GUILayout.TextField(aimTypeFilter, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            //所有的消息类型
            scrollPosForSelect = EditorGUILayout.BeginScrollView(scrollPosForSelect, GUILayout.Width(300), GUILayout.Height(200));
            foreach (var value in Enum.GetValues(typeof(Magic.Cougar.MessageType)))
            {
                string strName = Enum.GetName(typeof(Magic.Cougar.MessageType), value) + "("+ (ushort)value +")";
                if (strName.ToLower().Contains(aimTypeFilter.ToLower()) == false)
                    continue;

                GUILayout.BeginHorizontal();
                GUILayout.Label(strName, GUILayout.MaxWidth(240));
                if (GUILayout.Button("添加"))
                {
                    childConverterForShow.messageType = (Magic.Cougar.MessageType)value;
                    childConverterForShow.convertType = NetMessageConvertType.fake;
                    childConverters.Add(childConverterForShow.MakeNewConverter(childConverterForShow.convertType));
                }
                GUILayout.EndHorizontal();

            }
            EditorGUILayout.EndScrollView();

            //子操作列表
            //所有的消息类型
            GUILayout.Space(20);
            GUILayout.BeginHorizontal(GUILayout.Width(300));
            GUILayout.Label("已经选择的消息");
            childTypeFilter = GUILayout.TextField(childTypeFilter , GUILayout.Width(200));
            GUILayout.EndHorizontal();

            scrollPosForChilds = EditorGUILayout.BeginScrollView(scrollPosForChilds, GUILayout.Width(300), GUILayout.Height(200));
            for (int i = 0; i < childConverters.Count; i++)
            {
                string filterMask = childConverters[i].messageType.ToString().ToLower();
                if (filterMask.Contains(childTypeFilter.ToLower()) == false)
                    continue;

                GUILayout.BeginHorizontal();
                string strName = childConverters[i].messageType.ToString()  +"(" + ((ushort)childConverters[i].messageType) + ")"; 
                GUILayout.Label(strName, GUILayout.MaxWidth(220));

                if (GUILayout.Button("设置"))
                {
                    EditConverterWindow.ShowWindow(childConverters[i]);
                }
                if (GUILayout.Button("删除"))
                {
                    deleteIndex = i;
                }
                GUILayout.EndHorizontal();

            }
            EditorGUILayout.EndScrollView();

            //删除处理
            if (deleteIndex >= 0 && deleteIndex < childConverters.Count)
            {
                childConverters.RemoveAt(deleteIndex);
            }
            GUILayout.EndVertical();

        }

        //绘制操作的内容
        public override void DrawOperateGUI()
        {
            GUILayout.Space(20);
            GUILayout.Label("操作");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("加入到监听方案"))
            {
                NetMessageConverter theConverterForSave = MakeNewConverter(this.convertType);
                NetMessageConvertPlan.Instance.AddConverter(theConverterForSave.convertType, theConverterForSave);
                ConvertOperateWindow.Refresh();
            }

            if (GUILayout.Button("关闭"))
            {
                AddConverterWindow.instance.CloseWindow();
            }
            GUILayout.EndHorizontal();
        }

        #endregion

        #region 操作
        private NetMessageConverter childConverterForShow = new NetMessageConverter();
        public override void DoConvert()
        {
            for (int i = 0; i < childConverters.Count; i++)
            {
                childConverters[i].DoConvert();
            }
        }

        #endregion
    }
#endif
}
