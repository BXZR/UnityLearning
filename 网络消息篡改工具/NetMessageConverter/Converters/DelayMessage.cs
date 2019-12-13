using Magic.Cougar;
using Magic.GameLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using XLua;
using LuaAPI = XLua.LuaDLL.Lua;
namespace Magic.GameEditor
{
#if UNITY_EDITOR
    [System.Serializable]
    class DelayMessage : NetMessageConverter
    {
        #region 界面绘制
        public override string GetLabelInformation()
        {
            return "延迟选定类型的消息";
        }

        public override void DrawParmGUI()
        {
            GUILayout.BeginVertical();

            GUI.skin.label.normal.textColor = new Color(0, 0, 255, 1.0f);
            GUILayout.Label(GetLabelInformation());
            GUI.skin.label.normal.textColor = new Color(0, 0, 0, 1.0f);

            GUILayout.Label("参数处理");
            isEnabled = GUILayout.Toggle(isEnabled, "加入方案后自动启用", GUILayout.Width(200));

            GUILayout.Space(10f);
            GUILayout.Label("延迟时间");
            this.delayTime = GUILayout.TextField(this.delayTime, GUILayout.Width(200));

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
        public override string GetLuaScript()
        {
            StringBuilder luaCode = new StringBuilder();
            luaCode.AppendLine("local protoBase =  require \"Net.Proto.Base.BaseProto\"");
            luaCode.AppendLine("local BSMsgMap = require \"Net.Config.BSMessageMap\"");
            luaCode.AppendLine("local pb = require \"pb\"");

            luaCode.AppendLine("function GetValueForConveter(msg)");
            luaCode.AppendLine("local msgName = BSMsgMap[BSMsgDefine." + this.messageType + "]");
            luaCode.AppendLine("local data = assert(pb.encode( msgName, msg))");
            luaCode.AppendLine("\nCS.Magic.GameEditor.NetMessageConvertPlan.Instance:SendFakeMessageToClient(BSMsgDefine." + this.messageType + ",data , false)");
            luaCode.AppendLine("end");
            return luaCode.ToString();
        }

        public override void DoConvert()
        {
            var netObj = NetMessageConvertPlan.Instance.GetCurrentConverterMessage();
            var type = NetMessageConvertPlan.Instance.GetCurrentMessageType();
            Magic.Framework.CoroutineRunner.Instance.StartCoroutine(Delay(netObj , type));
        }

        private Type ExType = null;
        private Type MsgType = null;

        IEnumerator Delay(ProtoBuf.IExtensible netObj, MessageType type )
        {
            float timer = (float)Convert.ToDouble(this.delayTime);
            yield return new WaitForSeconds(timer);

            //获取全局lua环境
            LuaEnv theEnv = XLuaManager.Instance.LuaEnv;
            string luaCode = GetLuaScript();
            theEnv.DoString(luaCode);

            LuaAPI.xlua_getglobal(theEnv.L, "GetValueForConveter");
            if (ExType == null)
            {
                ExType = GetTypeFromString(type.ToString() + "_Ex");
                MsgType = GetTypeFromString(type.ToString());
            }
            ExType.InvokeMember("PushToLua",
                System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public, null, null,
                new object[] { Convert.ChangeType(netObj, MsgType), theEnv.L }
              );

            //下面这个方法的性能会很好，但是调用会失效
            //Magic.GameLogic.ProtoMsgHandler.Instance.SendToLua(type, netObj);
            LuaAPI.lua_pcall(theEnv.L, 1, 0, 0);
        }

        #endregion
    }
#endif
}
