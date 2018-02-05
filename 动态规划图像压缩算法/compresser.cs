using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class compresser : MonoBehaviour {


	int n = 6;
	int []s ;
	int []l ;
	int []b ;
	int[] p ;
	int mem = 0;
	int Lmax = 256;
	int headAdd = 11;
	// Use this for initialization
	void Start () 
	{
		s = new int[7] ;
		l = new int[7] ;
		b = new int[7] ;
		int  [] p2 =  { 0,10,12,15,255,1,2};
		p = p2;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Space))
			demo ();
		
	}

	int getLength(int i)
	{
		int length = 1;
		while (i >= 2)
		{
			i /= 2;
			length++;
		}
		return length;
	}

	void compress()
	{
		s [0] = 0;
		for (int i = 1; i <=n; i++) 
		{
			 
			b [i] = getLength (p[i]);
			int bMax = b [i];
			s [i] = s [i - 1] + bMax;
			l [i] = 1;
			for (int j = 2; j <= i && j <= Lmax; j++) 
			{
				 
				if (bMax < b [i - j + 1])
					bMax = b [i - j + 1];
				if (s [i] >= s [i - j] + j * bMax)
				{
					s [i] = s [i - j] + j * bMax;
					l [i] = j;
				}
			}
			s [i] += headAdd;
		}
	}
		
	void traceBack(int n2,  int []s,int []l)
	{
 
		if (n2 == 0)
			return;

		traceBack (n2-l[n2],s,l);
		s [mem++] = n2 - l [n2];
	}



	void demo()
	{

		compress ();

		for (int i = 0; i < n; i++) 
		{
			print ("----------------------------------");
			print ("p--"+p[i]);
			print ("s--"+s[i]);
			print ("l--"+l [i]);
			print ("d--"+b[i]);
		}
		print ("sAll---"+s[6]);

		traceBack (n, s,l);
		s [mem] =n;
		print ("m---"+mem);
		for (int j = 1; j <= mem; j++) 
		{
			print ("j---"+j+" s[j]--"+s[j]);
			l [j] = l [s [j]];
			b [j] = b [s [j]];
		}
		for (int j = 1; j <=mem; j++)
			print ("第"+j+"段的像素个数"+l[j]+"，每一个的存储位数"+b[j]);
	}
}
