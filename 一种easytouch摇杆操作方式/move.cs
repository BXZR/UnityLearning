using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class move : MonoBehaviour {


	public GameObject EffectForDestination;//导航目标标记，可以为空
	public float moveSpeed = 0.5f;//移动速度
	[Range(0f,1f)]
	public float rotateSpeed = 0.3f;

	//为了保证设定面板的简洁，暂时隐藏的一些参数
	[HideInInspector]
	public string xAxisName = "Vertical";//左右移动的轴名称
	[HideInInspector]
	public string yAxisName = "Horizontal";//前后移动的轴名称

	private NavMeshAgent TheAgent;//导航代理
	private Vector2 moveAxisValue;//输入轴数据保存
	private Quaternion headingAim  = Quaternion.identity;//转向的目标，用于差值

	/// <summary>
	/// 自动寻路的目标检查
	/// 这个是用于鼠标点击的时候和任务自动寻路的时候使用的
	/// </summary>
	public void FindDestination()
	{
		if (!TheAgent)
			return;

		//print ("开始寻路");
		Ray mRay = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit mHi;
		if (Physics.Raycast (mRay, out mHi))
		{
			//print (mHi.collider.gameObject.name+" is found");
			if (mHi.collider .tag == "earth")
			{
				Vector3 destenation = new Vector3 (mHi .point .x, mHi .point .y, mHi .point .z);
				TheAgent.SetDestination (destenation);

				if(TheAgent.hasPath)
				{
					if (EffectForDestination) 
					{
						GameObject ef = (GameObject)Instantiate (EffectForDestination);//创建预设
						ef.transform.position = destenation;
					}
				}
			}
		}
	}



	/// <summary>
	/// 使用轴进行移动的时候的操纵方法
	/// </summary>
	public void InputOperateWithAxis(Vector2 theAxis)
	{
		//控制当前的转向---------------------------------------------------------------------------------------------------------------------------
		this.transform.rotation = Quaternion.Lerp (this.transform.rotation , headingAim , rotateSpeed);

		//计算当前的目标角度-----------------------------------------------------------------------------------------------------------------------
		float xAxisValue = theAxis.x;
		float yAxisValue = theAxis.y;
		//用cos来算比较保靠，如果用tan就可能会出现0的情况
		//例如x = 0 ， y = 1
		float allAxisAdd = xAxisValue * xAxisValue + yAxisValue * yAxisValue;
		if (allAxisAdd <= 0.72f)
			return;

		float Yadd = Mathf.Asin( xAxisValue / Mathf.Sqrt( allAxisAdd) ) *Mathf.Rad2Deg;
		Vector3 eulerOld = this.transform.rotation.eulerAngles;
		Vector3 eulerNew = Vector3.zero;
		Yadd = yAxisValue > 0 ? Yadd : 180-Yadd;
	    eulerNew = new Vector3 (eulerOld.x , Yadd , eulerOld.z);
		headingAim = Quaternion.Euler (eulerNew );

		//真正的移动过程---------------------------------------------------------------------------------------------------------------------------
		//print ("headingAim.eulerAngles.y - this.transform.rotation.y  " + headingAim.eulerAngles.y +" - "+ this.transform.rotation.eulerAngles.y );
		//这是用于真正移动的方法，单独放在这里是因为很多时候是不可以移动的
		if( Mathf.Abs (headingAim.eulerAngles.y - this.transform.rotation.eulerAngles.y ) < 10f )
			TheAgent.SetDestination (this.transform.position + this.transform.forward * 1f);
	}

	/// <summary>
	/// 获取输入操作
	/// </summary>
	private Vector2 getAxisInputPC()
	{
		float xAxisValue = Input.GetAxis (yAxisName);
		float yAxisValue= Input.GetAxis (xAxisName);
		xAxisValue= Mathf.Abs (xAxisValue) < 0.6f ? 0f : xAxisValue;
		yAxisValue = Mathf.Abs (yAxisValue) < 0.6f ? 0f : yAxisValue;
		return new Vector2 (xAxisValue , yAxisValue);
	}



	void Start () 
	{
		TheAgent = this.GetComponent<NavMeshAgent> ();
		headingAim = this.transform.rotation;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (SystemValues.IsOperatingUI ())
			return;

		if (Input.GetMouseButtonDown (0))
			FindDestination ();

		moveAxisValue = getAxisInputPC ();
		InputOperateWithAxis(moveAxisValue );
	}
}
