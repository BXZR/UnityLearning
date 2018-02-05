using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ABMAKER : MonoBehaviour {

	//这个脚本是asset bundle的示例脚本
	//用于生成一个新的选项并选择相应的预设物进行打包

	private const string thePath = @"E:/ABSETs/";

	[MenuItem ("AB/Create Asset Bundles")]
	static void  CreateAssetBundle()
	{
		BuildPipeline.BuildAssetBundles(thePath , BuildAssetBundleOptions.CollectDependencies , BuildTarget .StandaloneWindows64);
	}



}
