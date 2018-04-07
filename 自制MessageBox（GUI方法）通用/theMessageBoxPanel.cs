using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class theMessageBoxPanel : MonoBehaviour {

	//全局唯一
	public static theMessageBoxPanel theMessageSave;

	//做一个有点意境的消息框吧
	public Text theTitleText;
	public Text theInformationText;

	//GUI方法需要的参数
	//所有的东西都是按照屏幕百分比来计算
	private float widthForScreen = 0.5f;
	private float heightForScreen = 0.3f;
	private string stringForTitle = "";
	private string stringForInformation = "";
	//有一个变大的效果
	private float showPercent = 0.0f;
	private float showPercentAdder = 0.1f;
	//设定显示信息
	public void setInformation(string title = "", string information = "")
	{
		theTitleText.text = title;
		theInformationText.text = information;
		stringForTitle = title;
		stringForInformation = information;
	}

    //自我销毁
	public void makeEnd()
	{
		Destroy (this.gameObject);
	}


	void Start()
	{
		makeAlone ();
	}

	//消息框全局唯一
	void makeAlone()
	{
		if (theMessageSave != null)
			Destroy (theMessageSave.gameObject);
		theMessageSave = this;
		
	}

	//messageOnGUI的做法
	void OnGUI()
	{ 
		
		GUIStyle GUIShowStyle=new GUIStyle();
		GUIShowStyle.normal.textColor = Color.yellow;
		GUIShowStyle.fontStyle = FontStyle.Bold;
		GUIShowStyle.alignment = TextAnchor.MiddleCenter;
		GUIShowStyle.fontSize = 25;

		GUIStyle GUIShowStyle2=new GUIStyle();
		GUIShowStyle2.normal.textColor = Color.yellow;
		GUIShowStyle2.fontStyle = FontStyle.Bold;
		GUIShowStyle2.alignment = TextAnchor.UpperCenter;
		GUIShowStyle2.fontSize = 20;

		float startPointX = (1 - widthForScreen*showPercent) * Screen.width / 2;
		float startPointY = (1 - heightForScreen*showPercent) * Screen.height / 2;
		startPointY *= 0.4f;//稍微向上移动一点
		float width = widthForScreen * Screen.width*showPercent;
		float height = heightForScreen* Screen.width*showPercent;
		GUI.BeginGroup (new Rect (startPointX  ,startPointY , width, height));
		GUI.Box (new Rect (0, 0, width, height ), "" );//背景
		GUI.Box (new Rect (width/3, 0f, width/3, height/5 ), stringForTitle , GUIShowStyle);//标题
		GUI.Box (new Rect (width*0.05f, height/5 , width*0.9f, height*3/5 ), stringForInformation ,GUIShowStyle2 );//文本
		if (GUI.Button (new Rect (width*2 / 5, height * 4 / 5, width / 5, height / 8), "我已知晓"))
		{
			Destroy (this.gameObject);
		}
		GUI.EndGroup ();

		if(showPercent  < 1)
			showPercent += showPercentAdder;

	}


}
