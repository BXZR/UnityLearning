using UnityEngine;
using UnityEditor;
using System .Reflection;
using System .Collections;


//用于测试，显示的类
[System .Serializable]
public class check : System.Object
{
	private int values = 0;
	public int Values
	{
		get { return this.values;}
		set
		{
			if (value < 100)
				this.values = value;
		}
	}
}

[CustomPropertyDrawer(typeof (check))]//用这种方式关联类
public class checkMaker : PropertyDrawer
{
	float inspectorHeighter = 0;
	float rowHeight = 15;
	float rowSpaceing = 5;

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{ 
		//与GUI一样的套路结构
		EditorGUI.BeginProperty (position, label, property);

		object o = property.serializedObject.targetObject;
		check C = o.GetType ().GetField (property.name).GetValue (o) as check;

		int indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		Rect layoutRect = new Rect (position .x , position .y,position .width,rowHeight);
		foreach (var prop in typeof(check).GetProperties(BindingFlags.Public | BindingFlags.Instance)) 
		{
			if (prop.PropertyType.Equals (typeof(int)))
			{
				//这就是显示各种不同数据结构的套路，使用不同的EditorGUI.XXXXField
				//例如：EditorGUI.IntField int
				//EditorGUI .FloatField float
				//EditorGUI .ColorField color
				prop .SetValue(C,EditorGUI.IntField(layoutRect ,prop.Name, (int )prop .GetValue(C ,null)),null);
				layoutRect = new Rect (layoutRect .x ,layoutRect .y + rowHeight + rowSpaceing,layoutRect.width ,rowHeight);
			}
		}
		inspectorHeighter = layoutRect.y - position.y;

		EditorGUI.indentLevel = indent;

		EditorGUI.EndProperty ();

	}
	//用于修正位置
	public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
	{
		return inspectorHeighter;
	}

}

//这个这个文件的主类。也只有这个类用于挂在游戏对象上
public class opRefelct : MonoBehaviour {
	public check beCheck;//被用来测试的对象

}
