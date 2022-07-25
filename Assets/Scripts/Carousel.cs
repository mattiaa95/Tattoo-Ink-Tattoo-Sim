using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Carousel : MonoBehaviour,  IBeginDragHandler, IDragHandler, IEndDragHandler  //,   IPointerClickHandler //, IPointerDownHandler  , IPointerUpHandler 
{
	CanvasGroup BlockAll;
	private const float inchToCm = 2.54f;
	private EventSystem eventSystem = null;
	private float dragThresholdCM =  0.5f; //vrednost u cm
 
	public GameObject[] ItemPanels;
	public int ActiveItemNo = 0;
  
	//public static bool bEnable = true;

	bool bEnableDrag = true;
	bool bDrag = false;
	bool bInertia = false;
	bool bSnapToPosition = false;

	float x;
	float y;
	float speedX = 0;
	float speedLimit = .2f;
	float prevX=0;
	Vector3 diffPos = new Vector3(0,0,0);
	Vector3 startPos = new Vector3(0,0,0);
 
	
	float itemDistanceX; 
	float itemHalfDistanceX; 
	//float itemThirdDistanceX; 

	public Button Next;
	public Button Prev;
 
	public int SelectedChild = -1;
	public Transform ContentHolder;
	Vector3 DragPosition;

	int snapCounter = 0;


	public bool b_Enabled = false;

	public Sprite FrameSelected;


	public Transform LimitLeft;
	public Transform LimitRight;
	



	void Start()
	{


		LimitLeft.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0,.5f,100));
		LimitRight.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(1,.5f,100));

 

		GameData.selectedModel = 1;
		//b_Enabled = true;
		
		
		BlockAll = GameObject.Find("BlockAll").GetComponent<CanvasGroup>();
		if (eventSystem == null)  eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
		
