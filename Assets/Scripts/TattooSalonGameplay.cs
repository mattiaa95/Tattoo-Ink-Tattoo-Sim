using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using AdvancedMobilePaint;
using System.Collections.Generic;

public class TattooSalonGameplay : MonoBehaviour {

	public GameObject PupUpMoreGames;
	public MenuManager menuManager;

	public AdvancedMobilePaint.AdvancedMobilePaint paintEngine;
	public Texture2D inkBrush;
	public Texture2D eraserBrush;

	Texture2D stencilTex;

	Texture2D paintSurface;
	Texture2D maskTexture;
 
	public CanvasGroup stencilHolder;
	public ParticleSystem psTattoo;
	public RawImage stencil;
	public Animator animStencilPaper;

	public Animator ButtonsHolderAnim;//tetovaze
	public Animator InkHolderAnim; //boje
	public Animator animTutorial;

	public Animator ButtonsHolderAnim2;//tufer i krema

	public int Phase = 0;

	public ProgressBar progressBar;
	public Transform [] TutorialPositions;

	public Transform tattooMachineRaySource;
	public Image tattooMachineInkColor;
	public Image tattooMachineInkBottle;

//	public RectTransform AMPRectTransform;
//	public RectTransform StencilRectTransform;

	public TattooMachine tattooMachine;
 


	//PODESAVANJA LOKACIJA TETOVAZA
	public GameObject[] tattooLocationBG;
	public Vector2[] tattooStencilPostitons;
	public Vector3[] tattooStencilScale;
	public float[] tattooStencilRotations;

	public Color[] modelColor;

	int locationIndex = 0;

	bool bEraseTattoo = false;
	public Transform ButtonEraseTattoo;
	public Transform ButtonNext;
	public Transform TattooRemoveFluid;

	IEnumerator Start () {
		ButtonEraseTattoo.gameObject.SetActive(false);
		ButtonNext.gameObject.SetActive( false);


		tattooMachine.gameObject.SetActive(false);
		progressBar.gameObject.SetActive(false);
 
		paintEngine.transform.gameObject.SetActive(false);

		stencilHolder.gameObject.SetActive(false);
		stencilHolder.alpha = 0;
		psTattoo.gameObject.SetActive(false);

		ButtonsHolderAnim.SetBool("bShow",true);
		InkHolderAnim.gameObject.SetActive(false);
		tattooMachineInkBottle.gameObject.SetActive(false);
		tattooMachineInkColor.gameObject.SetActive(false);

		//AMPRectTransform = paintEngine.GetComponent<RectTransform>();
		//StencilRectTransform = stencil.GetComponent<RectTransform>();

 		// GameData.selectedModel =1;
 		// GameData.selectedLocation = 1;

		SetScene();

		yield return new WaitForSeconds(.5f);
		LevelTransition.Instance.ShowScene();
		BlockClicks.Instance.SetBlockAll(true);
		BlockClicks.Instance.SetBlockAllDelay(1f,false);
	}

	void SetScene()
	{


		if(GameData.selectedModel == 0 || GameData.selectedModel == 2) 
		{
			//WOMAN
			if(GameData.selectedLocation == 1) locationIndex = 0;
			else if(GameData.selectedLocation == 3) locationIndex = 1;
			else if(GameData.selectedLocation == 4) locationIndex = 4;
			else if(GameData.selectedLocation == 2) locationIndex = 5;

		}
		else
		{
			//MAN
			if(GameData.selectedLocation == 1) locationIndex = 2;
			else if(GameData.selectedLocation == 3) locationIndex = 3;
			else if(GameData.selectedLocation == 4) locationIndex = 6;
			else if(GameData.selectedLocation == 2) locationIndex = 7;
		}

		for (int i = 0; i < tattooLocationBG.Length; i++) 
		{
			tattooLocationBG[i].SetActive(locationIndex == i);
		}
		tattooLocationBG[locationIndex].GetComponent<Image>().color = modelColor[(GameData.selectedModel)];
		 
		animStencilPaper.transform.localScale = tattooStencilScale[locationIndex];
		stencilHolder.GetComponent<RectTransform>().anchoredPosition = tattooStencilPostitons[locationIndex];
		animStencilPaper.transform.localRotation  =  Quaternion.Euler(0, 0,  tattooStencilRotations[locationIndex]);
		 

		//podesavanje rotacije kreme
		TattooRemoveFluid.localRotation = animStencilPaper.transform.localRotation;
		TattooRemoveFluid.localScale = tattooStencilScale[locationIndex];
		 
	}
	
	 
	void Update () {
		if(Phase ==1 && Input.GetMouseButtonDown(0))
		{
			 
			animTutorial.Play("default");//gameObject.SetActive(false);
			Phase = 2;
		}



	}


