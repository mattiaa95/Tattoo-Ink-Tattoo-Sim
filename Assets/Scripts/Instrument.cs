using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class Instrument : MonoBehaviour,IBeginDragHandler , IDragHandler, IEndDragHandler,  IPointerDownHandler , IPointerUpHandler 
{
	public Image icon;
	public Transform dragItem;
	public Animator anim;
	 
	public Animator CardiographAnim;
	Animator ButtonsHolderAnim;

	Vector3 dragOffset;
	public bool bDrag = false;
	public static bool bEnableDrag = true;
	public bool bMovingBack = false;
	public bool bCotton;
	public bool bCottonEnabled;
	public bool bInjection;

	float x;
	float y;
	Vector3 diffPos = new Vector3(0,0,0);

	Vector3  StartPosition;
	Transform StartParent;
	public Transform ActiveItemHolder;
	public Transform FingerPressPosition;
	public bool bTestFP = false;
	public int FingerPressCount = 0;

	int mouseFollowSpeed = 10; 

 
	public bool bFinished; 
	public bool bSnapToTarget; 

	public Transform TargetPoint;
	public Transform TestPoint;//woman
	public Transform TargetPointMan;
	public float snapScale =1;

	float SnapDistance = 0.5f;

	HospitalScene hospital;

	Animator tutorialAnim;
	Transform TutorialHolderUnder;
	Transform TutorialHolderOver;

	public ProgressBar progressBar;
	bool bMoveCotton = false;
	bool bPressureTest = false;
	 void Awake()
	{
		dragItem.gameObject.SetActive(false);
		StartParent = transform.parent;
		if(progressBar!=null) progressBar.gameObject.SetActive(false);
	}

	IEnumerator Start () {
		
		StartPosition = transform.position;
		
		ButtonsHolderAnim = GameObject.Find("Canvas/Menus/BottomButtonsHolder").GetComponent<Animator>();
		ButtonsHolderAnim.SetBool("bShow",true);
		yield return new WaitForSeconds(.5f);
		
		hospital = Camera.main.GetComponent<HospitalScene>();
		TutorialHolderUnder = GameObject.Find("Canvas/TutorialHolderUnder").transform;
		TutorialHolderOver = GameObject.Find("Canvas/TutorialHolderOver").transform;
	}
	
	void Update()
	{
		if(bTestFP)
		{
			if(Input.GetMouseButtonDown(0))
			{
				bTestFP = false;
				FingerPressCount++;
				if(FingerPressCount<3)
				{
					anim.Play("BloodPressureActive");
					StartCoroutine("EnableTestBloodPressureFP");
					if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound( SoundManager.Instance.BloodPressure); 
				}
				else if(FingerPressCount==3)
				{
					bFinished = true;
					bEnableDrag = false;
					anim.Play("BloodPressureEnd");
					CancelInvoke("TestDistance");
					if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound( SoundManager.Instance.BloodPressure); 
				}
				tutorialAnim.Play("default");
			}
		}
		else 	if( bDrag  && bEnableDrag)  
		{
			x = Input.mousePosition.x;
			y = Input.mousePosition.y;
			
			Vector3 posM = Camera.main.ScreenToWorldPoint(new Vector3(x ,y,10.0f) );
			if(posM.x<-2.5f  ) posM = new Vector3(-2.5f,posM.y,posM.z);
			else if(posM.x>2.5f  ) posM = new Vector3(2.5f,posM.y,posM.z);
			
			if(posM.y<-5.5f  ) posM = new Vector3(posM.x,-5.5f, posM.z);
			else if(posM.y>1.6f) posM = new Vector3(posM.x,1.6f,posM.z);
			transform.position =  Vector3.Lerp (transform.position, posM  , mouseFollowSpeed * Time.deltaTime)  ;
		}
		
		
	}

	public void OnPointerDown( PointerEventData eventData)
	{

		 


		if(tutorialAnim== null) tutorialAnim = GameObject.Find("TutorialAnim").GetComponent<Animator>();
  

//		Debug.Log("CLICKED:"+ transform.name);

		if(!bEnableDrag || bFinished || bPressureTest   ) return;

		if( !hospital.bCottonOK && bInjection)
		{
			//tutorial pokazivac na prethodno dugme
			tutorialAnim.transform.position = transform.parent.parent.Find("ButtonCotton/Icon").transform.position;
			tutorialAnim.Play("HandPointer");
			tutorialAnim.transform.SetParent(TutorialHolderOver);
			return;
		}


		if(bCotton && !bCottonEnabled && !bSnapToTarget && icon.enabled)
		{
			 
			icon.enabled = false;
			dragItem.gameObject.SetActive(true);

			anim.Play("CottonBottle");

			if(ButtonsHolderAnim== null) ButtonsHolderAnim = GameObject.Find("Canvas/Menus/BottomButtonsHolder").GetComponent<Animator>();
			ButtonsHolderAnim.SetBool("bShow",false);
			 transform.SetParent(ActiveItemHolder);
			bEnableDrag = false;
			if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound( SoundManager.Instance.ButtonClick);
			return;
		}
	 

		 

	 
		icon.enabled = false;
		dragItem.gameObject.SetActive(true);
	 

		dragOffset =  Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position; 
		dragOffset = new Vector3(dragOffset.x,dragOffset.y,0);


		if(ButtonsHolderAnim== null) ButtonsHolderAnim = GameObject.Find("Canvas/Menus/BottomButtonsHolder").GetComponent<Animator>();
		ButtonsHolderAnim.SetBool("bShow",false);
		if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound( SoundManager.Instance.ButtonClick);
		transform.SetParent(ActiveItemHolder);

	}

	public void OnPointerUp( PointerEventData eventData)
	{
		if(!bCotton  || ( bCotton && bDrag &&  bCottonEnabled && !bSnapToTarget) )
		{
			StartCoroutine("MoveBack" );
			CancelInvoke("TestDistance");
			tutorialAnim.Play("default");
			//tutorialAnim.transform.SetParent(TutorialHolderUnder);
		}
	}

	IEnumerator EnableTestBloodPressureFP () {
		yield return new WaitForSeconds(1f);
		bTestFP = true;
	}
	



	 
	public void OnBeginDrag (PointerEventData eventData)
	{
		if( !hospital.bCottonOK && bInjection || bPressureTest) return;
	 
		if(!bEnableDrag) return;
		if(!bDrag && !bFinished )
		{
			StartPosition = transform.position;
			bDrag = true;
			diffPos =transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition)   ;
			diffPos = new Vector3(diffPos.x,diffPos.y,0);
			 InvokeRepeating("TestDistance",0, .1f);
			//if( Tutorial.Instance!=null) 	Tutorial.Instance.StopTutorial();
			//transform.SetParent(DragItemParent);

			if(hospital.bModelMan) tutorialAnim.transform.position = TargetPointMan.position;
			else tutorialAnim.transform.position = TargetPoint.position;
			tutorialAnim.Play("Circle");
			tutorialAnim.transform.SetParent(TutorialHolderUnder);
		}
	}
	 
	public void OnEndDrag (PointerEventData eventData)
	{

	}
	
	
	public void OnDrag (PointerEventData eventData)
	{
		if( bCotton &&   bDrag &&  bCottonEnabled) bMoveCotton = true;
		
	}
 
	void TestDistance()
	{
		if(hospital.bModelMan)
		{
			if(Vector2.Distance(TestPoint.position,TargetPointMan.position)<SnapDistance)
			{
				if(!bCotton)
				{
					StartCoroutine("SnapToParent");
					bDrag = false;
				}
				else
				{
					if( progressBar.Value<1)
					{
						if(bMoveCotton) 
						{
							bMoveCotton = false;
							progressBar.SetProgress(progressBar.Value + 0.03f);
							if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.Cotton);
						}
					}
					else
					{
						CancelInvoke("TestDistance");
						bMoveCotton = false;
						anim.Play("CottonEnd");
						bCottonEnabled = false;
						progressBar.gameObject.SetActive(false);
					}

				}
			}
		}
		else
		{
			if(Vector2.Distance(TestPoint.position,TargetPoint.position)<SnapDistance)
			{
				if(!bCotton)
				{
		 			StartCoroutine("SnapToParent");
					 bDrag = false;
				}
				else
				{
					if( progressBar.Value<1)
					{
						if(bMoveCotton) 
						{
							bMoveCotton = false;
							progressBar.SetProgress(progressBar.Value + 0.05f);
							if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.Cotton);
						}
					}
					else
					{
						CancelInvoke("TestDistance");
						bMoveCotton = false;
						anim.Play("CottonEnd");
						bCottonEnabled = false;
						progressBar.gameObject.SetActive(false);
					}
				}
			}
		}
	}

	IEnumerator SnapToParent()
	{
		if(!bMovingBack && !bFinished ) 
		{
			bSnapToTarget = true;
			bEnableDrag = false;
			bDrag = false;
			//ugasiti tutorial
 
			CancelInvoke("TestDistance");
			tutorialAnim.Play("default");
			float timeMove = 0;
			bool animationStarted = false;
			//SoundManager.Instance.StopAndPlay_Sound(SoundManager.Instance.ElementCompleted);

			Vector3 target;
			if(hospital.bModelMan)
		 		target =  TargetPointMan.position - TestPoint.position +  transform.position  ;
			else
			{
				transform.Rotate(new Vector3(0,0,15));
				target =  TargetPoint.position - TestPoint.position +  transform.position  ;
			}

			if(dragItem.name == "Thermometer" ) {
				anim.Play("ThermometerActive");
				hospital.MouthOpen.enabled = true;
				 
			}

			Vector3 startScale = 	transform.localScale;
			while  (timeMove  <1 )
			{
				yield return new WaitForFixedUpdate();
				transform.position = Vector3.Lerp ( transform.position , target , timeMove)  ;
				//transform.localScale  = Vector3.Lerp (transform.localScale,  EndLocationScale * StartScale,  timeMove );
				timeMove += Time.fixedDeltaTime*2;


				if( hospital.bModelMan && snapScale !=1)  transform.localScale =  startScale * (1+  timeMove * (snapScale-1));
			
			}

			if(dragItem.name == "Thermometer" ) {
			 
				yield return new WaitForSeconds(1.5f);
				if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound( SoundManager.Instance.Temperature);
			}


			if(dragItem.name == "Stethoscope") 
			{
				anim.Play("StethoscopeAnim");
				if(CardiographAnim!=null)
				{
					CardiographAnim.gameObject.SetActive(true);
					CardiographAnim.Play("cardiograph");
					yield return new WaitForSeconds(.8f);
					if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound( SoundManager.Instance.Temperature);
				}

			}

			if(dragItem.name == "BloodPressureInstrument") 
			{
				bEnableDrag = false;
				yield return new WaitForSeconds(.5f);
				bTestFP = true;
				bPressureTest = true;
				tutorialAnim.transform.position = FingerPressPosition.position;

				tutorialAnim.Play("HandPointer");
				tutorialAnim.transform.SetParent(TutorialHolderOver);
			}

			if(bCotton && bCottonEnabled)
			{
				anim.Play("CottonEnd");
				bCottonEnabled = false;

			}

			if(dragItem.name == "Injection") 
			{
				hospital.MouthOpen.enabled = true;
				anim.Play("Injection2");//leva ruka 	//anim.Play("Injection");
				yield return new WaitForSeconds(1.3f);
				if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound( SoundManager.Instance.Needle);
			}
		}
		 
		
		yield return new WaitForFixedUpdate();
		
		
		//		Gameplay.Instance.ChangeProgressBar();
		//		
		//		Tutorial.bPause = true;
		//		Tutorial.timeLeftToShowHelp = 10000;
		//		Tutorial.ShowHelpPeriod = 10000;
		//		
//		if( Application.loadedLevelName == "LevelGame3" ) Camera.main.SendMessage("NextPhase", SendMessageOptions.DontRequireReceiver);
//		else 	bEnableDrag = true;
		 

	}


	public void AnimCottonBottleEnd()
	{
		bEnableDrag = true;
		bCottonEnabled = true;
		icon.transform.GetChild(0).GetComponent<Image>().enabled = true;
		progressBar.gameObject.SetActive(true);
		progressBar.SetProgress(0);
	}

	 

	public void AnimExamEnd(int exam)
	{
		//cekiranje zavrsene operacije
		bFinished = true;
		dragItem.gameObject.SetActive(false);
		icon.enabled = true;
		bMovingBack = false;
		
		transform.SetParent(StartParent);
		transform.position = StartPosition;
		bEnableDrag = true;
		bDrag = false;
		ButtonsHolderAnim.SetBool("bShow",true);

		if(exam == 1) bTestFP = false;
		hospital.ExamOver(exam);

		icon.transform.parent.GetChild(0).gameObject.SetActive(true);

	}

	public void AnimFingerPressEnd( )
	{
		if(dragItem.name == "BloodPressureInstrument") 
		{
			//bTestFP = true;
			tutorialAnim.transform.position = FingerPressPosition.position;
			tutorialAnim.Play("HandPointer");
		}
	}

	IEnumerator MoveBack(  )
	{

		bDrag = false;
		bEnableDrag = false;
		yield return new WaitForFixedUpdate( );
		if(!bMovingBack && !bSnapToTarget)
		{
			ButtonsHolderAnim.SetBool("bShow",true);
			//if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound( SoundManager.Instance.ShowDecorationMenu);


			bMovingBack = true;
			yield return new WaitForSeconds(.3f );
			float dist = Vector3.Distance( transform.position, StartPosition);			 
			Vector3 currentPosition = transform.position;

			if(dist>0.05f)
			{
				float timeCoef =  17f/dist;

				yield return new WaitForEndOfFrame( );
				float pom = 0;
				while(pom<1 )
				{ 
					pom+=Time.deltaTime*timeCoef;
					transform.position = Vector3.Lerp(currentPosition, StartPosition,pom);
					yield return new WaitForFixedUpdate( );
				}
			}
			
			 
			dragItem.gameObject.SetActive(false);

			icon.enabled = true;
			bMovingBack = false;

			transform.SetParent(StartParent);
			transform.localPosition = Vector3.zero;
			//transform.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
		}
 


		bEnableDrag = true;
		//Tutorial.bPause = false;	 
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
 
				StartCoroutine("MoveBack" );
			}
		}
		appFoucs = hasFocus;
		
	}
 


 
}
