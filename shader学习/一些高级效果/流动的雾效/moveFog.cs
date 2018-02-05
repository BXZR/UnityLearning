using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveFog : grphicsMade
{
	public Shader fogShader;
	private Material fogMaterial = null;
	public Material FogMaterial
	{
		get
		{
			fogMaterial = checkShaderAndCreateMaterial (fogShader , fogMaterial);
			return fogMaterial;
		}
	}

	private Camera myCamera = null;
	public Camera theCamera
	{
		get
		{
			if (myCamera == null)
				myCamera = this.GetComponent <Camera> ();
			return myCamera;
		}
	}

	private Transform myCameraTransform = null;
	public Transform cameraTransform
	{
		get
		{
			if (myCameraTransform == null)
				myCameraTransform = this.GetComponent <Transform> ();
			return myCameraTransform;
		}

	}

	[Range (0.1f, 3.0f)]
	public float fogDesity = 1.0f;
	public Color fogColor = Color.white;
	public float fogStart = 0.0f;
	public float fogEnd = 2.0f;

	public Texture noiseTexture;
	[Range(-0.5f,0.5f)]
	public float fogXSpeed = 0.1f;
	[Range(-0.5f,0.5f)]
	public float fogYSpeed = 0.1f;

	[Range(0.0f,3.0f)]
	public float noiseAmount = 1.0f;

	void OnEnable()
	{
		theCamera.depthTextureMode |= DepthTextureMode.Depth;
	}

	void OnRenderImage(RenderTexture src , RenderTexture desc )
	{
		if (FogMaterial != null) {
			Matrix4x4 frustumCorners = Matrix4x4.identity;
			//计算金剪裁平面的四个角对应的向量

			float fov = theCamera.fieldOfView;
			float near = theCamera.nearClipPlane;
			float aspect = theCamera.aspect;

			float halfHeight = near * Mathf.Tan (fov * 0.5f * Mathf.Deg2Rad);
			Vector3 toRight = cameraTransform.right * halfHeight * aspect;
			Vector3 toTop = cameraTransform.up * halfHeight;

			Vector3 topLeft = cameraTransform.forward * near + toTop - toRight;
			float scale = topLeft.magnitude / near;

			topLeft.Normalize ();
			topLeft *= scale;

			Vector3 topRight = cameraTransform.forward * near + toRight + toTop;
			topRight.Normalize ();
			topRight *= scale;

			Vector3 bottomLeft = cameraTransform.forward * near - toTop - toRight;
			bottomLeft.Normalize ();
			bottomLeft *= scale;

			Vector3 bottomRight = cameraTransform.forward * near + toRight - toTop;
			bottomRight.Normalize ();
			bottomRight *= scale;

			frustumCorners.SetRow (0, bottomLeft);
			frustumCorners.SetRow (1, bottomRight);
			frustumCorners.SetRow (2, topRight);
			frustumCorners.SetRow (3, topLeft);

			/////
			FogMaterial.SetMatrix("_FrustumCornersRay", frustumCorners);
			FogMaterial.SetFloat ("_FogDensity",  fogDesity);
			FogMaterial.SetColor ("_FogColor", fogColor);
			FogMaterial.SetFloat ("_FogStart", fogStart);
			FogMaterial.SetFloat ("_FogEnd", fogEnd);
			FogMaterial.SetTexture ("_NoiseTex", noiseTexture);
			FogMaterial.SetFloat ("_FogXSpeed", fogXSpeed);
			FogMaterial.SetFloat ("_FogYSpeed", fogYSpeed);
			FogMaterial.SetFloat ("_NoiseAmount", noiseAmount);

			Graphics.Blit (src, desc, FogMaterial);
		}
		else
		{
			Graphics.Blit(src, desc);
		}
	}


}
