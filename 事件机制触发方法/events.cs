using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine .EventSystems;

public class events : MonoBehaviour {

	public  GameObject theButton;//目标GameObject

	//使用事件的方式调用UI你的时间
	//作为事件触发是一个和拌好的思路吧
	void clickEventUse()
	{
		PointerEventData theData = new PointerEventData (EventSystem.current );//创建事件数据
		//传值：大概理解是：目标Gameobject ，事件数据 ， 类型（与那边接收的时候做匹配（大概））
		ExecuteEvents .Execute<IPointerClickHandler> ( theButton, theData ,ExecuteEvents.pointerClickHandler);
	}
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Space))
			clickEventUse ();//这样，按下Space键位也会触发Button的点击事件
		//这种做法在VR中还是比较实用的，这是一种不用鼠标的与UI进行交互的手段
	}
}
