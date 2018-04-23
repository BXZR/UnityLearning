using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodScaner : MonoBehaviour {

	//观察显示血条
	//主动搜索的策略而不是被动接受的策略
	//搜索会有开销，但是会减少整体显示的时间和内容，似乎更为合适
	//有这个脚本的单位以看到BloodBasic

	List<BloodBasic> theEMY  = new List<BloodBasic> ();
	public float angle = 30f;
	public float distance = 4f;
	private PlayerBasic thePlayer;//这个脚本是依附于playerBasic上面的，没有玩家，就没有必要展示了

	void Start ()
	{
		thePlayer = this.GetComponent <PlayerBasic> ();
		if (thePlayer)
		{
			angle = thePlayer.theViewAreaAngel;
			distance = thePlayer.theViewAreaLength;
			InvokeRepeating ("searchAIMs", 2f, 0.2f); 
		} 
		else 
		{
			Destroy (this);
		}
	}
	float change(float angle)//角度转弧度的方法
	{
		return( angle * Mathf.PI / 180);
	}

 
	//个人认为比较稳健的方法
	//传入的是攻击范围和攻击扇形角度的一半
	//选择目标的方法，这年头普攻都是AOE
	void searchAIMs()//不使用射线而是使用向量计算方法
	{
		//视野很有可能可以受到限制，而是这也许就是“致盲”效果的初始阶段了
		angle = thePlayer.theViewAreaAngel;
		distance = thePlayer.theViewAreaLength;
		//这个方法的正方向使用的是X轴正方向
		//具体使用的时候非常需要注意正方向的朝向
		theEMY.Clear();
		//以自己为中心进行相交球体探测
		//实际上身边一定圆周范围内的所有具有碰撞体的单位都会被被这一步探测到
		//接下来需要的就是对坐标进行审查
		Collider [] emys = Physics.OverlapSphere (this.transform .position, distance);
		//使用cos值进行比照，因为在0-180角度范围内，cos是不断下降的
		//具体思路就是，判断探测到的物体的cos值如果这个cos值大于标准值，就认为这个单位的角度在侦查范围角度内。
		float angleCosValue = Mathf.Cos (change(angle));//莫认真侧角度的cos值作为计算标准
		//print ("angleCosValue-"+angleCosValue);
		for (int i = 0; i < emys.Length; i++)//开始对相交球体探测物体进行排查
		{ 
			//print (emys [i].gameObject.name);
			if (emys[i].GetComponent <BloodBasic>() && emys [i].GetComponent <Collider>().gameObject != this.gameObject) //相交球最大的问题就是如果自身有碰撞体，自己也会被侦测到
			{
				//print ("name-"+ emys [i].name);
				Vector3 thisToEmy = emys [i].transform.position - this.transform.position;//目标坐标减去自身坐标
				Vector2 theVectorToSearch = (new Vector2 (thisToEmy.x, thisToEmy.z)).normalized;//转成2D坐标，高度信息在这个例子中被无视
				//同时进行单位化，简化计算向量cos值的时候的计算
				Vector2 theVectorForward = (new Vector2 (this.transform.forward.x, this.transform.forward.z)).normalized;//转成2D坐标，高度信息在这个例子中被无视
				//同时进行单位化，简化计算向量cos值的时候的计算
				float cosValue = (theVectorForward.x * theVectorToSearch.x + theVectorForward.y * theVectorToSearch.y);//因为已经单位化，就没必要再进行求模计算了
				//print ("cosValue-" + cosValue);
				/*
				 * 先求出两个向量的模
					再求出两个向量的向量积
					|a|=√[x1^2+y1^2]
					|b|=√[x2^2+y2^2]
					a*b=(x1,y1)(x2,y2)=x1x2+y1y2
					cos=a*b/[|a|*|b|]
					=(x1x2+y1y2)/[√[x1^2+y1^2]*√[x2^2+y2^2]]
				 * 
				*/
				if (cosValue >= angleCosValue)//如果cos值大于基准值，认为这个就是应该被探测的目标
				{
					BloodBasic theAIM = emys [i].gameObject.GetComponent<BloodBasic> ();
					if (theEMY .Contains (theAIM) == false) //不重复地放到已找到的列表里面
					{
						theEMY.Add (theAIM);
						//print ("SeachFind "+emys [i].GetComponent<Collider> ().gameObject.name);//找到目标
					}
				}
			}
		}
		foreach(BloodBasic B in theEMY)
			B.flashBloodShowTimer();

	}

}
