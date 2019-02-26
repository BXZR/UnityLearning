using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ABEditor : EditorWindow
{
    #region 变量
    //系统样式
    private GUIStyle _preButton = new GUIStyle();
    //打包路径
    private string _buildPath = @"E:\DemoAB";
    //_buildPath  = "Assets/AssetBundlesDemo";//默认的话是assets文件夹下面
    //_buildPath  = @"E:/DemoAB"; //给路径就可以存到指定位置了
    //资源存放路径
    private string _resourcesPath = "/AssetBundlesDemo";
    #endregion

    #region 初始化和绘制的总方法
    [MenuItem("ABEditor/AssetBundle Editor %#Z")]
    private static void OpenAssetBundleWindow()
    {
        ABEditor ABEditor = GetWindow<ABEditor>("ABTools");
        ABEditor._preButton = new GUIStyle();
        ABEditor._preButton.fontSize = 10;
        ABEditor._preButton.alignment = TextAnchor.UpperCenter;
        ABEditor.Show();
    }

    private void OnGUI()
    {
        DrawGUI();
    }
    
    private void DrawGUI()
    {
        GUI.Label(new Rect(5, 5, 70, 15), "Resources Path:");
        _resourcesPath = GUI.TextField(new Rect(80, 5, 300, 15), _resourcesPath);
        GUI.Label(new Rect(5, 25, 70, 15), "Build Path:");
        _buildPath = GUI.TextField(new Rect(80, 25, 300, 15), _buildPath);

        GUI.color = Color.yellow;
        GUI.backgroundColor = Color.yellow;

        if (GUI.Button(new Rect(5, 45, 80, 35), "Create"))
        {
            ButtonCreate();
        }

    }
    #endregion

    #region 方法细节
    /// <summary>
    /// create按钮按下方法
    /// </summary>
    private void ButtonCreate()
    {
        Debug.Log("start create");
        //检查路径
        CheckPath(_buildPath);
        //清理名字以保证只是保存指定目录的assetbund
        ClearAssetBundlesName();
        //设置指定路径下所有需要打包的assetbundlename
        SetAssetBundlesName(Application.dataPath + _resourcesPath);
        //建立AB包
        BuildPipeline.BuildAssetBundles(_buildPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }

    /// <summary>
    /// 路径检查
    /// </summary>
    /// <param name="_buildPath"></param>
    private void CheckPath(string _buildPath)
    {
        if (!Directory.Exists(_buildPath))
        {
            Directory.CreateDirectory(_buildPath);
        }

    }

    /// <summary>
    /// 清除所有的AssetBundleName，由于打包方法会将所有设置过AssetBundleName的资源打包，所以自动打包前需要清理
    /// </summary>
    static void ClearAssetBundlesName()
    {
        //获取所有的AssetBundle名称
        string[] abNames = AssetDatabase.GetAllAssetBundleNames();
        //强制删除所有AssetBundle名称
        for (int i = 0; i < abNames.Length; i++)
        {
            AssetDatabase.RemoveAssetBundleName(abNames[i], true);
        }
    }


    /// <summary>
    /// 设置所有在指定路径下的AssetBundleName
    /// </summary>
    static void SetAssetBundlesName(string _assetsPath)
    {
        //先获取指定路径下的所有Asset，包括子文件夹下的资源
        DirectoryInfo dir = new DirectoryInfo(_assetsPath);
        FileSystemInfo[] files = dir.GetFileSystemInfos(); //GetFileSystemInfos方法可以获取到指定目录下的所有文件以及子文件夹

        for (int i = 0; i < files.Length; i++)
        {
            if (files[i] is DirectoryInfo)  //如果是文件夹则递归处理
            {
                SetAssetBundlesName(files[i].FullName);
            }
            else if (!files[i].Name.EndsWith(".meta")) //如果是文件的话，则设置AssetBundleName，并排除掉.meta文件
            {
                SetABName(files[i].FullName);     //逐个设置AssetBundleName
            }
        }

    }

    /// <summary>
    /// 设置单个AssetBundle的Name
    /// </summary>
    /// <param name="filePath"></param>
    static void SetABName(string assetPath)
    {
        string importerPath = "Assets" + assetPath.Substring(Application.dataPath.Length);  //这个路径必须是以Assets开始的路径
        AssetImporter assetImporter = AssetImporter.GetAtPath(importerPath);  //得到Asset

        string tempName = assetPath.Substring(assetPath.LastIndexOf(@"\") + 1);
        string assetName = tempName.Remove(tempName.LastIndexOf(".")); //获取asset的文件名称
        assetImporter.assetBundleName = assetName;    //最终设置assetBundleName
    }
    #endregion

}



