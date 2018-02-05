using UnityEngine;
using System.Collections;

public class convolutionDemo : grphicsMade
{

	//使用卷积的方法进行描边处理
	//用shader的方法实现屏幕后期处理之屏幕描边

	public Shader theShader;//描边用的shader
	private Material theMaterial  =  null;
	[Range(0.0f,1.0f)]
	public float edgesOnly = 0.0f;
	public Color edgeColor = Color.black;
	public Color backGroundColor = Color .white;
	public  Material TheMaterial
	{
		get
		{
			theMaterial = checkShaderAndCreateMaterial (theShader, theMaterial);
			return theMaterial;
		}
	}

	void OnRenderImage(RenderTexture src , RenderTexture  desc)
	{
		if (TheMaterial != null)
		{
			//C#脚本控制shader参数
			TheMaterial.SetFloat ("_EdgeOnly", edgesOnly);
			TheMaterial.SetColor ("_EdgeColor", edgeColor);
			TheMaterial.SetColor ("_BackgroundColor", backGroundColor);
			Graphics.Blit (src, desc, TheMaterial);
		}
		else
		{
			Graphics.Blit (src , desc);
		}

	}
}

