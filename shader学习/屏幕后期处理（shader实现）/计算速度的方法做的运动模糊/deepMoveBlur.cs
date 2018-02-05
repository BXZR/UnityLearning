using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deepMoveBlur : grphicsMade 
{
	public Shader theShader;
	private Material theMaterial;

	public Material TheMaterial
	{
		get
		{
			theMaterial = checkShaderAndCreateMaterial (theShader , theMaterial);
			return  theMaterial;
		}
	}

	[Range (0.0f, 1.0f)]
	public float  blurSize = 0.5f;

	private  Camera theCamera;
	public Camera TheCamera
	{
		get
		{
			if (theCamera == null)
				theCamera = this.GetComponent<Camera> ();
			return theCamera;
		}
	}

	private Matrix4x4 PVPM;//上一帧摄像机视角的投影矩阵

	void OnEnable()
	{
		//设置摄像机的状态
		//为什么用“或”来设置摄像机的属性暂时还不知道
		TheCamera.depthTextureMode |= DepthTextureMode.Depth;
	}

	void OnRenderImage(RenderTexture src , RenderTexture desc )
	{
		if (TheMaterial != null) 
		{
			theMaterial.SetFloat ("_BlurSize", blurSize);

			theMaterial.SetMatrix ("_PVPM", PVPM);
			// TheCamera.worldToCameraMatrix 摄像机视角矩阵
			// TheCamera.projectionMatrix 摄像机投影矩阵
			// 得到的是师姐坐标中顶点坐标在前一帧中的NDC坐标
			// NDC为归一化的设备坐标
			Matrix4x4 CVPM = TheCamera.projectionMatrix * TheCamera.worldToCameraMatrix;
			//矩阵求逆矩阵用inverse
			Matrix4x4 CVPIM = CVPM.inverse;//当前的深度纹理
			theMaterial.SetMatrix ("_CVPIM", CVPIM);
			PVPM = CVPM;//上一帧的深度纹理
			Graphics.Blit (src , desc , theMaterial);
		} 
		else
		{
			Graphics.Blit (src , desc);
		}
	}
	 
}
