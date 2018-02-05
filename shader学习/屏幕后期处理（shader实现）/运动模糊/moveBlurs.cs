using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveBlurs : grphicsMade {

	public Shader  theShader;
	private Material theMaterial;
	public Material TheMaterial
	{
		get
		{
			theMaterial = checkShaderAndCreateMaterial (theShader, theMaterial);
			return theMaterial;
		}
	}
	//这个值越大，拖尾效果越明显，为了不把整体的效果也掩盖掉，所以限制在0.0和0.9之间 
	[Range(0.0f,0.9f)]
	public float blurAmount = 0.5f;
	private RenderTexture savedTexture =null;//保存的渲染后结果

	//很谨慎的内存保护，值得学习
	//此外还有防止停下之后瞬移然后再移动造成的第一帧突变的问题
	void  OnDisable()
	{
		DestroyImmediate (savedTexture);
	}

	void OnRenderImage(RenderTexture src  ,RenderTexture desc)
	{
		if (TheMaterial != null) 
		{
			if (savedTexture == null || savedTexture.width != src.width || savedTexture.height != src.height) 
			{
				//不满足要求就需要重新建立缓存
				DestroyImmediate (savedTexture);
				savedTexture = new RenderTexture (src.width, src.height, 0);
				savedTexture.hideFlags = HideFlags.HideAndDontSave;
				Graphics.Blit (src, savedTexture);
			}

			savedTexture.MarkRestoreExpected ();//就是用这个方法来获得的上一帧的图像
			//这个方法的作用是渲染纹理的恢复操作
			//恢复操作发生在渲染到纹理且纹理没有内提前清空或者销毁的情况下
			TheMaterial.SetFloat ("_BlurAmount", 1.0f - blurAmount);
			Graphics.Blit (src, savedTexture, TheMaterial);//其实就是多加上了一层混合

			Graphics.Blit (savedTexture, desc);
		}
		else
		{
			Graphics.Blit (src , desc);
		}
	}



}
