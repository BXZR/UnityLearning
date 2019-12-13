using Magic.Cougar;
using Magic.GameLogic;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using XLua;
using LuaAPI = XLua.LuaDLL.Lua;

namespace Magic.GameEditor
{

    internal class DeserializedData
    {
        internal ushort type;
        internal ProtoBuf.IExtensible netObj;
    }

    internal class TypeObj
    {
       public Type msgType;
       public Type exType;
    }

    //这个类用于操作所有的NetMessageConverter
    //在界面上显示的也是这个类的内容
    [System.Serializable]
    public class NetMessageConvertPlan
    {
        private static NetMessageConvertPlan instance = null;
        public static NetMessageConvertPlan Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new NetMessageConvertPlan();
                    instance.MakeStart();
                }
                return instance;
            }
        }

        public static  void SetOperater(NetMessageConvertPlan operaterIn )
        {
            instance = operaterIn;
        }


        public void MakeStart()
        {
            Magic.Framework.CoroutineRunner.Instance.StartCoroutine(SaveOperate());
        }


        #region 消息记录

        //是否真的记录信息到文件，这个功能
        [NonSerialized]
        public static bool SaveMsgToFile = false;

        [NonSerialized]
        Queue<DeserializedData> dataQueue = new Queue<DeserializedData>();
        [NonSerialized]
        Dictionary<string, TypeObj> typeBuff = new Dictionary<string, TypeObj>();
        [NonSerialized]
        WaitForEndOfFrame waitForSave = new WaitForEndOfFrame();

        //记录消息
        public void RecordMessage(ushort type, ProtoBuf.IExtensible netObj)
        {
            if (!SaveMsgToFile)
                return;

            DeserializedData newData = new DeserializedData();
            newData.type = type;
            newData.netObj = netObj;
            dataQueue.Enqueue(newData);
            //LuaSaveMessage(newData.type, newData.netObj);
        }

        IEnumerator  SaveOperate()  
        {
            while (true)
            {
                yield return waitForSave;

                for (int i = 0; i < 5; i++)
                {
                    if (dataQueue.Count == 0)
                        continue;

                    DeserializedData newData = null;
                    newData = dataQueue.Dequeue();
                    if (newData != null)
                    {
                        LuaSaveMessage(newData.type, newData.netObj);
                    }
                }
            }
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

        public void LuaSaveMessage(ushort type, ProtoBuf.IExtensible netObj)
        {
            try
            {
                string theMessageTypeName = Enum.GetName(typeof(MessageType), type);
                if (string.IsNullOrEmpty(theMessageTypeName))
                    return;

                Type theType = null;
                Type theExType = null;
                if (typeBuff.Keys.Contains(theMessageTypeName))
                {
                    theType = typeBuff[theMessageTypeName].msgType;
                    theExType = typeBuff[theMessageTypeName].exType;
                }
                else
                {
                    theType = GetTypeFromString(theMessageTypeName);
                    theExType = GetTypeFromString(theMessageTypeName + "_Ex");

                    TypeObj newObj = new TypeObj();
                    newObj.msgType = theType;
                    newObj.exType = theExType;
                    typeBuff.Add(theMessageTypeName, newObj);
                }

                if (theType == null || theExType == null)
                {
                    Debug.Log("no type or extype for " + theMessageTypeName);
                    return;
                }
                    
                var parmObj = Convert.ChangeType(netObj, theType);
                if (parmObj == null)
                {
                    Debug.Log("no net obj for " + theMessageTypeName);
                    return;
                }

                StringBuilder luaCode = new StringBuilder();
                luaCode.AppendLine("local protoBase =  require \"Net.Proto.Base.BaseProto\"");
                luaCode.AppendLine("function SaveMsg( msg , path , msgName )");
                luaCode.AppendLine("protoBase.SaveTable(protoBase , msg , path , msgName)");
                luaCode.AppendLine("end");
                LuaEnv theEnv = XLuaManager.Instance.LuaEnv;
                theEnv.DoString(luaCode.ToString());
                LuaAPI.xlua_getglobal(theEnv.L, "SaveMsg");

                theExType.InvokeMember("PushToLua",
                  System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public, null, null,
                  new object[] { parmObj, theEnv.L });

                LuaAPI.lua_pushstring(theEnv.L , GetrFileName());
                LuaAPI.lua_pushstring(theEnv.L, theMessageTypeName);
                LuaAPI.lua_pcall(theEnv.L ,3, 0, 0);
                //Debug.Log("save " + theMessageTypeName + "over");
            }
            catch (Exception E)
            {
                Debug.LogError(E.ToString());
            }
        }

        //存文件
        public string GetrFileName()
        {
            string userId = GameBattle.MapSwitchManager.Instance.playerId;
            userId = string.IsNullOrEmpty(userId) ? "Default" : userId;

            string folderPath = Application.persistentDataPath + "/" + userId;

            if (Directory.Exists(folderPath) == false)
                Directory.CreateDirectory(folderPath);
            string fileName = folderPath + "/NetMessageLog.txt";
            if (File.Exists(fileName) == false)
            {
                File.Create(fileName);
            }
            return fileName;
        }


        #endregion

#if UNITY_EDITOR
        private NetMessageConverter currentConverter = new NetMessageConverter();

        #region 消息篡改等操作
        //分类型记录converter的字典
        private Dictionary<NetMessageConvertType, List<NetMessageConverter>> recieveConverterDic = new Dictionary<NetMessageConvertType, List<NetMessageConverter>>();

        public void AddConverter(NetMessageConvertType type, NetMessageConverter theConverter)
        {
            if (recieveConverterDic.Keys.Contains(type))
            {
                theConverter.convertType = type;
                recieveConverterDic[type].Add(theConverter);
            }
            else
            {
                List<NetMessageConverter> converters = new List<NetMessageConverter>();
                theConverter.convertType = type;

                converters.Add(theConverter);
                recieveConverterDic.Add(type, converters);
            }
        }

        public void DeleteConverter(NetMessageConvertType type, NetMessageConverter theConverter)
        {
            if (recieveConverterDic.Keys.Contains(type) == false)
                return;
            if (recieveConverterDic[type].Contains(theConverter) == false)
                return;

            recieveConverterDic[type].Remove(theConverter);
        }

        public List<NetMessageConverter> GetConvertersWithType(NetMessageConvertType type)
        {
            if (recieveConverterDic.Keys.Contains(type) == false)
                return null;

            return recieveConverterDic[type];
        }

        //发送消息
        public bool OnSendMessage(ushort type)
        {
            if (recieveConverterDic.Keys.Contains(NetMessageConvertType.autofake) == false)
                return false;

            List<NetMessageConverter> theList = recieveConverterDic[NetMessageConvertType.autofake];
            for (int i = 0; i < theList.Count; i++)
            {
                NetMessageConverter theConverer = theList[i];
                if ((ushort)theConverer.messageType == type && theConverer.isEnabled)
                {
                    //操作
                    theConverer.DoConvert();
                    return true;
                }
            }
            return false;
        }


        //收到消息
        public bool OnReceiveMessage(ushort type, ProtoBuf.IExtensible netObj)
        {
            //记录
            RecordMessage(type, netObj);
            //第一部分，判断是不是要拦截
            if (recieveConverterDic.Keys.Contains(NetMessageConvertType.discard))
            {
                List<NetMessageConverter> theList = recieveConverterDic[NetMessageConvertType.discard];
                for (int i = 0; i < theList.Count; i++)
                {
                    NetMessageConverter theConverer = theList[i];
                    if ((ushort)theConverer.messageType == type && theConverer.isEnabled)
                        return true;
                }
            }
            //如果不是拦截就进行篡改/延迟/模拟
            bool isConvertedRecieve = false;
            foreach (KeyValuePair<NetMessageConvertType, List<NetMessageConverter>> kvp in recieveConverterDic)
            {
                List<NetMessageConverter> convertList = kvp.Value;
                for (int i = 0; i < convertList.Count; i++)
                {
                    if (convertList[i].isEnabled == false)
                        continue;

                    if ((ushort)convertList[i].messageType == type)
                    {
                        currentConverter = convertList[i];
                        convertList[i].OnRecieveMessage(type, netObj);
                        convertList[i].DoConvert();
                        isConvertedRecieve = true;
                    }
                }
            }
            return isConvertedRecieve;
        }
        #endregion

        #region 基础方法
        //拦截操作，判断一下这个类型的消息有没有被记录在被拦截的消息里面
        //返回的是是否被拦截
        private bool IsDiscardMessage(ushort type, ProtoBuf.IExtensible netObj)
        {
            if (recieveConverterDic.Keys.Contains(NetMessageConvertType.discard) == false)
                return false;

            for(int i = 0; i < recieveConverterDic[NetMessageConvertType.discard].Count; i ++ )
            {
                NetMessageConverter theConverer = recieveConverterDic[NetMessageConvertType.discard][i];
                if ((ushort)theConverer.messageType == type && theConverer.isEnabled)
                    return true;
            }
            return false;
        }


        //最下层的cougar的调用,不经过onsync直接
        public void SendFakeMessageToClient(ushort msgType, byte[] protoMsg , bool useConvert = true)
        {
            if (protoMsg == null)
                UnityEngine.Debug.Log("空消息："+ msgType);

            var netObj = MessageParse.ParseMsgWithoutHead(protoMsg, protoMsg.Length, msgType);
            var newCougar = Magic.GameLogic.StandaloneGameModule.Instance.m_cougar as CNewCougar;
            newCougar.ExecuteMsg(msgType, netObj , useConvert);
        }

        //模拟一个消息发给服务端
        public void SendFakeMessageToServer(ushort msgType, byte[] protoMsg)
        {
            Magic.GameLogic.BSNetModule.Instance.bsMsgCenter.SendProtoMsg(msgType, protoMsg);
        }

        public ProtoBuf.IExtensible GetCurrentConverterMessage()
        {
            if (currentConverter != null)
                return currentConverter.GetMessageRecieved();
            return null;
        }

        public MessageType GetCurrentMessageType()
        {
            if (currentConverter != null)
                return currentConverter.messageType;
            return MessageType.MsgC2GCollect;
        }
        #endregion
#endif

    }

}
