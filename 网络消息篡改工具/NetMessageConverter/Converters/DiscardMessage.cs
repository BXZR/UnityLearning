using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Magic.GameEditor
{
#if UNITY_EDITOR
    [System.Serializable]
    class DiscardMessage : NetMessageConverter
    {
        #region 界面绘制
        public override string GetLabelInformation()
        {
            return "丢弃选定类型的消息";
        }

        public override void DrawParmGUI()
        {
            GUILayout.BeginVertical();

            GUI.skin.label.normal.textColor = new Color(0, 0, 255, 1.0f);
            GUILayout.Label(GetLabelInformation());
            GUI.skin.label.normal.textColor = new Color(0, 0, 0, 1.0f);

            GUILayout.Label("参数处理");
            isEnabled = GUILayout.Toggle(isEnabled, "加入方案后自动启用", GUILayout.Width(200));
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
    }
#endif
}
