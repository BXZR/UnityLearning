using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bloom : grphicsMade //继承处理用的基类
{

	public Shader theShader;//bloom效果的shader
	private  Material theMaterial;//用于创建的材质

	public  Material TheMaterial//只读
	{
		get
		{
			theMaterial = checkShaderAndCreateMaterial (theShader , theMaterial);//顺带一提，这个写法其实还是有一点冗余的感觉
			return theMaterial ;
		}
	}

	[Range(0,4)]
	public int iterations = 2;
	[Range(0.2f, 3.0f)]
	public float  blurSpeed = 0.6f;
	[Range(1,8)]
	public int downSample = 2;
	//用到的高斯模糊的参数
	[Range(0.0f ,4.0f)]
	public float  lum = 0.6f;
	//bloom是应用高斯模糊的

	void OnRenderImage (RenderTexture src , RenderTexture desc)
	{
		if (TheMaterial != null) 
		{
			//控制shader的参数
			TheMaterial.SetFloat ("_lum", lum);

			//下面的方法与高斯的方法很相近
			//降低采样的宽高，减少计算量
			int W = src.width / downSample; 
			int H = src.height / downSample;
			//建立缓冲区存储原始图
			RenderTexture buff0 = RenderTexture.GetTemporary (W, H, 0);
			buff0.filterMode = FilterMode.Bilinear;//双线性滤波方法
			Graphics.Blit (src, buff0, TheMaterial , 0);//用shader的第一个pass提取出比较亮的部分，需要对这个部分做高斯处理

			for (int i = 0; i < iterations; i++) {
				TheMaterial.SetFloat ("_BlurSize", 1.0f + i * blurSpeed);
				//做第一次混合
				RenderTexture buff1 = RenderTexture.GetTemporary (W, H, 0);
				//使用第二个pass来做处理
				Graphics.Blit (buff0, buff1, TheMaterial, 1);
				//释放缓冲区
				RenderTexture.ReleaseTemporary (buff0);
				buff0 = buff1;
				buff1 = RenderTexture.GetTemporary (W, H, 0);

				//使用第三个pass做处理
				Graphics.Blit (buff0, buff1, TheMaterial, 2);
				RenderTexture.ReleaseTemporary (buff0);
				buff0 = buff1;

				//用两个pass做高斯处理是因为要是用两次卷积（横向卷积核纵向卷积）
				//每一次pass处理后的结果都会保存到buff0中
			}
			TheMaterial.SetTexture ("_Bloom", buff0);
			Graphics.Blit (src, desc,TheMaterial ,3);//用第四个pass做最终的混合
			RenderTexture.ReleaseTemporary (buff0);
		}
		else 
		{
			Graphics.Blit (src , desc);//其实就是什么都不做
		}
	}


}
