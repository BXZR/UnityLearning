using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class smogController : grphicsMade{

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

	private Camera theCamera;

	public Camera TheCamera 
	{
		get
		{
			if (theCamera == null) 
			{
				theCamera = GetComponent<Camera> ();
			}
			return theCamera;
		}
	}

	private Transform theTransform;
	public Transform TheTransform
	{
		get
		{
			if (theTransform == null)
				theTransform = this.transform;
			return theTransform;
		}
	}

	[Range (0.0f , 3.0f)]
	public float fogDensity = 1.0f;

	public Color fogColor = Color.white;
	public float fogStart = 0.0f;
	public float fogEnd = 2.0f;

	void OnEnable()
	{
		TheCamera.depthTextureMode |= DepthTextureMode.Depth;
	}

	void OnRenderImage( RenderTexture src , RenderTexture desc)
	{
		if (TheMaterial != null) 
		{
			Matrix4x4 fru = Matrix4x4.identity;

			float fov = TheCamera.fieldOfView;
			float near = TheCamera.nearClipPlane;
			float far = TheCamera.farClipPlane;
			float aspect = TheCamera.aspect;

			float halfHeight = near * Mathf.Tan (fov*0.5f * Mathf .Deg2Rad);
			Vector3 toRight = TheTransform.right * halfHeight * aspect;
			Vector3 toTop = TheTransform.up * halfHeight;

			Vector3 topLeft = TheTransform.forward * near + toTop - toRight;
			float scale = topLeft.magnitude / near;

			topLeft.Normalize ();
			topLeft *= scale;

			Vector3 topRight = TheTransform.forward * near + toRight + toTop;
			topRight.Normalize ();
			topRight *= scale;

			Vector3 buttonLeft = TheTransform.forward * near - toRight - toTop;
			buttonLeft.Normalize ();
			buttonLeft *= scale;

			Vector3 buttonRight = TheTransform.forward * near + toRight - toTop;
			buttonRight.Normalize ();
			buttonRight *= scale;

			fru.SetRow (0,buttonLeft);
			fru.SetRow (1,buttonRight);
			fru.SetRow (2, topRight);
			fru.SetRow (3, topLeft);

			TheMaterial.SetMatrix ("_FrustumCornersRay", fru);
			TheMaterial.SetMatrix ("_ViewProjectionInverseMatrix" , (TheCamera .projectionMatrix * TheCamera .worldToCameraMatrix).inverse);

			TheMaterial.SetFloat ("_fogDensity", fogDensity);
			TheMaterial.SetColor ("_fogColor" , fogColor);
			TheMaterial.SetFloat ("_fogStart" , fogStart);
			TheMaterial.SetFloat ("_fogEnd", fogEnd);

			Graphics.Blit (src , desc ,TheMaterial);

		} 
		else
		{
			Graphics.Blit (src ,desc );
		}
	}

}
