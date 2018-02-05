using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof (Camera))]//强制配合Camera脚本，如果没有Unity回自己加一个
public class grphicsMade : MonoBehaviour {


	protected void CheckResources()
	{
		if (CheckSupport () == false)
			NotSupported ();
	}


	protected bool CheckSupport ()
	{
        //直接检查系统标记看看是不是符合效果
		//SystemInfo.supportsRenderTextures在本版本中必定返回true
		if (SystemInfo.supportsImageEffects == false || SystemInfo.supportsRenderTextures == false) 
		{
			print ("不支持屏幕后期效果");
			return false;
		}
		print ("支持屏幕后期效果");
		return true;
	}

	protected void  NotSupported ()
	{
		this.enabled = false;//关掉这个脚本的整体计算效果
	}


	protected Material checkShaderAndCreateMaterial(Shader theShader , Material theMaterial)
	{
		if (theShader == null)
			return null;
		if (theShader.isSupported && theMaterial && theMaterial.shader == theShader)
			return theMaterial;//因为不需要做什么改变
		if (theShader.isSupported == false)
			return null;
		else 
		{
			theMaterial = new Material (theShader);
			theMaterial.hideFlags = HideFlags.DontSave;
//			if (theMaterial != null)
//				return theMaterial;
//			return null;
			//上面这几行实在是有点冗余，我暂时用下面的一行代替，如有问题再改回来
			return theMaterial;
		}
	}

	// Use this for initialization
	void Start () {
		CheckResources ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
