using UnityEngine;
using System.Collections;

public class screenShaderController_1 : grphicsMade
{
	//这个脚本的关键在于 OnRenderImage(RenderTexture src , RenderTexture dest)
	//做屏幕后期处理的效果

	public Shader theShader;//效果shader
	private Material theMaterial;//使用材质来表现shader的效果

	public Material TheMaterial
	{
		get
		{
			theMaterial = checkShaderAndCreateMaterial (theShader,theMaterial);
			return theMaterial;
		}
		//保证材质的只读效果，使用属性来实现。
	}

	[Range (0.0f,3.0f)]
	public float brightness = 1.0f;//亮度
	[Range (0.0f , 3.0f)]
	public float saturation = 1.0f;//饱和度
	[Range (0.0f,3.0f)]
	public float contrast = 1.0f;//对比度

	void OnRenderImage(RenderTexture src , RenderTexture dest)
	{
		//简单地理解这个就是屏幕后期处理效果的处理方法
		//其中src是上一步渲染的到的，需要在摄像机前面绘制的那张图
		//dest是经过这个方法之后打算在摄像机前面绘制的图
		if (TheMaterial != null) {//直接用属性进行查询
			//应该注意的是这里到额参数名称必须与shader的参数名称一致，空格也一样才行
			TheMaterial.SetFloat ("_Brightness", brightness);
				TheMaterial.SetFloat ("_Saturation", saturation);
				TheMaterial.SetFloat ("_Contrast", contrast);
			Graphics.Blit (src , dest , TheMaterial);//用材质绘制
		}
		else
		{
			Graphics.Blit (src , dest);//初始化之后一致绘制就可以了
		}
	}

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}