	//odabrana je tetvaza

	public void ShowStencil(Sprite spr, int stencilIndex)
	{
		ButtonsHolderAnim.SetBool("bShow",false);

		paintSurface =  AdvancedMobilePaint.PaintUtils.ConvertSpriteToTexture2D(spr);

		AdjustStencilRotationAndSize(stencilIndex);


		StartCoroutine("WShowStencil" );


		 
	}

	IEnumerator WShowStencil()
	{
		BlockClicks.Instance.SetBlockAll(true);
		psTattoo.transform.position = stencil.transform.position;
		psTattoo.gameObject.SetActive(true);
		psTattoo.Play();
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.ParticlesSound);
		yield return new WaitForFixedUpdate();
		 
		stencilTex =  GenerateStencilTexture2D(paintSurface);
		yield return new WaitForFixedUpdate(); 
		maskTexture = GenerateMaskTexture2D(paintSurface);
 

		stencil.texture = stencilTex;



	
		// BlockClicks.Instance.SetBlockAllDelay(4.5f,false);

		stencilHolder.gameObject.SetActive(true);
		stencilHolder.alpha = 0;

		float timeA = 0;
		while(timeA<1)
		{
			yield return new WaitForFixedUpdate();
			timeA += Time.fixedDeltaTime*4;
			stencilHolder.alpha = timeA;
		}
		stencilHolder.alpha = 1;

		yield return new WaitForSeconds(1.0f);
		psTattoo.Stop();
		psTattoo.gameObject.SetActive(false);
		BlockClicks.Instance.SetBlockAll(false);

		//prikazati tutorijal
		yield return new WaitForSeconds(.2f);
		animTutorial.transform.position = TutorialPositions[0].position;
		animTutorial.Play("HandPointerApplyStencil");
		Phase = 1;
		progressBar.gameObject.SetActive(true);
		progressBar.SetProgress(0);