//		float distX1 = (Camera.main. ViewportToWorldPoint(new Vector3(1,0,0) ).x -  Camera.main. ViewportToWorldPoint(new Vector3(0.38f,0,0)).x );
//		for( int i = 0; i < ItemPanels.Length; i++)
//		{
//			ItemPanels[i].transform.position = new Vector3(ItemPanels[0].transform.position.x+i*distX1,  ItemPanels[i].transform.position.y,ItemPanels[i].transform.position.z);
//		}
		
		
		
		
		itemDistanceX = new Vector3(ItemPanels[1].transform.position.x - ItemPanels[0].transform.position.x, 0, 0).x;
		itemHalfDistanceX = itemDistanceX /2f;
		//itemThirdDistanceX = itemDistanceX/3f;
		startPos   = ContentHolder.position;
		SetDragThreshold();




		ActiveItemNo = ActiveChildPanelIndex( GameData.selectedModel);
		
		if(ActiveItemNo>=ItemPanels.Length)  Next.interactable = false;
		else Next.interactable = true;
		if(ActiveItemNo<1)  Prev.interactable = false;
		else Prev.interactable = true;
		
		SelectedChild = -1;	
		
		bSnapToPosition = false;
		bInertia = false;
		ContentHolder.position -=  ItemPanels[ActiveItemNo].transform.position -  ItemPanels[0].transform.position;
		ContentHolder.position = new Vector3(ContentHolder.position.x, startPos.y,startPos.z);
		
		
		
	}
	
	
	int ActiveChildPanelIndex( int childNo)
	{

		return childNo-1;

	}
	
	int ActiveChildIndex( int _ActiveItemNo)
	{

		return _ActiveItemNo+1;
		
	}
	
	
	void FixedUpdate()
	{
		if(!b_Enabled) return;
		if( bDrag)
		{

		}
		else if(bInertia)
		{
			prevX = ContentHolder.position.x;
			speedX *= 0.92f;
			if( speedX > 0.3f || speedX < -0.3f ) ContentHolder.position += new Vector3(speedX*Time.fixedDeltaTime,0,0); 
			else 
			{
				bInertia = false;
				bSnapToPosition = true;
			}
 
			if(ContentHolder.position.x <  (startPos.x - itemDistanceX*ActiveItemNo-0.5f ) -2)
			{
				if(ActiveItemNo < (ItemPanels.Length -1 ) )
				{
					btnNext();
				}
			}
			else if(ContentHolder.position.x >  (startPos.x - itemDistanceX*ActiveItemNo-0.5f ) +2)
			{
				if(ActiveItemNo > 0 ) 
				{
					btnPrevious();
				}
			}

		}

		if( (bDrag || bInertia)  &&  ContentHolder.position.x> prevX )   
		{

			if(ActiveItemNo == 0 && ItemPanels[0].transform.position.x >0.5f)
			{
				ContentHolder.position = new Vector3 (.5f ,  startPos.y,  startPos.z);
				bInertia = false;
			//	if(!bDrag) 

					bSnapToPosition = true;
			}
		}
		
		else if( (bDrag || bInertia)  &&  ContentHolder.position.x<prevX )   
		{
			if( ActiveItemNo == (ItemPanels.Length-1) && ItemPanels[ItemPanels.Length-1].transform.position.x <-0.5f )
			{
				ContentHolder.position = new Vector3 (startPos.x - itemDistanceX*ActiveItemNo-0.5f,  startPos.y,  startPos.z);
				bInertia = false;
			//	if(!bDrag)
					bSnapToPosition = true;
			}

		}

 
		if(bSnapToPosition)
		{
			float x = Mathf.Lerp(ContentHolder.position.x ,(startPos.x - itemDistanceX*ActiveItemNo ), Time.fixedDeltaTime*8);
			ContentHolder.position = new Vector3(x, startPos.y,startPos.z);
//			if(snapCounter<600)
//				snapCounter++;
//			else
//			{
//				bSnapToPosition = false;
//				snapCounter = 0;
//			}
		}


		//skaliranje
		if(bDrag || bInertia || bSnapToPosition)
		{
			for(int i = ActiveItemNo-1; i<=ActiveItemNo+1; i++)
			{
				if(i<0 || i>=ItemPanels.Length) continue;
				if( ItemPanels[i].transform.position.x  < 3f && ItemPanels[i].transform.position.x  > -3f) 
				{
					ItemPanels[i].transform.localScale =   Vector3.one*( .7f +.1f* (3- Mathf.Abs(ItemPanels[i].transform.position.x )));
					//if(!bSnapToPosition) ActiveItemNo = i;
				}
				else ItemPanels[i].transform.localScale = Vector3.one*.7f;
			}
			
			if(ActiveItemNo>ItemPanels.Length-2)  Next.interactable = false;
			else Next.interactable = true;
			if(ActiveItemNo<1)  Prev.interactable = false;
			else Prev.interactable = true;

//			Debug.Log(Prev.interactable  + "  pr, n " +  Next.interactable);
		}


	}
	
	
	
	private void SetDragThreshold()
	{
		if (eventSystem != null) eventSystem.pixelDragThreshold = (int)( dragThresholdCM * Screen.dpi / inchToCm);
	}
	
	public void  OnBeginDrag (PointerEventData eventData)
	{
		if(!b_Enabled) return;
		bInertia = false;
		bSnapToPosition = false;
		bDrag = true;
		diffPos =  -ContentHolder.position + Camera.main.ScreenToWorldPoint(Input.mousePosition)   ;
	}
	
	public void  OnDrag(PointerEventData eventData)
	{
		if(!b_Enabled) return;
		if(!bDrag || bSnapToPosition) return;
		else
		{
			DragPosition =  Camera.main.ScreenToWorldPoint(Input.mousePosition) - diffPos;
			prevX = ContentHolder.position.x;
			float x =  DragPosition.x;// Mathf.Lerp(ContentHolder.position.x ,DragPosition.x, Time.deltaTime*6);
			ContentHolder.position = new Vector3(x, startPos.y,startPos.z);
		}
 
		if(ContentHolder.position.x <  (startPos.x - itemDistanceX*ActiveItemNo-0.5f ) -1.5f)
		{
			if(ActiveItemNo < (ItemPanels.Length -1) ) 
			{
				btnNext();
			}
		}
		else if(ContentHolder.position.x >  (startPos.x - itemDistanceX*ActiveItemNo-0.5f ) +1.5f)
		{
			if(ActiveItemNo > 0 ) 
			{
				btnPrevious();
			}
		}
 	
	}
	
	public void  OnEndDrag(PointerEventData eventData)
	{
		snapCounter = 0;
		speedX = (ContentHolder.position.x-prevX)/Time.deltaTime;
		bDrag = false;
		if( bSnapToPosition) bInertia = false;
		else bInertia = true;
	}
	
	
	int SelectedChildIndex( int _ActiveItemNo)
	{

		return _ActiveItemNo;
		
	}
	 /*
	public void OnPointerClick( PointerEventData eventData)
	{
		if(!b_Enabled) return;
		if(!eventData.dragging) 
		{
			
			//Debug.Log("CLICK" + eventData.rawPointerPress.name);
			string PanelId = eventData.rawPointerPress.transform.parent.name.Replace("Panel","") ;
			int childPIndex = ActiveChildIndex( ActiveItemNo );
			//Debug.Log(PanelId + " CLICK " + childPIndex.ToString());
			
			if(childPIndex .ToString().PadLeft(2,'0') == PanelId)
			{
				 
				SelectedChild = SelectedChildIndex(ActiveItemNo);
				
				GameData.selectedModel = SelectedChild;
				 Debug.Log("Selected MODEL:   " + SelectedChild);

				Transform panel = eventData.rawPointerPress.transform.parent;
				//eventData.rawPointerPress.transform.parent.FindChild("ImageSelected").GetComponent<Animator>().SetTrigger("tShow");

				panel.GetComponent<Animator>().SetBool("bSelected",true);
				//panel.GetChild(0).GetComponent<Image>().sprite =  FrameSelected;

				b_Enabled = false;
		 		//SoundManager.Instance.Play_Sound(SoundManager.Instance.SelectChild);
			}
		}
	}
	*/
	 

	public void SelectPanel( int panel)
	{
		SelectedChild = SelectedChildIndex(ActiveItemNo);
		GameData.selectedModel = SelectedChild ;

		GameData.ClearTattoos();


// 		Debug.Log("Selected MODEL:   " + SelectedChild);

		ItemPanels[ActiveItemNo].transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite =  FrameSelected;



		ItemPanels[ActiveItemNo].GetComponent<Animator>().SetBool("bSelected",true);
		BlockClicks.Instance.SetBlockAll(true);
		b_Enabled = false;
		 SoundManager.Instance.Play_Sound(SoundManager.Instance.CheckButton);
		StartCoroutine("LoadNext");
	}

	IEnumerator LoadNext()
	{
		yield return new WaitForSeconds(1);
		 LevelTransition.Instance.HideSceneAndLoadNext("Hospital") ;
	}
	
	public void btnNext(  )
	{
		if(!b_Enabled) return;
		if(ActiveItemNo>(ItemPanels.Length-2) )  return;
		
		ActiveItemNo++;
		bSnapToPosition = true;
		bInertia = false;
		
		
		BlockAll.blocksRaycasts = true;
		StartCoroutine(SetBlockAll(.4f,false));
		
		if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound( SoundManager.Instance.ButtonClick2);
	}
	
	public void btnPrevious(  )
	{
		if(!b_Enabled) return;
		if(ActiveItemNo<1 )  return;
		
		ActiveItemNo--;
		
		bSnapToPosition = true;
		bInertia = false;
		
		
		BlockAll.blocksRaycasts = true;
		StartCoroutine(SetBlockAll(.4f,false));
		
		if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound( SoundManager.Instance.ButtonClick2);
	}
	
	IEnumerator SetBlockAll(float time, bool blockRays)
	{
		if(BlockAll == null) BlockAll = GameObject.Find("BlockAll").GetComponent<CanvasGroup>();
		yield return new WaitForSeconds(time);
		BlockAll.blocksRaycasts = blockRays;
		
	}
	
	


	






 
}