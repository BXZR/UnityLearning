using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gimbalLock : MonoBehaviour {

	int step = 0;

	void Start () {
		
	}
	
	// Update is called once per frame
//���������ʾ��
//�ǳ������һ���޷���ת�����⣬��˵��Ҫʹ����Ԫ���ֵ�ķ������
	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.Space))
		{
			step++;
			switch (step) 
			{
			case 1:
				{
					this.transform.Rotate (this.transform.right * 30);
				}
				break;
			case 2:
				{
					this.transform.Rotate (this.transform.up * 90);
				}
				break;
			case 3:
				{
					this.transform.Rotate (this.transform .forward *-40);
				}
				break;
			default :
				{
					this.transform.rotation = new Quaternion (0,0,0,0);
					this.transform.localRotation = new Quaternion (0,0,0,0);
					step = 0;
				}
				break;
				
			}

		}
	}
}
