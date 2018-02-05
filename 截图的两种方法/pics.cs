using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class pics : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}


	void makePictureBGasic()
	{
		Application.CaptureScreenshot ("Screenshot.png");
	}

	IEnumerator OnScreenCapture ()
   {
     yield return new WaitForEndOfFrame();//等待这一帧画完了才能截图
	 try
	 {
        int width = Screen.width;
        int height = Screen.height;
		Texture2D tex = new Texture2D ( width, height, TextureFormat.RGB24, false);//新建一张图
		tex.ReadPixels (new Rect (0, 0, width, height), 0, 0, true);//从屏幕开始读点
		byte[] imagebytes = tex.EncodeToJPG ();//用的是JPG(这种比较小)
		//使用它压缩实时产生的纹理，压缩过的纹理使用更少的显存并可以更快的被渲染
		//通过true为highQuality参数将抖动压缩期间源纹理，这有助于减少压缩伪像
		//因为压缩后的图像不作为纹理使用，只是一张用于展示的图
		//但稍微慢一些这个小功能暂时貌似还用不到
			tex.Compress (false);
			tex.Apply();
			Texture2D mScreenShotImgae = tex;
			File.WriteAllBytes ( @"E:\Screenshot.png", imagebytes);


		}
		catch (System.Exception e)
		{
			Debug.Log ("ScreenCaptrueError:" + e);
		}

	}



	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space))
			makePictureBGasic ();
		if(Input .GetMouseButtonDown(1))
		StartCoroutine (OnScreenCapture());
	}
}
