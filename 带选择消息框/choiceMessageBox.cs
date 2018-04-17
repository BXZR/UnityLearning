using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public delegate void MesageOperate();
public class choiceMessageBox : MonoBehaviour {

	//这个脚本描述带有选择的消息框
	//全局唯一
	public static choiceMessageBox theMessageSave;

	//做一个有点意境的消息框吧
	//OnGUI模式之下这两个无效
	public Text theTitleText;
	public Text theInformationText;
	//一个背景图
	public Texture2D theBackPicture;

	//GUI方法需要的参数
	//所有的东西都是按照屏幕百分比来计算
	private float widthForScreen = 0.4f;
	private float heightForScreen = 0.24f;
	private string stringForTitle = "";
	private string stringForInformation = "";
	public bool isAutoResize = false;

	//有一个缩放的效果
	private bool isClosing = false;//开始还是关闭
	private float showPercent = 0.0f;
	private float showPercentAdder = 0.1f;


	//GUI 额外设定
	GUIStyle GUIShowStyleForTitle;
	GUIStyle GUIShowStyleForInformation;
	GUIStyle GUIShowStyleForBack;

	//额外的操作
	MesageOperate theOperate;

	//设定显示信息
	public void setInformation(string title, string information, MesageOperate theOperateIn )
	{
		theOperate = theOperateIn;
		theTitleText.text = title;
		theInformationText.text = information;
		stringForTitle = title;
		stringForInformation = information;
		if (isAutoResize) 
		{
			float arr = Mathf.Clamp( (float)stringForInformation.Split ('\n').Length / 7 ,1f,2.8f);
			setSize (new Vector2(1f , arr));
		}
	}

	//设定大小
	//用的是百分比
	public void setSize ( Vector2 theSize)
	{
		float widthSave = widthForScreen;
		float heightSave = heightForScreen;
		widthForScreen *= theSize.x;
		heightForScreen *= theSize.y;

		if (widthForScreen< widthSave)
			widthForScreen = widthSave;
		if (heightForScreen < heightSave)
			heightForScreen = heightSave;
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
		GUIShowStyleForTitle.normal.textColor = Color.green;
		GUIShowStyleForTitle.fontStyle = FontStyle.Bold;
		GUIShowStyleForTitle.alignment = TextAnchor.UpperCenter;
		GUIShowStyleForTitle.fontSize = 19;

		GUIShowStyleForInformation=new GUIStyle();
		GUIShowStyleForInformation.normal.textColor = Color.yellow;
		GUIShowStyleForInformation.fontStyle = FontStyle.Bold;
		GUIShowStyleForInformation.alignment = TextAnchor.UpperCenter;
		GUIShowStyleForInformation.fontSize = 16;
		GUIShowStyleForInformation.wordWrap = true;

		GUIShowStyleForBack = new GUIStyle ();
		GUIShowStyleForBack.normal.background = theBackPicture;
	}

	void Start()
	{
		makeAlone ();
		makeStart ();
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
		GUI.Box (new Rect (width/3,  height * 0.05f , width/3, height*0.12f ), stringForTitle ,GUIShowStyleForTitle);//标题
		GUI.Box (new Rect (width*0.05f, height* 0.2f , width*0.9f, height*3/5 ), stringForInformation , GUIShowStyleForInformation);//文本
		if (GUI.Button (new Rect (width * 1 / 5, Mathf.Max( height * 4 / 5, height-80), width / 5, 35), "是")) {theOperate();makeEnd ();}
		if (GUI.Button (new Rect (width * 3 / 5, Mathf.Max( height * 4 / 5, height-80), width / 5, 35), "否")) {makeEnd ();}

		GUI.EndGroup ();

		if(showPercent  < 1 && !isClosing)
			showPercent += showPercentAdder;

		if(isClosing)
			showPercent -= showPercentAdder;

	}

 
}
