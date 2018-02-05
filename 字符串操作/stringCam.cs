using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class stringCam : MonoBehaviour {
	//字符串操作方法例子

	//判断字符串的方法A，直接比较
	public bool stringCamer(string a,string b )
	{
		//这只是简单的字符串比较的方法，需要注意的是这里有额外的选项
		return string.Equals (a,b,System.StringComparison.OrdinalIgnoreCase);
	}
	//判断字符串的方法A，转换成哈希比较
	public bool stringCamer2(string a,string b)
	{
		//非常惊讶的就是这里使用的是Animator的方法，估计跟状态机字符串哈希有关
		return Animator.StringToHash(a) == Animator.StringToHash(b);
	}

	//这个方法功能可以比较多
	//a在b之前，返回-1；二者相等，返回0；b在a之前，返回1
	//但是这个方法效率比较低

	 // Ordinal 使用序号排序规则比较字符串
	 //对两个字符串进行byte级别的比较,因此这种比较是比较严格和准确的,并且在性能上也很好
	 
	//OrdinalIgnoreCase 使用序号排序规则并忽略被比较字符串的大小写，对字符串进行比较。
	//同样是byte级别的比较.性能稍弱于StringComparison.Ordinal

	// CurrentCulture 在当前的区域信息下进行比较,这是String.Compare在没有指定StringComparison的时候默认的比较方式

	//InvariantCulture来进行字符串比较,在任何系统中(不同的culture)比较都将得到相同的结果

	//XXXIgnoreCase 

	public int stringOrder(string a,string b)
	{
		return string.Compare (a,b,System.StringComparison.CurrentCultureIgnoreCase);
	}

	//文一点说就是字符串格式化，感觉比字符串拼接方便很多了
	 /*
		public static string Format (IFormatProvider provider, string format, params object[] args)
		{
			if (format == null || args == null) {
				throw new ArgumentNullException ((format == null) ? "format" : "args");
			}
			StringBuilder stringBuilder = new StringBuilder (format.Length + args.Length * 8);
			stringBuilder.AppendFormat (provider, format, args);
			return stringBuilder.ToString ();
		}
	 */
	public string stringFormats(int ID)
	{
		return string.Format ("the ID is {0}", ID);
	}

	//字符串循环
	//给出三种字符串迭代的方法
	//顺带一提，中英文都可以
	//stringLoop("suck"); 	stringLoop("君临天下") ;
	public void stringLoop(string a)
	{
		print ("=====================================1");
		foreach (char d in a) print ("_"+d);
			
		print ("=====================================2");
		IEnumerator thr = a.GetEnumerator ();
		while (thr.MoveNext ()) print ("_"+(char)thr.Current);
			
		print ("=====================================3");
		for(int i=0;i<a.Length;i++)  print ("_"+a[i]);
			
	}

	//这个方法当然仅仅是一个示例
	//据说为了保持与.NET一致的工作方式
	//推荐用这种方式替代string s = "";
	public string makeNewString()
	{
		return string.Empty;
	}

	//字符串搜索子串返回开始下标
	public int searchChildString(string longString , string childStringToSearch)
	{
		return longString.IndexOf (childStringToSearch);
	}

	//字符串的正则表达式
	//支持字符串的高级查询
	//[dw]ay表示搜索全部以ay结束以d或者w开始的字符串
	//似乎.NET用RegularExpressions名字空间做的正则表达式
	public void stringFLiter(string stringToBeSearch, string fliter = "[dw]ay")
	{
		//Collection是一个集合，获取所有的符合条件项目
		MatchCollection m = Regex.Matches (stringToBeSearch,fliter);
		for(int i=0;i<m.Count;i++)
			print (m[i].Value);
		print ("--------------------------------");
		//获取第一个
		Match m2 = Regex.Match (stringToBeSearch,fliter);
		print (m2.Value);
		print ("--------------------------------");
		//获取所有的符合条件项目
		Match m3 = Regex.Match (stringToBeSearch,fliter);
		while (m3.Success)
		{
			print (m3.Value);
			m3 = m3.NextMatch ();
		}
	}





	void Start () 
	{
		
	}

	void Update ()
	{
		if(Input .GetKeyDown(KeyCode .Space))
			stringFLiter ("waydaywaydaydydydydykdfjkdfgdjfwayday");
	}
}