		ButtonsHolderAnim.gameObject.SetActive(false);

	}
 


	public void StencilApplied()
	{
		Phase = 4;
		animTutorial.transform.position = TutorialPositions[1].position;
		animTutorial.gameObject.SetActive(true);
		animTutorial.Play("HandPointerPeelOffStencil");
		animStencilPaper.Play ("WaitToPeel");

		progressBar.gameObject.SetActive(false);
	}
		
	public void StencilPelledOff()
	{
		Phase = 6;
		 
		StartCoroutine("WStencilPelledOff");
	}

	IEnumerator WStencilPelledOff()
	{
		progressBar.gameObject.SetActive(true);
		progressBar.SetProgress(0);
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.RemoveStencil);

		BlockClicks.Instance.SetBlockAll(true);

		psTattoo.transform.position = stencil.transform.position;
		psTattoo.gameObject.SetActive(true);
		psTattoo.Play();
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.ParticlesSound);
		yield return new WaitForSeconds(2);
	 
		GameObject.Destroy( animStencilPaper.gameObject);

		psTattoo.Stop();
		psTattoo.gameObject.SetActive(false);
		BlockClicks.Instance.SetBlockAll(false);
		 
 
		tattooMachine.SetFirstPassTextureProgressData();
		tattooMachine.gameObject.SetActive(true);
		Phase = 7;
		tattooMachine.bEnableDrag = true;
	}






	public void  EraseBloodPhase()
	{
		tattooMachine.gameObject.SetActive(false);
		ButtonsHolderAnim2.gameObject.SetActive(true);
		ButtonsHolderAnim2.SetBool("bShow",true);
		
		ButtonsHolderAnim2.transform.GetChild(1).gameObject.SetActive(false);
		ButtonsHolderAnim2.transform.GetChild(2).gameObject.SetActive(false);
		ButtonsHolderAnim2.transform.GetChild(3).gameObject.SetActive(false); 
		ButtonsHolderAnim2.transform.GetChild(4).gameObject.SetActive(true);
	}


	public void StencilFirstPassDone()
	{
		ButtonsHolderAnim2.transform.GetChild(4).gameObject.SetActive(false);
		psTattoo.gameObject.SetActive(true);
		psTattoo.Play();
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.ParticlesSound);

		StartCoroutine("WShowAMP");

		Phase = 8;
		 //tattooMachine.gameObject.SetActive(false);

	}


	IEnumerator WShowAMP()
	{
		yield return new WaitForSeconds(1);
		progressBar.gameObject.SetActive(false);

		//---------aktiviranje AMP--------------------------------------

		paintEngine.transform.gameObject.SetActive(true);

		paintEngine.transform.localScale =   stencil.transform.localScale;
		paintEngine.transform.localRotation =  stencil.transform.localRotation;
		paintEngine.transform.position = stencil.transform.position;
 
		//stencil.enabled = false; 

		//Debug.Log("ZAMENA SLIKE ZA STENCIL DA BI SE OCUVALI TAMNI DETALJI");
		stencil.gameObject.AddComponent<GraphicIgnoreRaycast>();
		stencil.texture = GenerateStencil_BlackDetailsOverlayTexture2D(paintSurface);
		
//		Debug.Log("AMP");
	//-----------------------------------
		paintEngine.undoEnabled = false;
		paintEngine.multitouchEnabled = false;
		paintEngine.InitializeEverything();

		//crta se vrhom igle
		paintEngine.useAlternativeRay = true;
		paintEngine.raySource = tattooMachineRaySource;

 
		paintEngine.SetDrawingTexture( paintSurface);
	 
	 
 
		paintEngine.SetBitmapBrush(inkBrush,AdvancedMobilePaint.BrushProperties.Default,true,false,Color.red,true,true,null);

		//paintEngine.drawMode=AdvancedMobilePaint.DrawMode.CustomBrush;
		paintEngine.brushAlphaStrength = 0.01f;


 
  		 paintEngine.useLockArea=false;
//		paintEngine.useMaskLayerOnly=true;
//		paintEngine.useMaskImage = true;


		paintEngine.maskTex = maskTexture;
		paintEngine.SetDrawingMask( maskTexture );

		paintEngine.SetMaskTextureMode();
		//paintEngine.ReadMaskImage();
		 
		 

		SetPixelsAMPFromTexAlpha(paintSurface);
	 
		yield return new WaitForSeconds(1);
		//  paintEngine.drawEnabled=true;
		Debug.Log("paintEngine drawEnabled");

		psTattoo.Stop();
		psTattoo.gameObject.SetActive(false);

		tattooMachine.bEnableDrag = true;
		tattooMachine.gameObject.SetActive(true);

		tattooMachineInkBottle.gameObject.SetActive(true);
		tattooMachineInkColor.gameObject.SetActive(true);


		tattooMachine.transform.position = tattooMachine.startPosition;
		Phase = 9;
		paintEngine.CreateAreaLockMask( tX,tY );

		InkHolderAnim.gameObject.SetActive(true);
		InkHolderAnim.SetBool("bShow",true);

		ButtonEraseTattoo.gameObject.SetActive(true);
		ButtonNext.gameObject.SetActive(true);


	}
 
	void SetPixelsAMPFromTexAlpha( Texture2D texOriginal )
	{
		int pix = 0;
		Color[] tmp = texOriginal.GetPixels(); 
		//		 Debug.Log(tmp.Length);
		for (int i = 0; i < tmp.Length; i++) 
		{
			paintEngine.pixels[pix] =  (byte)(tmp[i].r*255);
			paintEngine.pixels[pix+1] =  (byte)(tmp[i].g*255);
			paintEngine.pixels[pix+2] =  (byte)(tmp[i].b*255);

			if( (tmp[i].r +  tmp[i].g +  tmp[i].b)<.5f) paintEngine.pixels[pix+3] =  (byte)(tmp[i].a*255*.6);
			else paintEngine.pixels[pix+3] =  (byte)(tmp[i].a*255);
			 
			pix+=4;
		}
		paintEngine.textureNeedsUpdate = true;
		paintEngine.UpdateTexture();
	}

	void SetPixelsAMPFromTex( Texture2D texOriginal )
	{
		int pix = 0;
		Color[] tmp = texOriginal.GetPixels(); 
		//		 Debug.Log(tmp.Length);
		for (int i = 0; i < tmp.Length; i++) 
		{
			paintEngine.pixels[pix] =  (byte)(tmp[i].r*255);
			paintEngine.pixels[pix+1] =  (byte)(tmp[i].g*255);
			paintEngine.pixels[pix+2] =  (byte)(tmp[i].b*255);
			paintEngine.pixels[pix+3] =  (byte)(tmp[i].a*255);
			
			pix+=4;
		}
		paintEngine.textureNeedsUpdate = true;
		paintEngine.UpdateTexture();
	}

	public void SetInk( Color inkColor)
	{
		paintEngine.CreateAreaLockMask( tX,tY  );
		//Debug.Log(inkColor);
		paintEngine.SetBitmapBrush(inkBrush,AdvancedMobilePaint.BrushProperties.Default,true,false,inkColor,true,true,null);
		tattooMachineInkColor.color = inkColor;
	 
	 
	}

 


 
	public  Texture2D GenerateStencilTexture2D(Texture2D texOriginal)
	{
		Texture2D tex1 = new Texture2D (texOriginal.width,texOriginal.height, TextureFormat.ARGB32, false);
		Color[] tmp = texOriginal.GetPixels(); 
//		 Debug.Log(tmp.Length);
		for (int i = 0; i < tmp.Length; i++) 
		{
			tmp[i].a *= .4f; 
		}
		
		tex1.SetPixels( tmp,0 );
		tex1.Apply (false, false);
		return tex1;
	}


	public  Texture2D GenerateStencil_BlackDetailsOverlayTexture2D(Texture2D texOriginal)
	{
		Texture2D tex1 = new Texture2D (texOriginal.width,texOriginal.height, TextureFormat.ARGB32, false);
		Color[] tmp = texOriginal.GetPixels(); 
		//		 Debug.Log(tmp.Length);
		for (int i = 0; i < tmp.Length; i++) 
		{
			if(tmp[i].a < .5f) tmp[i] = new Color(0,0,0,tmp[i].a);
		 	else
			{
				//generisanje alfa vrednosit na osnovu sadrzaja crne boje
				float a = 1-(tmp[i].r +  tmp[i].g +  tmp[i].b)/ 3f ;
				tmp[i] = new Color(0,0,0,a*.7f);
			}
			 
		}
		
		tex1.SetPixels( tmp,0 );
		tex1.Apply (false, false);
		return tex1;
	}



	public  Texture2D CombineImages(Texture2D texA, Texture2D texB)
	{
		//prva je tekstura iz paint engine a druga stencil
		if(texA.width != texB.width || texA.height != texB.height) return null;

		Texture2D tex1 = new Texture2D (texA.width,texA.height, TextureFormat.ARGB32, false);
		Color[] tmp1 = texA.GetPixels(); 
		Color[] tmp2 = texB.GetPixels(); 
		Color point = new Color(0,0,0,0);
		for (int i = 0; i < tmp1.Length; i++) 
		{
			point = Color.Lerp(tmp1[i],tmp2[i],tmp2[i].a);
			point.a = tmp1[i].a;
			if(  ((tmp1[i].r +  tmp1[i].g +  tmp1[i].b)< 1) && tmp1[i].a >.3f)  point.a =1;
			tmp1[i] = point;
 
		}
		
		tex1.SetPixels( tmp1,0 );
		tex1.Apply (false, false);
		return tex1;
		
	}






	public  Texture2D GenerateMaskTexture2D(Texture2D texOriginal)
	{
		Color transparent = new Color(1,1,1,1);
		Texture2D tex1 = new Texture2D (texOriginal.width,texOriginal.height, TextureFormat.ARGB32, false);
		Color[] tmp = texOriginal.GetPixels(); 

		for (int i = 0; i < tmp.Length; i++) 
		{

			if(tmp[i].a<.95f)
			{
				tmp[i] = Color.black;

			}
			else
			{
//				if((tmp[i].r+tmp[i].g+tmp[i].b) < .9f)  
//				{
//					tmp[i] = Color.black;
//				}
//				else
				{
					tmp[i] = transparent;
					if(tY<0) 
					{
							tY= Mathf.FloorToInt( i/256);
							tX = i%256;
					} 
				}
			}
		}

		tex1.SetPixels( tmp,0 );
		tex1.Apply (false, false);
		return tex1;
	}


	int tX = -1;
	int tY = -1;

	public void SetEraser()
	{
		paintEngine.CreateAreaLockMask( tX,tY  );//CreateAreaLockMask(int x, int y)

		paintEngine.clearColor = new Color32(0,0,0,0);
		paintEngine.SetBitmapBrush(eraserBrush,AdvancedMobilePaint.BrushProperties.Clear,false,true,paintEngine.clearColor ,false,true,null);
	}


	public void ButtonEraseTattooClicked()
	{
		BlockClicks.Instance.SetBlockAll(true);
		 
		if(!bEraseTattoo)
		{
			StartCoroutine("ShowEraseTattoo");

			paintEngine.drawEnabled = false;
			InkHolderAnim.SetBool("bShow",false);
			GameObject.Destroy(InkHolderAnim.gameObject,2);

			//SetEraser();
			bEraseTattoo = true;
			Phase = 10;

		}
		else
		{
			LevelTransition.Instance.HideSceneAndLoadNext("TattooSalon") ;
		}
		if(SoundManager.Instance!=null)
		{
			SoundManager.Instance.Play_ButtonClick();
		 	SoundManager.Instance.Stop_Sound( SoundManager.Instance.Laser);
			SoundManager.Instance.Stop_Sound( SoundManager.Instance.TattooMachine);
		}
	}

	IEnumerator ShowEraseTattoo()
	{
		LevelTransition.Instance.HideAndShowSceneWithoutLoading();

 
		yield return new WaitForSeconds(1.0f);
		tattooMachine.gameObject.SetActive(false);

		stencil.enabled = false;
		SetPixelsAMPFromTex(  CombineImages(  paintEngine.tex, (Texture2D) stencil.texture)) ;



		ButtonEraseTattoo.GetChild(0).gameObject.SetActive(false);
		ButtonEraseTattoo.GetChild(1).gameObject.SetActive(true);
		ButtonEraseTattoo.gameObject.SetActive(false);
		yield return new WaitForSeconds(1.0f);
		ButtonsHolderAnim2.gameObject.SetActive(true);
		ButtonsHolderAnim2.SetBool("bShow",true);

		ButtonsHolderAnim2.transform.GetChild(1).gameObject.SetActive(true);
		ButtonsHolderAnim2.transform.GetChild(2).gameObject.SetActive(false);
		ButtonsHolderAnim2.transform.GetChild(3).gameObject.SetActive(false);
		ButtonsHolderAnim2.transform.GetChild(4).gameObject.SetActive(false);
	}

	public void ShowLaser()
	{
		ButtonsHolderAnim2.SetBool("bShow",false);
		tattooMachine.transform.position =  tattooMachine.startPosition;
		tattooMachine.gameObject.SetActive(true);
		tattooMachine.SetLaser();
		SetEraser();
		paintEngine.drawEnabled = true;
		Phase = 11;
	}



	public void TestErasedTattoo()
	{
 
		int i = 0;
		int leftPix = 0;
		while( i < (paintEngine.pixels.Length-4))
		{
			i+=4;
			if(paintEngine.pixels[i] > 15 ) leftPix++;

		}
		 
 
		if(leftPix <150 ) 
		{
			//Debug.Log("OBRISANO");
			Phase = 12;
			tattooMachine.bDrag = false;
			tattooMachine.gameObject.SetActive(false);
			paintEngine.GetComponent<RawImage>().color = new Color(1,1,1,0);
			ButtonsHolderAnim2.SetBool("bShow",true);

			ButtonsHolderAnim2.transform.GetChild(1).gameObject.SetActive(false);
			ButtonsHolderAnim2.transform.GetChild(2).gameObject.SetActive(true);
			ButtonsHolderAnim2.transform.GetChild(3).gameObject.SetActive(true);
			ButtonsHolderAnim2.transform.GetChild(4).gameObject.SetActive(false);

			CancelInvoke("TestErasedTattoo");
			tattooMachine.transform.position = new Vector3(-100,100,0);
			if(SoundManager.Instance!=null)
			{
				SoundManager.Instance.Stop_Sound( SoundManager.Instance.Laser);
				SoundManager.Instance.Stop_Sound( SoundManager.Instance.TattooMachine);
			}
		}
	}

	IEnumerator   TattooRemoved()
	{

		//Debug.Log("TattooRemoved");
		ButtonEraseTattoo.GetChild(0).gameObject.SetActive(false);
		ButtonEraseTattoo.GetChild(1).gameObject.SetActive(true);
		ButtonEraseTattoo.gameObject.SetActive(true);
		yield return new WaitForSeconds(1);
		ButtonsHolderAnim2.SetBool("bShow",false);
	}


	public void ButtonHomeClicked()
	{
		BlockClicks.Instance.SetBlockAll(true);
		LevelTransition.Instance.HideSceneAndLoadNext("HomeScene") ;
		GameData.IncrementButtonHomeClickedCount();
		if(SoundManager.Instance!=null)
		{
			SoundManager.Instance.Play_ButtonClick();
			SoundManager.Instance.Stop_Sound( SoundManager.Instance.Laser);
			SoundManager.Instance.Stop_Sound( SoundManager.Instance.TattooMachine);
		}
	}
	
	
	public void ButtonNextClicked()
	{
		BlockClicks.Instance.SetBlockAll(true);



		if( /*true ||*/  Phase == 9 ) 
		{
			//Debug.Log("TETOVAZA URADJENA");
 
			GameData.UradjeneTetovaze[(GameData.selectedLocation-1)] = CombineImages(  paintEngine.tex, (Texture2D) stencil.texture);
			GameData.locationRotZ =stencil.transform.localRotation.eulerAngles.z;
 
			GameData.locationScale =stencil.transform.localScale;
 
			GameData.UradjeneTetovazeScale[(GameData.selectedLocation-1)] = stencil.transform.localScale;
			GameData.UradjeneTetovazeRotZ[(GameData.selectedLocation-1)] = stencil.transform.localRotation.eulerAngles.z;
  
			 StartCoroutine("LoadNext",true);
		}
		else
		{
			//Debug.Log("TETOVAZA NIJE URADJENA");
			GameData.UradjeneTetovaze[(GameData.selectedLocation-1)] = null;
			GameData.locationRotZ = 0;
			StartCoroutine("LoadNext",false);
			GameData.locationScale = Vector3.one;
		}
 
		GameData.IncrementButtonNextClickedCount();
		if(SoundManager.Instance!=null)
		{
			SoundManager.Instance.Play_ButtonClick();
			SoundManager.Instance.Stop_Sound( SoundManager.Instance.Laser);
			SoundManager.Instance.Stop_Sound( SoundManager.Instance.TattooMachine);
		}
	}

 
 
	IEnumerator LoadNext(bool bDone)
	{
		yield return new WaitForSeconds(1);
		if(bDone) LevelTransition.Instance.HideSceneAndLoadNext("TattooPicture") ;
		else 	LevelTransition.Instance.HideSceneAndLoadNext("Select Place") ;
	}

	public void ButtonMoreGamesClicked()
	{
		BlockClicks.Instance.SetBlockAll(true);
		menuManager.ShowPopUpMenu( PupUpMoreGames );
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
		
	}


	//ovde se podesava skaliranje tetovaze i papira 
	void AdjustStencilRotationAndSize(int stencilIndex)
	{
		ScaleRotPosOftset srp = StencilSRP[stencilIndex];
		float xC =   tattooStencilScale[locationIndex].x/  tattooStencilScale[0].x  ;   
		float yC =   tattooStencilScale[locationIndex].y/  tattooStencilScale[0].y  ; 
		if(srp.izduzena)
		{
			if(xC < yC) yC = xC;
			else xC = yC;
		}
 
		stencil.transform.localRotation = Quaternion.Euler(0,0,srp.RotZ - (tattooStencilRotations[0]   - tattooStencilRotations[ locationIndex] ) );
		stencil.transform.localScale =    new Vector3(srp.x   *xC , srp.y * yC , 1f);     
		stencil.rectTransform.anchoredPosition = new Vector2(srp.posOffsetX,srp.posOffsetY);
	}


	List<ScaleRotPosOftset>StencilSRP = new List <ScaleRotPosOftset>()
	{
		new ScaleRotPosOftset { x= 1.05f, y = 1.7f, RotZ = 10, 	izduzena = false},		//0
		new ScaleRotPosOftset { x= 1.45f, y = 1.45f, RotZ = 340,  posOffsetY = 20},		//1
		new ScaleRotPosOftset { x= 1, y = 1.6f, RotZ = 18, 			 izduzena = false},		//2
		new ScaleRotPosOftset { x= 1.35f, y = 1.35f, RotZ = 342,  },								//3
		new ScaleRotPosOftset { x= 1.3f, y = 1.3f, RotZ = 65 	,izduzena = false},			//4
		new ScaleRotPosOftset { x= 1.2f, y = 1.2f, RotZ = 335,  posOffsetX = -12, posOffsetY = 70, izduzena = false},//5
		new ScaleRotPosOftset { x= 1f, y = 1f, RotZ = 0,  izduzena = false},  				//6
		new ScaleRotPosOftset { x= 1.3f, y = 1.3f, RotZ = 335 	 },  								//7
		new ScaleRotPosOftset { x= 1.5f, y = 1.5f, RotZ = 335, },  								//8
		new ScaleRotPosOftset { x= 1f, y = 1f, RotZ = 347, 		posOffsetX = -10, posOffsetY = 0,	 izduzena = false},  	//9
		new ScaleRotPosOftset { x= 1.05f, y = 1.2f, RotZ = 0, 		posOffsetX = -8,	posOffsetY = 0,	 izduzena = false},  	//10
		new ScaleRotPosOftset { x= 1.2f, y = 1.2f, RotZ = 30, 	posOffsetX = -8,		posOffsetY = 0,	 izduzena = false},  	//11
		new ScaleRotPosOftset { x= 1f, y = 1f, RotZ = 30,   izduzena = false},  	//12
		new ScaleRotPosOftset { x= 1.45f, y = 1.45f, RotZ = 20, 		  izduzena = false},  	//13
		new ScaleRotPosOftset { x= 1.1f, y = 1.2f, RotZ = 20, 		  izduzena = false},  		//14
		new ScaleRotPosOftset { x= 1.3f, y = 1.3f, RotZ = 65, 	  },  									//15

		//t2
		new ScaleRotPosOftset { x= 1.3f, y = 1.3f, RotZ = 340  	 	  },  								//0
		new ScaleRotPosOftset { x= 1f, y = 1f, RotZ = 340,  izduzena = false},  						//1
		new ScaleRotPosOftset { x= .9f, y = .9f, RotZ = 340 			},  									//2
		new ScaleRotPosOftset { x= 1f, y = 1f, RotZ = 345, 		 izduzena = false},  				//3
		new ScaleRotPosOftset { x= 1.15f, y = 1.15f, RotZ = 68 		 },  								//4
		new ScaleRotPosOftset { x= 1f, y = 1f, RotZ = 18, 			 izduzena = false},  			//5
		new ScaleRotPosOftset { x= 1.4f, y = 1.4f, RotZ = 338  		 },  								//6
		new ScaleRotPosOftset { x= 1.2f, y = 1.2f, RotZ = 0, 		posOffsetX = -20  },  			//7
		new ScaleRotPosOftset { x= 1f, y = 1f, RotZ = 0, 	 	 izduzena = false},  					//8
		new ScaleRotPosOftset { x= 1.3f, y = 1.3f, RotZ = 65, 		posOffsetY = 6 },  				//9
		new ScaleRotPosOftset { x= 1.5f, y = 1.5f, RotZ = 340,  posOffsetX = -5, posOffsetY = 25 },  	//10
		new ScaleRotPosOftset { x= .9f, y = .9f, RotZ = 65, 		posOffsetX = -10,	 izduzena = false},  	//11

		new ScaleRotPosOftset { x= 1.4f, y = 1.4f, RotZ = 340, 		posOffsetX = 9,},  	//12
		new ScaleRotPosOftset { x= 1.15f, y = 1.15f, RotZ = 0, 		posOffsetX = -5,	 izduzena = false},  	//13
		new ScaleRotPosOftset { x= 1.3f, y = 1.3f, RotZ =348, 		posOffsetX = 10, posOffsetY = 10 },  	//14
		new ScaleRotPosOftset { x= 1.2f, y = 1.2f, RotZ = 20, 		posOffsetX = 5 },  	//15
		 
	};
	//List < ScaleRotPosOftset>  StencilSRP = new List<ScaleRotPosOftset>(); 
	 
}

public class  ScaleRotPosOftset 
{
	public float x = 1;
	public float y = 1;
	public int RotZ = 0;
	public int posOffsetX = 0;
	public int posOffsetY = 0;
	public bool  izduzena = true;
}

