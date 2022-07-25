using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
 
public class TattooMachine : MonoBehaviour, IBeginDragHandler , IDragHandler, IEndDragHandler
{

	public  bool bEnableDrag = false;
	public bool bDrag = false;
	float x;
	float y;
	Vector3 diffPos = new Vector3(0,0,0);
	Vector3 diffNeedlePosScreenSpace; 
	int mouseFollowSpeed = 10; 

	public TattooSalonGameplay gameplay;
	public ProgressBar progressBar;

	public GameObject BloodPref;
	public Transform BloodHolder;
	public Transform TestPointNeedle;
	Vector3 TestPointNeedle_StartPosition = Vector3.zero;
	Vector3 TestPointNeedle_DisabledPosition = new Vector3(10000,10000,0);


	int startStenicilDrawPoints = 0;

	public GraphicRaycaster graphicRaycaster;
	public  RawImage StencilRawImage;

	public Texture2D brushNeedle;

	Animator[] animBloodDrops;
	int bloodDropsRandomCount = 0;
	int bloodDropsCount = 0;

//	int stencilTexWidth = 0;
	//int stencilTexHeight = 0;
	int stencilTexSize = 0;

	int brushTexSize = 48;// 48;// 32; //16 //24
	int brushHalfTexSize = 24; //8 //12
	float[] brushAlpha;
	Color[] colorEdit;
	bool[] bBrushAlpha;


	bool[,] textureProgress;

	public Vector3 startPosition;

	public  Animator animLaser;

	void Start () 
	{

		brushAlpha = new float[brushTexSize*brushTexSize];
		colorEdit =  new Color[brushTexSize*brushTexSize];
		bBrushAlpha =  new bool[brushTexSize*brushTexSize];
		Color[] tmp = brushNeedle.GetPixels();
		for (int i = 0; i < tmp.Length; i++) 
		{
			brushAlpha[i] = tmp[i].a; 
			bBrushAlpha[i] = tmp[i].a>0.1f;
//			Debug.Log(	brushAlpha[i] );
		}

		startPosition =transform.position;
		TestPointNeedle_StartPosition = TestPointNeedle.localPosition;
	}
	

	void Update () {
		if( bDrag  && bEnableDrag)  
		{
			x = Input.mousePosition.x;
			y = Input.mousePosition.y;
			
			Vector3 posM = Camera.main.ScreenToWorldPoint(new Vector3(x ,y,10.0f) )  + diffPos;
//			if(posM.x<-2.5f  ) posM = new Vector3(-2.5f,posM.y,posM.z);
//			else if(posM.x>2.5f  ) posM = new Vector3(2.5f,posM.y,posM.z);
//			
//			if(posM.y<-5.5f  ) posM = new Vector3(posM.x,-5.5f, posM.z);
 			 if(posM.y>2.2f) posM = new Vector3(posM.x,2.2f,posM.z);
			transform.position =  Vector3.Lerp (transform.position, posM  , mouseFollowSpeed * Time.deltaTime)  ;

			//PointerEventData cursor = new PointerEventData(EventSystem.current);  
			//OnDrag1(cursor);
		}
	}

 

