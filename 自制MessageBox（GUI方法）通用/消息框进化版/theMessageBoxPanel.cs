using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class theMessageBoxPanel : MonoBehaviour {

	//全局唯一
	public static theMessageBoxPanel theMessageSave;

	//做一个有点意境的消息框吧
	//OnGUI模式之下这两个无效
	public Text theTitleText;
	public Text theInformationText;
	//一个背景图
	public Texture2D theBackPicture;

	//GUI方法需要的参数
	//所有的东西都是按照屏幕百分比来计算
	private float widthForScreen = 0.35f;
	private float heightForScreen = 0.2f;
	private string stringForTitle = "";
	private string stringForInformation = "";

	//有一个缩放的效果
	private bool isClosing = false;//开始还是关闭
	private float showPercent = 0.0f;
	private float showPercentAdder = 0.1f;

	//额外的定时显示
	private bool withTimer = false;
	private float timer = 15f;

	//GUI 额外设定
	GUIStyle GUIShowStyleForTitle;
	GUIStyle GUIShowStyleForInformation;
	GUIStyle GUIShowStyleForBack;

	//设定显示信息
	public void setInformation(string title = "", string information = "" )
	{
		theTitleText.text = title;
		theInformationText.text = information;
		stringForTitle = title;
		stringForInformation = information;
	}
	//设定显示时间
	//不设定就是不用计时器
	public void setTimer ( float timerIn)
	{
		timer = timerIn;
		withTimer = true;
	}
	//自我销毁
	public void makeEnd()
	{
		isClosing = true;
		Destroy (this.gameObject , 0.5f);
	}
		
	//消息框全局唯一
	private void makeAlone()
	{
		if (theMessageSave != null)
			Destroy (theMessageSave.gameObject);
		theMessageSave = this;
		
	}
	//初始化
	private  void makeStart()
	{
		GUIShowStyleForTitle =new GUIStyle();
		GUIShowStyleForTitle.normal.textColor = Color.yellow;
		GUIShowStyleForTitle.fontStyle = FontStyle.Bold;
		GUIShowStyleForTitle.alignment = TextAnchor.MiddleCenter;
		GUIShowStyleForTitle.fontSize = 18;

		GUIShowStyleForInformation=new GUIStyle();
		GUIShowStyleForInformation.normal.textColor = Color.yellow;
		GUIShowStyleForInformation.fontStyle = FontStyle.Bold;
		GUIShowStyleForInformation.alignment = TextAnchor.UpperCenter;
		GUIShowStyleForInformation.fontSize = 15;

		GUIShowStyleForBack = new GUIStyle ();
		GUIShowStyleForBack.normal.background = theBackPicture;
	}

	void Start()
	{
		makeAlone ();
		makeStart ();
	}

	void Update()
	{
		if (withTimer) 
		{
			timer -= Time.deltaTime;
			if (timer < 0)
				makeEnd();
		}
	}

	//messageOnGUI的做法
	void OnGUI()
	{ 
		
		float startPointX = (1 - widthForScreen*showPercent) * Screen.width / 2;
		float startPointY = (1 - heightForScreen*showPercent) * Screen.height / 2;
		startPointY *= 0.4f;//稍微向上移动一点
		float width = widthForScreen * Screen.width*showPercent;
		float height = heightForScreen* Screen.width*showPercent;
		GUI.BeginGroup (new Rect (startPointX  ,startPointY , width, height));
		GUI.Box (new Rect (0, 0, width, height ), "" ,GUIShowStyleForBack );//背景
		GUI.Box (new Rect (width/3,  height * 0.05f , width/3, height/5 ), stringForTitle ,GUIShowStyleForTitle);//标题
		GUI.Box (new Rect (width*0.05f, height* 0.25f , width*0.9f, height*3/5 ), stringForInformation , GUIShowStyleForInformation);//文本
		string showOnButton = withTimer? "我已知晓("+timer.ToString("f0")+")" : "我已知晓";
		if (GUI.Button (new Rect (width * 2 / 5, height * 4 / 5, width / 5, height / 8), showOnButton)) {makeEnd ();}

		GUI.EndGroup ();

		if(showPercent  < 1 && !isClosing)
			showPercent += showPercentAdder;

		if(isClosing)
			showPercent -= showPercentAdder;

	}


}
