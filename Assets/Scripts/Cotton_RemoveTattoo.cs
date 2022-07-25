using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class Cotton_RemoveTattoo  : MonoBehaviour,IBeginDragHandler , IDragHandler, IEndDragHandler,  IPointerDownHandler , IPointerUpHandler 
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
	public bool bCloth;//poslednja krpica za brisanje uklonjene tetovaze - trenutno je slika tufera
	public bool bCottonEnabled;
	public bool bClothBlood; //krpica za brisanje krvi 
	
	float x;
	float y;
	Vector3 diffPos = new Vector3(0,0,0);
	
	Vector3  StartPosition;
	Transform StartParent;
	public Transform ActiveItemHolder;
	 
	
	int mouseFollowSpeed = 10; 
	
	
	public bool bFinished; 
	public bool bSnapToTarget; 
	
	public Transform TargetPoint;
	public Transform TestPoint; 
	public float snapScale =1;
	
	float SnapDistance = 0.5f;
	
	TattooSalonGameplay gameplay;
	public Animator tutorialAnim;

	
	public ProgressBar progressBar;
	bool bMoveCotton = false;

	public Image TattooRemoveFluid;
	public Sprite TattooRemoveFluidSprite;
	bool bStarted = false;
	Vector3 TattooRemoveFluidScale = Vector3.one;
	void Awake()
	{
		dragItem.gameObject.SetActive(false);
		StartParent = transform.parent;
		//if(progressBar!=null) progressBar.gameObject.SetActive(false);
	}
	
	public void OnPointerDown( PointerEventData eventData)
	{
		 
		//Debug.Log("CLICKED:"+ transform.name);
		tutorialAnim.gameObject.SetActive(true);
		if( !bEnableDrag || bFinished ) return;
		if(gameplay.Phase < 14 && bCloth)
		{
			//tutorial pokazivac na prethodno dugme
			tutorialAnim.transform.position = transform.parent.parent.Find("ButtonCream/Icon").transform.position;
			tutorialAnim.Play("HandPointer");
			return;
		}
		else tutorialAnim.Play("default");
		
		if(bCotton && !bCottonEnabled && !bSnapToTarget && icon.enabled)
		{
		 
			icon.enabled = false;
			dragItem.gameObject.SetActive(true);
			
			anim.Play("CottonBottle");
			
			if(ButtonsHolderAnim== null) ButtonsHolderAnim = GameObject.Find("Canvas/Menus/BottomButtonsHolder").GetComponent<Animator>();
			ButtonsHolderAnim.SetBool("bShow",false);
			transform.SetParent(ActiveItemHolder);
			bEnableDrag = false;
			return;
		}

		if(gameplay.Phase == 14 && bCloth)
		{

			bEnableDrag = true;
			bCottonEnabled = true;
		}
		
		
		icon.enabled = false;
		dragItem.gameObject.SetActive(true);
		
		
		dragOffset =  Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position; 
		dragOffset = new Vector3(dragOffset.x,dragOffset.y,0);
		
		
		if(ButtonsHolderAnim== null) ButtonsHolderAnim = GameObject.Find("Canvas/Menus/BottomButtonsHolder").GetComponent<Animator>();
		ButtonsHolderAnim.SetBool("bShow",false);
		transform.SetParent(ActiveItemHolder);
		
	}
	
	public void OnPointerUp( PointerEventData eventData)
	{
		if(!bCotton  || ( bCotton && bDrag &&  bCottonEnabled && !bSnapToTarget) )
		{
			if( !bClothBlood ||  !(bFinished && bClothBlood) )
			{
				StartCoroutine("MoveBack" );
			}
			CancelInvoke("TestDistance");
			tutorialAnim.Play("default");
		}
	}
	
	IEnumerator Start () {
		
		StartPosition = transform.position;
		
		ButtonsHolderAnim = GameObject.Find("Canvas/Menus/BottomButtonsHolder").GetComponent<Animator>();
	//	ButtonsHolderAnim.SetBool("bShow",true);
		yield return new WaitForSeconds(.5f);
		
		gameplay = Camera.main.GetComponent<TattooSalonGameplay>();
		
	}
	
	void Update()
	{
		 if( bDrag  && bEnableDrag)  
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
	
	
	
	public void OnBeginDrag (PointerEventData eventData)
	{
		
		if(!bEnableDrag) return;
		if(!bDrag && !bFinished )
		{
			bDrag = true;
			StartPosition = transform.position;
			diffPos =transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition)   ;
			diffPos = new Vector3(diffPos.x,diffPos.y,0);
			InvokeRepeating("TestDistance",0, .1f);
			//if( Tutorial.Instance!=null) 	Tutorial.Instance.StopTutorial();
			//transform.SetParent(DragItemParent);

			TestPoint.position = gameplay.stencil.transform.position;
		  	
			if(  !bCloth ||  ( gameplay.Phase == 14 && bCloth ) ) 
			{
				tutorialAnim.transform.position = gameplay.stencil.transform.position;
				tutorialAnim.Play("Circle");
			}

			if(!bStarted)
			{
				 
				bStarted = true;
				TattooRemoveFluid.enabled = true;
 
			}
		}
	}
	
	public void OnEndDrag (PointerEventData eventData)
	{
		bDrag = false;
	}
	
	
	public void OnDrag (PointerEventData eventData)
	{
		if( bCotton &&   bDrag &&  bCottonEnabled) bMoveCotton = true;
		if( bCloth &&   bDrag  ) bMoveCotton = true;
	}
	
	void TestDistance()
	{
	 
		if(bCotton || bCloth) //tufer ili krpica
		{
			if(Vector2.Distance(TestPoint.position,TargetPoint.position)<SnapDistance)
			{
			 
				if( progressBar.Value<1)
				{
					if(bMoveCotton) 
					{
						bMoveCotton = false;
						progressBar.SetProgress(progressBar.Value + 0.03f);
						if(bCotton) {
							TattooRemoveFluid.color = new Color(1,1,1,progressBar.Value);
							TattooRemoveFluid.transform.localScale = TattooRemoveFluidScale;
							if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.Cotton);
						}
						else if(bCloth) 
						{
							TattooRemoveFluid.transform.localScale = (0.3f + progressBar.Value*.7f)  * TattooRemoveFluidScale;
							TattooRemoveFluid.color = Color.white;
							if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.Cotton);
						}
					}
				}
				else
				{
					CancelInvoke("TestDistance");
					bMoveCotton = false;
					anim.Play("CottonEnd");
					bCottonEnabled = false;
					progressBar.gameObject.SetActive(false);
					if(bCloth && gameplay.Phase == 14)
					{
						gameplay.Phase = 15;
						TattooRemoveFluid.transform.localScale = TattooRemoveFluidScale;

					}
				}
			}
		}
		else if(!bClothBlood)//krema
		{
			if(Vector2.Distance(TestPoint.position,TargetPoint.position)<SnapDistance)
			{

				StartCoroutine("MoveBack");
				CancelInvoke("TestDistance");
				tutorialAnim.Play("default");
				 bSnapToTarget = true;
				bEnableDrag = false;
				bDrag = false;
				TattooRemoveFluid.gameObject.SetActive(true);
				TattooRemoveFluid.enabled = true;
				TattooRemoveFluid.color = new Color(1,1,1,1);
				TattooRemoveFluid.transform.localScale = 0.3f * TattooRemoveFluidScale;
				TattooRemoveFluid.sprite = TattooRemoveFluidSprite;

				gameplay.Phase = 14;

				progressBar.SetProgress(0);
				progressBar.gameObject.SetActive(true);


				bFinished = true;
				dragItem.gameObject.SetActive(false);
				icon.enabled = true;
				bMovingBack = false;
				
				transform.SetParent(StartParent);
				transform.position = StartPosition;
				bEnableDrag = true;
				icon.transform.parent.GetChild(0).gameObject.SetActive(true);

				ButtonsHolderAnim.SetBool("bShow",true);
			}
		}
		else
		{
			if(Vector2.Distance(TestPoint.position,TargetPoint.position)<SnapDistance)
			{
				bFinished = true;
				bDrag = false;
				bMovingBack = false; 
				bEnableDrag = false;
				CancelInvoke("TestDistance");
				tutorialAnim.Play("Circle");

				anim.Play("RemoveBlood");
				if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.Cotton);

			}
		}
 
	 
	}
	
	 
	
	
	public void AnimCottonBottleEnd()
	{
		bEnableDrag = true;
		bCottonEnabled = true;
		if(icon.transform.childCount>0)  icon.transform.GetChild(0).GetComponent<Image>().enabled = true;
		progressBar.gameObject.SetActive(true);
		progressBar.SetProgress(0);

		TattooRemoveFluidScale = TattooRemoveFluid.transform.localScale ;
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

		if(gameplay.Phase ==7)
		{
			gameplay. StencilFirstPassDone();

		}
		else
		if(gameplay.Phase < 15)
		{
			ButtonsHolderAnim.SetBool("bShow",true);
	 		gameplay.ShowLaser( );
			StartCoroutine("HideFluid");
		}
		else
		{
 			Debug.Log("KRAJ: upali dugme");

			gameplay.StartCoroutine("TattooRemoved");
			StartCoroutine("HideFluid");
		}
		
		icon.transform.parent.GetChild(0).gameObject.SetActive(true);

		 tutorialAnim.Play("default");
	}

	IEnumerator HideFluid()
	{
		float i = 1;
		while(i>0)
		{
			i-=Time.fixedDeltaTime*.5f;
			if(i<0) i =0;
			TattooRemoveFluid.color = new Color(1,1,1,i);
			yield return new WaitForFixedUpdate();
		}

		TattooRemoveFluid.gameObject.SetActive(false);
	}
	

	
	IEnumerator MoveBack(  )
	{
		
		bDrag = false;
		bEnableDrag = false;
		yield return new WaitForFixedUpdate( );
		if(!bMovingBack && !bSnapToTarget)
		{
			ButtonsHolderAnim.SetBool("bShow",true);
			yield return new WaitForSeconds(.2f );
			
			bMovingBack = true;
			
			float dist = Vector3.Distance( transform.position, StartPosition);			 
			Vector3 currentPosition = transform.position;
			
			if(dist>0.1f)
			{
				float timeCoef =  20f/dist;
				
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