	public void OnBeginDrag (PointerEventData eventData)
	{
		 
		if(!bEnableDrag) return;
		if( gameplay.Phase == 7 && !bDrag   ) //pojacavanje tetovaze preko sablona
		{
			bDrag = true;
			diffPos =transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition)   ;
			diffPos = new Vector3(diffPos.x,diffPos.y,0);

			diffNeedlePosScreenSpace = Camera.main.WorldToScreenPoint(TestPointNeedle.position) - Input.mousePosition;
		 
			InvokeRepeating( "TestFirstPassTextureProgress",0.2f,0.2f);

			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.TattooMachine);
		}
		 
		

		if( gameplay.Phase == 9 && !bDrag   ) //bojenje tetpovaze amp
		{
			TestPointNeedle.localPosition = TestPointNeedle_StartPosition;
			bDrag = true;
			diffPos =transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition)   ;
			diffPos = new Vector3(diffPos.x,diffPos.y,0);

			diffNeedlePosScreenSpace = Camera.main.WorldToScreenPoint(TestPointNeedle.position) - Input.mousePosition;
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.TattooMachine);

			if(!gameplay.paintEngine.drawEnabled)
			{
				gameplay.paintEngine.drawEnabled=true;
				//gameplay.paintEngine.set
			}
		}

		if( gameplay.Phase == 11 && !bDrag   ) //brisanje tetovaze amp
		{
			TestPointNeedle.localPosition = TestPointNeedle_StartPosition;

			animLaser.Play("LaserOn");
			bDrag = true;
			diffPos =transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition)   ;
			diffPos = new Vector3(diffPos.x,diffPos.y,0);

			diffNeedlePosScreenSpace = Camera.main.WorldToScreenPoint(TestPointNeedle.position) - Input.mousePosition;
			gameplay.InvokeRepeating("TestErasedTattoo",0,.5f);
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.Laser);
		}
	}
	
	public void OnEndDrag (PointerEventData eventData)
	{
		bDrag = false;
		CancelInvoke( "TestFirstPassTextureProgress");

		if( gameplay.Phase == 11 ) 
		{
			animLaser.Play("LaserOff");
			gameplay.CancelInvoke("TestErasedTattoo");
 
		}
		if( gameplay.Phase == 9 || gameplay.Phase == 11 ) 
		{
		   TestPointNeedle.localPosition = TestPointNeedle_DisabledPosition;
		 
		}
		if(SoundManager.Instance!=null)
		{
			SoundManager.Instance.Stop_Sound( SoundManager.Instance.Laser);
		 	SoundManager.Instance.Stop_Sound( SoundManager.Instance.TattooMachine);
		}
	}



	public void OnDrag (PointerEventData eventData)
	{
		if( gameplay.Phase == 7 && bDrag  && bEnableDrag  )  
		{
			eventData.position = Input.mousePosition + diffNeedlePosScreenSpace;
			List<RaycastResult> objectsHit = new List<RaycastResult> ();
			graphicRaycaster.Raycast(eventData, objectsHit);
			int count = objectsHit.Count;

			foreach(RaycastResult r in objectsHit)
			{
				if(r.gameObject.name == "Stencil")
				{
	 
					Vector2 localPosition = ((Vector2) StencilRawImage.transform.InverseTransformPoint(TestPointNeedle.position)) ;//   eventData.position);
					int px = Mathf.FloorToInt(localPosition.x)+128;
					int py = Mathf.FloorToInt(localPosition.y)+128; 

					if(px>255) px= 255;
					else if(px<0) px= 0;

					if(py>255) py = 255;
					else if(py<0) py = 0;

					Color color = (StencilRawImage.texture as Texture2D).GetPixel ( px,py );

					//Debug.Log(r.gameObject.name + "   ,c: "+ color + "             tex pos:( "+ px+", "+py  + " )  " );


					DrawTattoo(px,py);

				}
	 
			}
		}
		else if( gameplay.Phase == 9 &&  bDrag  && bEnableDrag )  
		{
			 gameplay.paintEngine.ImmediateDraw( TestPointNeedle);
		}

//		else if( gameplay.Phase == 11 &&  bDrag  && bEnableDrag )  
//		{
//			 
//		}
	}





	void DrawTattoo(int PX, int PY)
	{
		int xStart = PX-brushHalfTexSize;
		int width = brushTexSize;
		if(xStart <0)
		{
			width = brushTexSize + xStart;
			xStart = 0;
		}
		else  if(xStart >255-brushTexSize)
        {
			width = 255 - xStart;
		}


		int yStart = PY-brushHalfTexSize;
		int height = brushTexSize;
		if(yStart <0)
		{
			height = brushTexSize + yStart;
			yStart = 0;
		}
		else  if(yStart >255-brushTexSize)
		{
			height = 255 - yStart;
		}




		colorEdit =  (StencilRawImage.texture as Texture2D).GetPixels( xStart, yStart,width,height);
		float alpha;
		int testI = xStart;
		int testJ = yStart;
		int testPos = 0; 
		int testPos2 = 0;
		for (int i = 0; i < colorEdit.Length; i++) 
		{

			alpha = colorEdit[i].a;
			//if(alpha >0.39f )
			//{
				//alpha  = alpha+ alpha* (1.5f  * brushAlpha[i]) ;
				//if(alpha >1) alpha = 1;
			testPos2 = testI+testJ*256;
			if(bBrushAlpha[i] && !textureProgress[ testPos2, 1] )  
			{
				colorEdit[i].a  *= 2.5f;
				textureProgress[ testPos2, 1] = true;
			}
 
			testPos ++;
			if(testPos == width) 
			{
				testPos = 0;
				testJ++;
			}
			testI = xStart+testPos;
		}

		/*  radi
		for (int i = 0; i < colorEdit.Length; i++) 
		{
			alpha = colorEdit[i].a;
			if(alpha >0.39f )
			{
				//alpha  = alpha+ alpha* (1.5f  * brushAlpha[i]) ;
				//if(alpha >1) alpha = 1;

				if(bBrushAlpha[i]) colorEdit[i].a  *= 2.5f;

				//colorEdit[i].a = alpha;
				 
				 
			}
		}
		   */                                                                                   
		(StencilRawImage.texture as Texture2D).SetPixels( xStart, yStart,width,height, colorEdit);
		(StencilRawImage.texture as Texture2D).Apply();
	}




	public void SetFirstPassTextureProgressData()
	{

		//stencilTexWidth =   StencilRawImage.texture.width;
		//stencilTexHeight =   StencilRawImage.texture.height;

		//stencilTexSize =  StencilRawImage.texture.width *  StencilRawImage.texture.height;

		Color[] tmp  =   (StencilRawImage.texture as Texture2D).GetPixels();
		stencilTexSize = tmp.Length;
		textureProgress = new bool[stencilTexSize,2];
		for (int i = 0; i < stencilTexSize; i++) 
		{
			if(tmp[i].a > 0.1f)
			{
				textureProgress[i,0] = true; //potrebno je ptomeniti alpha vrednos piksela
				startStenicilDrawPoints++;
			}
			else textureProgress[i,0] = false;

			textureProgress[i,1] = false; //  alpha vrednost piksela nije promenjena
		}

		bloodDropsRandomCount = Random.Range(5,11);
		animBloodDrops = new Animator[bloodDropsRandomCount];
	 
		bloodCreateStep = Mathf.FloorToInt(startStenicilDrawPoints/(float)bloodDropsRandomCount)  ;
	//	Debug.Log("START COUNT:" + startStenicilDrawPoints + "   RandomCount:" + bloodDropsRandomCount + "  step:" + bloodCreateStep);
	}

	int bloodCreateStep;
	void TestFirstPassTextureProgress()
	{
		int pixelsLeft = 0;
		for (int i = 0; i < stencilTexSize; i++) 
		{
			if(textureProgress[i,0]  && 	!textureProgress[i,1] ) pixelsLeft++;
		}

		 
		progressBar.SetProgress( 1- (pixelsLeft/(float)startStenicilDrawPoints));

		if(pixelsLeft < startStenicilDrawPoints*.001f)   //*.008f //100
		{
			progressBar.SetProgress(1);
// 			Debug.Log("end phase 7");
	 
			if(SoundManager.Instance!=null) SoundManager.Instance.Stop_Sound( SoundManager.Instance.TattooMachine);
			gameplay.EraseBloodPhase();
			//gameplay. StencilFirstPassDone();
			CancelInvoke("TestFirstPassTextureProgress");
			bEnableDrag = false;
			bDrag = false;

 
		}

		//kreiranje kapljica krvi
		//bloodDropsRandomCount
		if( (bloodDropsRandomCount- bloodDropsCount-1) *bloodCreateStep > pixelsLeft)
		{

			GameObject go = GameObject.Instantiate(BloodPref);
			go.transform.SetParent(BloodHolder);
			go.transform.position = TestPointNeedle.position;
			go.transform.localScale = Vector3.one;

			animBloodDrops[bloodDropsCount] = go.GetComponent<Animator>();

			bloodDropsCount++;
		}
		   
	}


	public void HideBlood()
	{
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.Cotton);
		for (int i = 0; i < bloodDropsCount; i++) {
			 animBloodDrops[ i ].Play("hide");
		}
		GameObject.Destroy( BloodHolder.gameObject,3);

	}

 

	public void SetLaser()
	{
		transform.GetChild(1) .gameObject.SetActive(false);
		transform.GetChild(2) .gameObject.SetActive(false);
		transform.GetChild(3) .gameObject.SetActive(false);
		transform.GetChild(4) .gameObject.SetActive(true); //.GetComponent<Animator>
		animLaser.Play("LaserOff");
		if(SoundManager.Instance!=null) SoundManager.Instance.Stop_Sound( SoundManager.Instance.Laser);
		if(SoundManager.Instance!=null) SoundManager.Instance.Stop_Sound( SoundManager.Instance.TattooMachine);
	}
 



	bool appFoucs = true;
	void OnApplicationFocus( bool hasFocus )
	{
		if(  !appFoucs && hasFocus )
		{
			if(  bDrag )
			{
				bDrag = false;
				
				CancelInvoke("TestDistance");
				if(SoundManager.Instance!=null) SoundManager.Instance.Stop_Sound( SoundManager.Instance.Laser);
				if(SoundManager.Instance!=null) SoundManager.Instance.Stop_Sound( SoundManager.Instance.TattooMachine);
				//StartCoroutine("MoveBack" );
			}
		}
		appFoucs = hasFocus;
		
	}

 
}
