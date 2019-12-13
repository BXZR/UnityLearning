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
    class FakeMessage : NetMessageConverter
    {
        #region 界面绘制
        public override string GetLabelInformation()
        {
            return "模拟一个消息发送给客户端";
        }

        public override void DrawParmGUI()
        {
            GUILayout.BeginVertical();

            GUI.skin.label.normal.textColor = new Color(0, 0, 255, 1.0f);
            GUILayout.Label(GetLabelInformation());
            GUI.skin.label.normal.textColor = new Color(0, 0, 0, 1.0f);

            GUILayout.Label("参数处理");
            GUILayout.Space(10f);
            GUILayout.Label("附加的LUA代码");
            if (GUILayout.Button("根据proto内容生成初始LUA代码"))
            {
                ShowScriptEditWindow();
            }
            this.insertedLuaCode = GUILayout.TextArea(this.insertedLuaCode, GUILayout.Width(300), GUILayout.Height(400));
            GUILayout.EndVertical();

        }

        //绘制操作的内容
        public override void DrawOperateGUI()
        {
            GUILayout.Space(20);
            GUILayout.Label("操作");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("发送到客户端"))
            {
                this.DoConvert();
            }

            if (GUILayout.Button("关闭"))
            {
                AddConverterWindow.instance.CloseWindow();
            }
            GUILayout.EndHorizontal();
        }
        #endregion

        #region 操作
        public override string GetLuaScript()
        {
            StringBuilder luaCode = new StringBuilder();
            luaCode.AppendLine("local protoBase =  require \"Net.Proto.Base.BaseProto\"");
            luaCode.AppendLine("local BSMsgMap = require \"Net.Config.BSMessageMap\"");
            luaCode.AppendLine("local pb = require \"pb\"");
            luaCode.AppendLine("local msgName = BSMsgMap[BSMsgDefine." + this.messageType + "]");
            luaCode.AppendLine("\nlocal msg = {}");
            luaCode.AppendLine(this.insertedLuaCode);
            luaCode.AppendLine("local data = assert(pb.encode( msgName, msg))");
            luaCode.AppendLine("\nCS.Magic.GameEditor.NetMessageConvertPlan.Instance:SendFakeMessageToClient(BSMsgDefine." + this.messageType + ",data)");
            return luaCode.ToString();
        }
        #endregion
    }
#endif
}
