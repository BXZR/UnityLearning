using Magic.Cougar;
using Magic.GameLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using XLua;

namespace Magic.GameEditor
{
#if UNITY_EDITOR
    #region 操作类型枚举
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class MemberStrAttribute : Attribute
    {
        public MemberStrAttribute(string str)
        {
            MemberStr = str;
        }

        public string MemberStr { get; private set; }
    }

    public static class EnumExtend
    {
        public static string MemberStr(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            MemberStrAttribute[] attrs =
                fi.GetCustomAttributes(typeof(MemberStrAttribute), false) as MemberStrAttribute[];
            if (attrs.Length > 0) return attrs[0].MemberStr;

            return value.ToString();
        }
    }

    [System.Serializable]
    public  enum NetMessageConvertType
    {
        [MemberStr("什么都不做")]
        none,//什么都没有
        [MemberStr("丢弃消息")]
        discard,//丢弃消息
        [MemberStr("篡改消息")]
        distort,//篡改消息
        [MemberStr("自动模拟返回")]
        autofake,//自动模拟返回
        [MemberStr("延迟消息")]
        delay,//延迟获取
        [MemberStr("模拟获取")]
        fake,//模拟获取
        [MemberStr("模拟发送")]
        send,//模拟发送
    }
    #endregion

    //这个类是对收到网络消息之后进行额外后处理的操作的基类
    //这个功能仅限于编辑器下
    [System.Serializable]
    public class NetMessageConverter
    {
        //处理的消息的type(unshort),对应消息类型的枚举，这里还有一些对应关系需要处理
        public Cougar.MessageType messageType = Cougar.MessageType.MsgC2GAddActor;
        //处理类型
        public NetMessageConvertType convertType = NetMessageConvertType.none;
        //用于检索的label
        public string label = "Normal";
        //是否进行操作
        public bool isEnabled = true;
        //额外的lua代码
        public string insertedLuaCode = "print(\"heihei\")";
        //延迟时间
        public string delayTime = "5";

        //筛选的时候的Filter字符串
        public static  string protoFilter = "";

        //收到的消息内容
        ProtoBuf.IExtensible netObjRecieved = null;

        //子操作
        public List<NetMessageConverter> childConverters = new List<NetMessageConverter>();

        #region 基础的界面绘制
        [NonSerialized]
        Vector2 scrollPos = Vector2.zero;

        //绘制选择消息的GUI（很长的一个滑动框）
        public virtual void DrawSelectMessageGUI()
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal(GUILayout.Width(240));
            GUILayout.Label("消息类型");
            protoFilter = GUILayout.TextField(protoFilter);
            GUILayout.EndHorizontal();
            //所有的消息类型
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(260), GUILayout.Height(500));
            foreach (var value in Enum.GetValues(typeof(Magic.Cougar.MessageType)))
            {
                string strName = Enum.GetName(typeof(Magic.Cougar.MessageType), value);
                if (strName.ToLower().Contains(protoFilter.ToLower()) == false)
                    continue;

                //仅仅是表现效果-----------------------------------------------------
                if (this.messageType == (Magic.Cougar.MessageType)value)
                {
                    GUI.color = Color.yellow;
                }

                if (GUILayout.Button(strName, GUILayout.MaxWidth(240)))
                {
                    this.messageType = (Magic.Cougar.MessageType)value;
                }

                //仅仅是表现效果-----------------------------------------------------
                if (this.messageType == (Magic.Cougar.MessageType)value)
                {
                    GUI.color = Color.white;
                }
            }
            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        //绘制参数GUI内容
        public virtual void DrawParmGUI()
        {
            GUILayout.Label("没有参数");
        }

        public virtual string GetLabelInformation()
        {
            return "基础操作";
        }

        public  NetMessageConverter MakeNewConverter(NetMessageConvertType theType)
        {
            NetMessageConverter theConverterForSave = new NetMessageConverter();
            switch (theType)
            {
                case NetMessageConvertType.discard:
                    theConverterForSave = new DiscardMessage(); break;
                case NetMessageConvertType.distort:
                    theConverterForSave = new DistortMessage(); break;
                case NetMessageConvertType.delay:
                    theConverterForSave = new DelayMessage(); break;
                case NetMessageConvertType.fake:
                    theConverterForSave = new FakeMessage(); break;
                case NetMessageConvertType.send:
                    theConverterForSave = new SendMessge(); break;
                case NetMessageConvertType.autofake:
                    theConverterForSave = new AutoFakeMessage();break;
                default:
                    theConverterForSave = new NetMessageConverter(); break;
            }
            theConverterForSave.convertType = theType;
            theConverterForSave.messageType = this.messageType;
            theConverterForSave.isEnabled = this.isEnabled;
            theConverterForSave.insertedLuaCode = this.insertedLuaCode;
            for (int i = 0; i < this.childConverters.Count; i++)
            {
                theConverterForSave.childConverters.Add(this.childConverters[i]);
            }
            return theConverterForSave;
        }

        public virtual void CopyConfig(NetMessageConverter converterIn)
        {
            this.isEnabled = converterIn.isEnabled;
            this.insertedLuaCode = converterIn.insertedLuaCode;
            this.childConverters = converterIn.childConverters;
        }

        //绘制操作的内容
        public virtual void DrawOperateGUI()
        {
            GUILayout.Space(20);
            if (GUILayout.Button("关闭"))
            {
                AddConverterWindow.instance.CloseWindow();
            }
        }

        public void ShowScriptEditWindow()
        {
            ConverterScriptWindow.ShowWindow(this);
        }

        #endregion

        //具体的操作内容
        //在加入到消息队列之前做一些事情
        public void OnRecieveMessage(ushort type, ProtoBuf.IExtensible netObj)
        {
            netObjRecieved = netObj;
        }


        //获取这个消息
        public ProtoBuf.IExtensible GetMessageRecieved()
        {
            return netObjRecieved;
        }

        public string GetInformation()
        {
            string theInformation = "";
            theInformation += messageType + "("+ ((short)messageType).ToString() + ")[" + convertType.MemberStr() + "]";
            return theInformation;
        }

        //执行篡改方法
        public virtual void DoConvert()
        {
            //获取全局lua环境
            LuaEnv theEnv = XLuaManager.Instance.LuaEnv;
            string luaCode = GetLuaScript();
            theEnv.DoString(luaCode);
        }


        //获取根据规则设定好的lua代码
        public virtual string GetLuaScript()
        {
            return "";
        }

        public Type GetTypeFromString(string typeName)
        {
            Type type = null;
            Assembly[] assemblyArray = AppDomain.CurrentDomain.GetAssemblies();
            int assemblyArrayLength = assemblyArray.Length;
            for (int i = 0; i < assemblyArrayLength; ++i)
            {
                type = assemblyArray[i].GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }

            for (int i = 0; (i < assemblyArrayLength); ++i)
            {
                Type[] typeArray = assemblyArray[i].GetTypes();
                int typeArrayLength = typeArray.Length;
                for (int j = 0; j < typeArrayLength; ++j)
                {
                    if (typeArray[j].Name.Equals(typeName))
                    {
                        return typeArray[j];
                    }
                }
            }
            return type;
        }
    }
#endif
}
