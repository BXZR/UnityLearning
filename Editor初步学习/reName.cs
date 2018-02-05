using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;//作为编辑器必须引用的名字空间


//继承这个类已经标记了这个类不是游戏对象而是编辑器

public class reName :ScriptableWizard {

	//公有属性会被显示在面板上
	public string theBasicName = "Suck_";

	//定制在选项面板中的位置
	[MenuItem("Suck/ SuckName")]
	static void createWizard()
	{
		//参数的含义分别是：
		//窗口title,定制需要使用的脚本类型，提交按钮的字符串
		ScriptableWizard.DisplayWizard ("suck name operater",typeof (reName),"let's suck names");
	}

	void OnEnable()
	{
		//因为这段代码的位置，这个显示的字符串是不修改的，出现窗口后修改选择也不会变的
		//修改的提示信息，应该是一个public父类的值，修改之后显示
		helpString = "";
		//Selection.objects 为当前选择的GameObject的集合
		if (Selection.objects != null)
			helpString = "Items selected count: "+Selection.objects.Length ;
	}

	void OnWizardCreate()
	{
		if (Selection.objects == null)
			return;
		//正常按照集合的使用方法处理就行
		for (int i = 0; i < Selection.objects.Length; i++)
			Selection.objects [i].name = theBasicName + i;
	}

}
