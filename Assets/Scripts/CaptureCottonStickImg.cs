using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class CaptureCottonStickImg : MonoBehaviour {

	public static  Texture2D cottonStickFinished;
	public static  Texture2D cottonStickToys;
	public static CaptureCottonStickImg Instance;

	public GameObject[] HideOnScreenCapture;
	public Image Background;

	public Transform DecorationHolder;
	public Transform ToysHolder;
 
	void Start () {
		Instance = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

 
	public static Texture2D Screenshot() {
		Instance.Background.enabled = false;
		foreach(GameObject go in Instance.HideOnScreenCapture)
		{
			go.SetActive(false);
		}

		int resWidth = Camera.main.pixelWidth;
		int resHeight = Camera.main.pixelHeight;

		int startPosY =  (int) (resHeight *.1f);


		Camera camera = Camera.main;
		RenderTexture rt = new RenderTexture(resWidth, resHeight, 32);
		camera.targetTexture = rt;
		Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.ARGB32, false);
		//Texture2D screenShot = new Texture2D(resWidth, resWidth, TextureFormat.ARGB32, false);
		camera.Render();
		RenderTexture.active = rt;
		screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
		//screenShot.ReadPixels(new Rect(0, startPosY , resWidth, resWidth), 0, 0);
		screenShot.Apply ();

	 
		//-----------test
		//byte[] bytes = screenShot.EncodeToPNG();
		//File.WriteAllBytes(Application.dataPath + "/../SavedScreen.png", bytes);
	 
	 
		cottonStickFinished = screenShot;


		foreach(Transform tr1 in Instance.DecorationHolder)
		{
			tr1.gameObject.SetActive(false);
		}
		Instance.ToysHolder.gameObject.SetActive(true);

		//scren igacaka
		rt = new RenderTexture(resWidth, resHeight, 32);
		camera.targetTexture = rt;
		screenShot = new Texture2D(resWidth, resHeight, TextureFormat.ARGB32, false);
		camera.Render();
		RenderTexture.active = rt;
		screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
		screenShot.Apply ();


		//-----------test
	 // bytes = screenShot.EncodeToPNG();
	//	 File.WriteAllBytes(Application.dataPath + "/../SavedScreen2.png", bytes);
		 


		cottonStickToys = screenShot;

		camera.targetTexture = null;
		RenderTexture.active = null;  

		Destroy(rt);

		foreach(Transform tr1 in Instance.DecorationHolder)
		{
			tr1.gameObject.SetActive(true);
		}

		foreach(GameObject go in Instance.HideOnScreenCapture)
		{
			go.SetActive(true);
		}
		Instance.Background.enabled = true;
 
		return screenShot;
	}
	 
}
