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
    class ConverterScriptWindow : EditorWindow
    {
        private static ConverterScriptWindow instance;
        private NetMessageConverter theConverter;

        public static void ShowWindow(NetMessageConverter converterIn)
        {
            Rect rect = new Rect((Screen.currentResolution.width - 320) / 2, (Screen.currentResolution.height - 240) / 2, 700, 560);
            ConverterScriptWindow window = (ConverterScriptWindow)EditorWindow.GetWindowWithRect<ConverterScriptWindow>(rect, true, "ConvertScriptEdit", true);
            window.Init(converterIn);
            window.Show();
            instance = window;
        }

        ConverterProperties root = new ConverterProperties();
        Vector2 scrollPos = Vector2.zero;

        void OnGUI()
        {
            if (theConverter == null)
                return;
            GUILayout.Label( "["+ theConverter.messageType + " -- "+ theConverter.convertType + "]");
            GUILayout.Space(10);

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(690), GUILayout.Height(480));
            MakeDraw(root , 0);
            EditorGUILayout.EndScrollView();

            GUILayout.Space(10);
            if (GUILayout.Button("生成代码"))
            {
                innerScript = new StringBuilder();
                root.varribleName = "msg";
                MakeLuaInsertScript(root ,0);
                theConverter.insertedLuaCode = innerScript.ToString();
            }
        }

        public void Init(NetMessageConverter converterIn)
        {
            theConverter = converterIn;
            string typeName = this.theConverter.messageType.ToString();
            root = new ConverterProperties();
            GetProperties(typeName , root);
        }

        StringBuilder innerScript = new StringBuilder();

        private void MakeLuaInsertScript(ConverterProperties root , int depth , bool isArrayItem = false )
        {
            for (int i = 0; i < root.childs.Count; i++)
            {
                ConverterProperties aValue = root.childs[i];
                string nameSave = aValue.varribleName;

                if (aValue.propertieType == ConverterPropertieType.Primitive )
                {
                    aValue.varribleName = isArrayItem ? root.varribleName :  root.varribleName + "." + aValue.varribleName;
                    innerScript.AppendLine(GetTab(depth) + aValue.varribleName + " = " + aValue.valueString);
                }

                if (aValue.propertieType == ConverterPropertieType.String)
                {
                    aValue.varribleName = isArrayItem ? root.varribleName : root.varribleName + "." + aValue.varribleName;
                    innerScript.AppendLine(GetTab(depth) + aValue.varribleName + " =\" " + aValue.valueString + "\"");
                }

                else if (aValue.propertieType == ConverterPropertieType.Enum)
                {
                    aValue.varribleName = isArrayItem ? root.varribleName : root.varribleName + "." + aValue.varribleName;
                    innerScript.AppendLine(GetTab(depth) + aValue.varribleName + " = " + aValue.valueString);
                }
                else if (aValue.propertieType == ConverterPropertieType.Class && aValue.childs.Count != 0)
                {
                    aValue.varribleName = isArrayItem ? root.varribleName : root.varribleName + "." + aValue.varribleName;
                    innerScript.AppendLine(GetTab(depth) + aValue.varribleName + " = {}");
                    MakeLuaInsertScript(aValue, depth + 1);
                }
                else if (aValue.propertieType == ConverterPropertieType.List)
                {
                    aValue.varribleName = isArrayItem ? root.varribleName : root.varribleName + "." + aValue.varribleName;
                    innerScript.AppendLine(GetTab(depth) + aValue.varribleName + " = {}");
                    for (int j = 0; j < aValue.listChilds.Count; j++)
                    {
                        innerScript.AppendLine(GetTab(depth) + aValue.varribleName + "[" + (j + 1) + "]  = {}");
                        aValue.listChilds[j].varribleName = aValue.varribleName + "[" + (j + 1) + "] ";
                        MakeLuaInsertScript(aValue.listChilds[j], depth + 1);
                    }
                }
                else if (aValue.propertieType == ConverterPropertieType.Array)
                {
                    aValue.varribleName = root.varribleName + "." + aValue.varribleName;
                    innerScript.AppendLine(GetTab(depth) + aValue.varribleName + " = {}");
                    for (int j = 0; j < aValue.listChilds.Count; j++)
                    {
                        string childNameSave = aValue.listChilds[j].varribleName;

                        aValue.listChilds[j].varribleName = aValue.varribleName + "[" + (j + 1) + "] ";
                        MakeLuaInsertScript(aValue.listChilds[j], depth + 1 , true);

                        aValue.listChilds[j].varribleName = childNameSave;
                    }
                }

               aValue.varribleName = nameSave;
            }
        }

        private string GetTab(int depth)
        {
            string tab = "";
            for (int i = 0; i < depth; i++)
                tab += "  ";
            return tab;
        }


        //递归反射获取属性
        private void GetProperties(string typeName , ConverterProperties root)
        {
            Type msgType = theConverter.GetTypeFromString(typeName);
            //Debug.Log("type = " + msgType + " typeString = "+ typeName);
            if (msgType == null)
                return;

            if (msgType.IsPrimitive || msgType.IsEnum || typeName == "System.String")
            {
                ConverterProperties aValue = new ConverterProperties();
                aValue.type = msgType;
                aValue.typeString = typeName;
                aValue.varribleName = root.varribleName;
                OperateWithType(aValue, root);
            }
            else
            {
                foreach (System.Reflection.PropertyInfo p in msgType.GetProperties())
                {
                    ConverterProperties aValue = new ConverterProperties();
                    aValue.type = p.PropertyType;
                    aValue.typeString = p.PropertyType.ToString();
                    aValue.varribleName = p.Name;
                    OperateWithType(aValue, root);
                }
            }
        }

        //设定类型和更细分的递归获取子内容
        private void OperateWithType(ConverterProperties aValue, ConverterProperties root)
        {
            //基础类型不做处理，直接添加到上一层的child里面
            if (aValue.type.IsPrimitive)
            {
                aValue.propertieType = ConverterPropertieType.Primitive;
                aValue.valueString = Activator.CreateInstance(aValue.type).ToString().ToLower();
            }
            //如果是枚举，需要搞个选择
            else if (aValue.type.IsEnum)
            {
                aValue.propertieType = ConverterPropertieType.Enum;
            }
            //如果是数组则需要更细节处理
            //顺序要对，因为List也是一个Class类型
            else if (aValue.typeString.StartsWith("System.Collections.Generic.List"))
            {
                bool gate = aValue.typeString.Contains("[") && aValue.typeString.Contains("]");
                if (gate && aValue.type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    string childTypeName = aValue.typeString.Split('[')[1];
                    childTypeName = childTypeName.Split(']')[0];
                    aValue.propertieType = ConverterPropertieType.List;
                    GetProperties(childTypeName, aValue);
                }
            }
            //值类型的list
            else if (aValue.typeString.EndsWith("[]"))
            {
                string childTypeName = aValue.typeString.Split('[')[0];
                if (childTypeName.Contains("."))
                {
                    string [] typeNames = childTypeName.Split('.');
                    childTypeName = typeNames[typeNames.Length - 1];
                }
                aValue.propertieType = ConverterPropertieType.Array;
                GetProperties(childTypeName, aValue);
            }
            //如果是引用类型（并且不是string类型，这是因为stirng类型没有必要再细分了）
            else if (aValue.typeString != "System.String")
            {
                string childTypeName = aValue.typeString;
                aValue.propertieType = ConverterPropertieType.Class;
                GetProperties(childTypeName, aValue);
            }
            else if (aValue.typeString == "System.String")
            {
                aValue.propertieType = ConverterPropertieType.String;
                aValue.valueString = "";
            }
            root.childs.Add(aValue);
        }


        //绘制和初始化是两个方法
        //这个方法主管绘制
        private void MakeDraw(ConverterProperties root , int depth )
        {
            GUILayout.BeginVertical();
            for (int i = 0; i < root.childs.Count; i++)
            {
                ConverterProperties aValue = root.childs[i];
                GUILayout.BeginHorizontal();

                //处理缩进
                for (int j = 0; j < depth; j++)
                {
                    GUILayout.Label("" , GUILayout.Width(20));
                }

                if (aValue.propertieType == ConverterPropertieType.Primitive)
                {
                    GUILayout.Label(aValue.varribleName + ":", GUILayout.Width(80));
                    aValue.valueString = GUILayout.TextField(aValue.valueString, GUILayout.Width(250 - depth * 20));
                }
                else if (aValue.propertieType == ConverterPropertieType.String)
                {
                    GUILayout.Label(aValue.varribleName + ":", GUILayout.Width(80));
                    aValue.valueString = GUILayout.TextField(aValue.valueString, GUILayout.Width(250 - depth * 20));
                }
                else if (aValue.propertieType == ConverterPropertieType.Enum)
                {
                    List<string> enumNames = new List<string>();

                    foreach (int v in Enum.GetValues(aValue.type))
                    {
                        string strName = Enum.GetName(aValue.type, v);
                        enumNames.Add(strName);
                    }
                    GUILayout.Label(aValue.varribleName + ":", GUILayout.Width(80));
                    aValue.valueString = EditorGUILayout.Popup( Convert.ToInt32(aValue.valueString) , enumNames.ToArray(), GUILayout.Width(250-depth*20)).ToString();
                }
                else if (aValue.propertieType == ConverterPropertieType.Class && aValue.childs.Count != 0)
                {
                    GUILayout.Label(aValue.varribleName + ":", GUILayout.Width(80));
                    MakeDraw(aValue, depth + 1);
                }
                else if (aValue.propertieType == ConverterPropertieType.List || aValue.propertieType == ConverterPropertieType.Array)
                {
                    GUILayout.BeginVertical();
                    GUILayout.Label(aValue.varribleName + ":", GUILayout.Width(80));
                    int deleteIndex = -1;
                    for (int j = 0; j < aValue.listChilds.Count; j++)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("[" + j + "]", GUILayout.Width(40));
                        MakeDraw(aValue.listChilds[j], depth + 1);
                        if (GUILayout.Button("Delete" , GUILayout.Width(80)))
                        {
                            deleteIndex = j;
                        }
                        GUILayout.EndHorizontal();
                    }
                    //增加新项
                    if (GUILayout.Button("Add New " + aValue.varribleName + " Item", GUILayout.Width(400 - depth*30)))
                    {
                        ConverterProperties newListItem = new ConverterProperties();
                        for (int w = 0; w < aValue.childs.Count; w++)
                        {
                            newListItem.childs.Add(aValue.childs[w].GetClone());
                        }
                        aValue.listChilds.Add(newListItem);
                    }
                    //删除
                    if (deleteIndex >= 0)
                    {
                        aValue.listChilds.RemoveAt(deleteIndex);
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

    }


    class ConverterProperties
    {
        public Type type;
        public string typeString;
        public string varribleName;
        public string valueString;
        public ConverterPropertieType propertieType = ConverterPropertieType.None;
        //一般的引用类型用这个就可以提现所有的属性
        public List<ConverterProperties> childs = new List<ConverterProperties>();
        //数组是两层的属性保存，所有需要一个这样的结构
        public List<ConverterProperties> listChilds = new List<ConverterProperties>();

        public ConverterProperties GetClone()
        {
            ConverterProperties newItem = new ConverterProperties();
            newItem.type = this.type;
            newItem.typeString = this.typeString;
            newItem.varribleName = this.varribleName;
            newItem.valueString = this.valueString;
            newItem.propertieType = this.propertieType;

            newItem.childs = new List<ConverterProperties>();
            for (int i = 0; i < this.childs.Count; i++)
            {
                newItem.childs.Add(this.childs[i].GetClone());
            }

            newItem.listChilds = new List<ConverterProperties>();
            for (int i = 0; i < this.listChilds.Count; i++)
            {
                newItem.listChilds.Add(this.listChilds[i].GetClone());
            }
            return newItem;

        }
    }

    enum ConverterPropertieType
    {
        None, //未识别
        Primitive,//基本类型
        Enum,//枚举
        String,//字符串
        Class,//引用类型
        List,//引用数组类型
        Array,//值数组类型
    }
#endif
    }
