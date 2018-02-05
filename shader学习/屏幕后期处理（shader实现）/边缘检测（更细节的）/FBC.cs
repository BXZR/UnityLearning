using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBC : grphicsMade {

	public Shader theShader;
	private Material theMaterial;
	public Material TheMaterial
	{
		get
		{
			theMaterial = checkShaderAndCreateMaterial (theShader ,theMaterial);
			return theMaterial;
		}
	}

	[Range(0.0f,1.0f)]
	public float edgesOnly = 0.0f;
	public Color edgeColor = Color.magenta;
	public Color backgroundColor = Color.white;
	public float samplerDistance = 1.0f; //采样的距离
	public float sensitivityDepth = 1.0f;
	public float sensitivitiNormals = 1.0f;

	void OnEnable()
	{
		this.GetComponent <Camera> ().depthTextureMode |= DepthTextureMode.DepthNormals;
	}

	[ImageEffectOpaque]//只对不透明物体生效
	void OnRenderImage(RenderTexture src ,RenderTexture desc)
	{
		if (TheMaterial != null)
		{
			TheMaterial.SetFloat ("_EdgeOnly", edgesOnly);
			TheMaterial.SetColor ("_EdgeColor", edgeColor);
			TheMaterial.SetColor ("_BackgroundColor", backgroundColor);
			TheMaterial.SetFloat ("_SampleDistance", samplerDistance);
			TheMaterial.SetVector ("_Sensitivity", new Vector4 (sensitivitiNormals, sensitivityDepth, 0.0f, 0.0f));
			Graphics.Blit (src, desc, TheMaterial);
		} 
		else 
		{
			Graphics.Blit (src, desc);
		}
	}

}
