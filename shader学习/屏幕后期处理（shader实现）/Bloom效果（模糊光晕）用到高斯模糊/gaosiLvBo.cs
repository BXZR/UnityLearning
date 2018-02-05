using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gaosiLvBo : grphicsMade
{

	public Shader theShader;
	private Material theMaterial;

	public Material TheMaterial
	{
		get
		{
			theMaterial = checkShaderAndCreateMaterial (theShader , theMaterial);
			return theMaterial;
		}
	}

	[Range(0,4)]
	public int iterations = 2;
	[Range(0.2f, 3.0f)]
	public float  blurSpeed = 0.6f;
	[Range(1,8)]
	public int downSample = 2;

	void OnRenderImage (RenderTexture src , RenderTexture desc)
	{
		if (TheMaterial != null) 
		{
			//降低采样的宽高，减少计算量
			int W = src.width / downSample; 
			int H = src.height / downSample;
			//建立缓冲区存储原始图
			RenderTexture buff0 = RenderTexture.GetTemporary (W, H, 0);
			buff0.filterMode = FilterMode.Bilinear;//双线性滤波方法
			Graphics.Blit (src, buff0);//第一次绘制,0表示第一个pass

			for (int i = 0; i < iterations; i++) {
				TheMaterial.SetFloat ("_BlurSize", 1.0f + i * blurSpeed);
				//做第一次混合
				RenderTexture buff1 = RenderTexture.GetTemporary (W, H, 0);
				//使用第一个pass来做处理
				Graphics.Blit (buff0, buff1, TheMaterial, 0);
				//释放缓冲区
				RenderTexture.ReleaseTemporary (buff0);
				buff0 = buff1;
				buff1 = RenderTexture.GetTemporary (W, H, 0);

				//使用第二个pass做处理
				Graphics.Blit (buff0, buff1, TheMaterial, 1);
				RenderTexture.ReleaseTemporary (buff0);
				buff0 = buff1;

				//每一次pass处理后的结果都会保存到buff0中
			}
			Graphics.Blit (buff0 , desc);
			RenderTexture.ReleaseTemporary (buff0);
		}
		else 
		{
			Graphics.Blit (src , desc);//其实就是什么都不做
		}
	}
}
