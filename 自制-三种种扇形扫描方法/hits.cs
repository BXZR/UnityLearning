using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hits : MonoBehaviour {

	public float angle = 30f;//探测的角度
	public float distance = 5f;//探测视野范围
	private float angleNow = 0f;//当前的角度值 
	private float angelAdder =0.5f;
	private List<GameObject> finds;//为了防止重复，并且获取所有检测的物体

	float change(float angle)//角度转弧度的方法
	{
		return( angle * Mathf.PI / 180);
	}

	void init()
	{
		finds = new List<GameObject> ();//重建
		basicFwd = this.transform.position +this.transform.right.normalized * distance;//基准座标
	}
	void basicHit()
	{
		RaycastHit hit;
		Vector3 fwd = transform.TransformDirection(Vector3.forward);//有一点冗余，这个是相对坐标到世界坐标的转换
		if (Physics.Raycast(transform.position, fwd,out hit,6))//只有在有反馈的时候才会显示
			Debug.DrawLine(transform.position,hit.point,Color.red);//起点终点和颜色，不是方向
	
	}


	void angelChange()//双向摇摆检查的方式，比较慢，但是似乎比较适合巡逻检查
	{
		angleNow += angelAdder;
		if (angleNow > angle || angleNow <-angle)
			angelAdder = -angelAdder;

	}
    
	void  search(float angle)//会受到阻挡的射线检测（循环起来就是扇形检测）
	{
		RaycastHit hit;
		Vector3 basicFwd = new Vector3 (this.transform.position.x + distance, this.transform.position.y, this.transform.position.z);
		Debug.DrawLine (transform.position, basicFwd, Color.yellow);//基准线
		float zAdd = distance * Mathf.Sin (change (angle));
		float xAdd = distance - distance * Mathf.Cos (change (angle));
		float zNow = basicFwd.z + zAdd;
		float xNow = basicFwd.x - xAdd;

		Vector3 theAimPosition = new Vector3 (xNow, this.transform.position.y, zNow);
		Vector3 direction = theAimPosition - this.transform.position;

		Debug.DrawLine (transform.position, theAimPosition, Color.red);
		if (Physics.Raycast (transform.position, direction, out hit, distance))
		{
			Debug.DrawLine (transform.position, theAimPosition, Color.magenta);
			if (finds.Contains (hit.collider.gameObject) == false) 
			{
				finds.Add (hit.collider.gameObject);
				print (hit.collider.gameObject.name);
			}
		}
	}

	void  searchAll(float angle)//不会受到阻挡的扇形检测(循环起来就是扇形检测)
	{
		RaycastHit [] hits;
		Vector3 basicFwd = new Vector3 (this.transform.position.x + distance, this.transform.position.y, this.transform.position.z);
		Debug.DrawLine (transform.position, basicFwd, Color.yellow);//基准线
		float zAdd = distance * Mathf.Sin (change (angle));
		float xAdd = distance - distance * Mathf.Cos (change (angle));
		float zNow = basicFwd.z + zAdd;
		float xNow = basicFwd.x - xAdd;

		Vector3 theAimPosition = new Vector3 (xNow, this.transform.position.y, zNow);
		Vector3 direction = theAimPosition - this.transform.position;

		Debug.DrawLine (transform.position, theAimPosition, Color.red);
		hits = Physics.RaycastAll (transform.position, direction,  distance);
			for(int i=0;i<hits .Length;i++)
			{
				if(finds.Contains (hits[i].collider.gameObject) ==false)
				{
					finds.Add (hits[i].collider.gameObject);
					print ("SeachFind "+hits[i].collider.gameObject.name);
				}
			}
 
	}


	void makeSee()//用画线的方法使得侦测范围可见（不跟随选转）
	{
		for (float angles = -this.angle; angles < this.angle; angles++)//按照角度进行扫描
		{
			Vector3 basicFwd = new Vector3 (this.transform.position.x + distance, this.transform.position.y, this.transform.position.z);//基准座标
			float zAdd = distance * Mathf.Sin (change (angles));
			float xAdd = distance - distance * Mathf.Cos (change (angles));
			float zNow = basicFwd.z + zAdd;
			float xNow = basicFwd.x - xAdd;
			//通过cos和sin计算修正后的坐标值
			Vector3 theAimPosition = new Vector3 (xNow, this.transform.position.y, zNow);//计算圆形上面点的坐标
			//自身位置和圆上面的坐标点画线
			Debug.DrawLine (transform.position, theAimPosition, Color.red);
		}
	}

//原始不跟随选转德芳法
/*********************************************************************************************************************/

	Vector3 basicFwd;//= this.transform.position +this.transform.right.normalized * distance;//基准座标
	//基准坐标只在初始化的赋值，这个是计算坐标的时候的“正方向基准值”
	void makeSeeWorld()//用画线的方法使得侦测范围可见（划线位置跟随观察者的y轴旋转进行旋转）
	{
		Debug.DrawLine (transform.position,this.transform.position +this.transform.right.normalized * distance*2f, Color.yellow);
		for (float angles = -this.angle; angles < this.angle; angles++)//按照角度进行扫描
		{
			float angleInPart =  -(angles  + transform.eulerAngles.y);
		    //在具体运用的时候千万注意XZ轴的坐标顺序
			float zAdd = distance * Mathf.Sin (change (angleInPart));
			float xAdd = distance - distance * Mathf.Cos (change (angleInPart));
		    //在这个算法中没有考虑y轴坐标，也就是没有相对高度的信息采集
			float zNow = basicFwd.z + zAdd;
			float xNow = basicFwd.x - xAdd;
			//通过cos和sin计算修正后的坐标值
			Vector3 theAimPosition = new Vector3 (xNow, this.transform.position.y, zNow);//计算圆形上面点的坐标
			//自身位置和圆上面的坐标点画线
			Debug.DrawLine (transform.position, theAimPosition, Color.red);

		}
	}
	void  searchRound(float angle)//会受到阻挡的射线检测（循环起来就是扇形检测）
	{
		RaycastHit hit;
		float angleInPart =  -(angle  + transform.eulerAngles.y);
		Debug.DrawLine (transform.position, this.transform.position +this.transform.right.normalized * distance*2f, Color.yellow);//基准线
		float zAdd = distance * Mathf.Sin (change ( angleInPart ));
		float xAdd = distance - distance * Mathf.Cos (change ( angleInPart ));

		float zNow = basicFwd.z + zAdd;
		float xNow = basicFwd.x - xAdd;

		Vector3 theAimPosition = new Vector3 (xNow, this.transform.position.y, zNow);
		Vector3 direction = theAimPosition - this.transform.position;

		Debug.DrawLine (transform.position, theAimPosition, Color.green);
		if (Physics.Raycast (transform.position, direction, out hit, distance))
		{
			Debug.DrawLine (transform.position, theAimPosition, Color.magenta);
			if (finds.Contains (hit.collider.gameObject) == false) 
			{
				finds.Add (hit.collider.gameObject);
				print (hit.collider.gameObject.name);
			}
		}
	}

	void  searchAllRound(float angle)//不会受到阻挡的扇形检测(循环起来就是扇形检测)
	{
		RaycastHit [] hits;
		float angleInPart =  -(angle  + transform.eulerAngles.y);
		Debug.DrawLine (transform.position, this.transform.position +this.transform.right.normalized * distance*2f, Color.yellow);//基准线
		float zAdd = distance * Mathf.Sin (change ( angleInPart ));
		float xAdd = distance - distance * Mathf.Cos (change ( angleInPart ));
		float zNow = basicFwd.z + zAdd;
		float xNow = basicFwd.x - xAdd;

		Vector3 theAimPosition = new Vector3 (xNow, this.transform.position.y, zNow);
		Vector3 direction = theAimPosition - this.transform.position;

		Debug.DrawLine (transform.position, theAimPosition, Color.green);
		hits = Physics.RaycastAll (transform.position, direction,  distance);
		for(int i=0;i<hits .Length;i++)
		{
			if(finds.Contains (hits[i].collider.gameObject) ==false)
			{
				finds.Add (hits[i].collider.gameObject);
				print ("SeachFind "+hits[i].collider.gameObject.name);
			}
		}

	}

	void searchInFlash()//立即进行一次全部范围的扇形检查（非常快，从一侧到另一侧的单次扫描）
	{
		float angelInUse = -this.angle;//从负数开始
		//进行一次角度的扫描
		for (; angelInUse < this.angle; angelInUse += this.angelAdder) 
		{
			search(angelInUse);//不包含穿透
			//searchAll (angelInUse);//包含穿透
		} 
	}



	//个人认为比较稳健的方法
	void searchMethod2(float angle)//不使用射线而是使用向量计算方法
	{
		//这个方法的正方向使用的是X轴正方向
		//具体使用的时候非常需要注意正方向的朝向
		finds = new List<GameObject> ();//寻找到的列表，避免重复查找
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
			if (emys [i].GetComponent <Collider>().gameObject != this.gameObject) //相交球最大的问题就是如果自身有碰撞体，自己也会被侦测到
			{
				//print ("name-"+ emys [i].name);
				Vector3 thisToEmy = emys [i].transform.position - this.transform.position;//目标坐标减去自身坐标
				Vector2 theVectorToSearch = (new Vector2 (thisToEmy.x, thisToEmy.z)).normalized;//转成2D坐标，高度信息在这个例子中被无视
				//同时进行单位化，简化计算向量cos值的时候的计算
				Vector2 theVectorForward = (new Vector2 (this.transform.right.x, this.transform.right.z)).normalized;//转成2D坐标，高度信息在这个例子中被无视
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
					if (finds.Contains (emys [i].GetComponent<Collider> ().gameObject) == false) //不重复地放到已找到的列表里面
					{
						finds.Add (emys [i].GetComponent<Collider> ().gameObject);
						print ("SeachFind "+emys [i].GetComponent<Collider> ().gameObject.name);//找到目标
					}
				}
			}

		}

	}



	void Start () 
	{
		init ();//进行初始化
		//在这个例子中就是初始化finds列表
		InvokeRepeating ("angelChange",0,0.001f);
	}
	void Update () {
        //searchAll(this.angleNow);
		//makeSee ();
		makeSeeWorld();
		//searchRound(this.angleNow);
		searchAllRound(this.angleNow);
		if (Input.GetKeyDown (KeyCode.Space))
		{

		//	searchMethod2(this.angle);
			//searchInFlash ();
			//print (transform.eulerAngles.y);
		}

	}
}
