using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PPDD : MonoBehaviour {

	long f(long a, long b)//最大公约数 
    {
		   if (a < b) { a = a + b; b = a - b; a = a - b; }
		   return (a % b == 0) ? b : f(a % b, b);
	}

	long m (long a, long b)//最小公倍数 
	 {
		 return a * b / f(a, b);
	 }

	//1-n中能被所有整数整除的最小的数
	void canculate(long  n)
	{
		long value = 1;
		for(long i = 1 ; i <= n ; i++)
			value = m (value , i);

		print ("value = " + value);
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.Space))
			canculate (20);
	}
}
