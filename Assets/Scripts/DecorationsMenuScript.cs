using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class DecorationsMenuScript : MonoBehaviour {
	
	public TattooSalonGameplay gameplay;

	public HorizontalLayoutGroup hlgDecotrations;
	public Transform DecorationsButtonsHolder;
 
	public ScrollRect scrollRect;
	 
	public Sprite[] tattoos;
	bool [] unlocked = {true,false,true,false,true,false,true,false,true,false,true,false,true,false,true,false,true,false,true,false,true,false,true,false,true,false,true,false,true,false,true,false}; // 20 //da li je dekoracija otkljucana
	int decorationToUnlock = 0;
  
	public Transform LimitLeft;
	public Transform LimitRight;
	


	private const float inchToCm = 2.54f;
	private EventSystem eventSystem = null;
	private float dragThresholdCM =  0.5f; //vrednost u cm


	void Awake () 
	{
		for (int i = 1; i < DecorationsButtonsHolder.childCount; i++) 
		{
			DecorationsButtonsHolder.GetChild(i).GetChild(0).GetComponent<Image>().sprite = tattoos[(i-1)];
		}
	}

	void Start () 
	{
		SetDragThreshold();

		GameData.GetUnlocekedItems(1, ref unlocked);

		SetButtons();
	}
 
	private void SetDragThreshold()
	{
		if (eventSystem == null)  eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
		if (eventSystem != null) eventSystem.pixelDragThreshold = (int)( dragThresholdCM * Screen.dpi / inchToCm);
	}

 
	void SetButtons()
	{
		for (int i = 0; i < DecorationsButtonsHolder.childCount; i++) 
		{
			DecorationsButtonsHolder.GetChild(i).GetChild(1).GetComponent<Image>().enabled =  !unlocked[(i)];
		}
	}
 
	//--------------------kreiranje dekoracije---------------------------------------------------------------
	public void ButtonSelectDecoration(int decorationInex)
	{
		decorationInex--;
		BlockClicks.Instance.SetBlockAll(true);
		//BlockClicks.Instance.SetBlockAllDelay(.5f,false);

		 
		if( !unlocked[decorationInex] ) 
		{
			decorationToUnlock = decorationInex;
            Yodo1Ads.Instance.bVideoTattoo = true;
            Yodo1Ads.Instance.bVideoInk = false;
            Yodo1Ads.Instance.ShowVideoReward();

			return; 
		}
		else
		{
		 
			Sprite spr  = tattoos[ decorationInex ];
			 
			gameplay.ShowStencil( spr, decorationInex);
			if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound( SoundManager.Instance.SelectDecoration);
		}
	}

	public void EndWatchingVideoDecoration()
	{
		Debug.Log("UNLOCK DECORATION: "+   ",  "+ decorationToUnlock);
		unlocked[decorationToUnlock] = true;
 

		DecorationsButtonsHolder.GetChild(decorationToUnlock).GetChild(1).GetComponent<Image>().enabled =  !unlocked[decorationToUnlock];
	 
		GameData.SaveUnlocekedItemsToPP(1 , ref unlocked );
		if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound( SoundManager.Instance.UnlockedDecoration);
	}

	 
	/*
	IEnumerator ChangeSprite(Image _img, Sprite _sprite)
	{
		float alpha =_img.color.a;
		_img.enabled = true;

		while( alpha >0  )
		{
			alpha -= Time.deltaTime*5;
			_img.color = new Color(1,1,1,alpha);
			yield return new WaitForEndOfFrame();
		}

		_img.sprite = _sprite;
		while( alpha < 1)
		{
			alpha += Time.deltaTime*2;
			_img.color = new Color(1,1,1,alpha);
			yield return new WaitForEndOfFrame();
		}
		
		_img.color = Color.white;

	}

 */

	string scrollMenu = "";
	float speedSubMenu = 1;

	public void ButtonPointerDown(string scrollMenu)
	{

		this.scrollMenu = scrollMenu;
		//if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();

		 
		if(DecorationsButtonsHolder.childCount > 3 ) speedSubMenu =  3f/(float)(DecorationsButtonsHolder.childCount-3);
		else  speedSubMenu =1;
	
	}
	
	public void ButtonPointerUp( )
	{
		scrollMenu = "";
		
	}


	
	void Update () {
		if(scrollMenu != "")
		{
			if(scrollMenu == "MoveLeft") SubMenuScrollLeft();
			else if(scrollMenu == "MoveRight") SubMenuScrollRight();

		}
	}
	
 
	void SubMenuScrollRight()
	{
	
		scrollRect.horizontalNormalizedPosition += speedSubMenu*Time.deltaTime;
		if(scrollRect.horizontalNormalizedPosition >1) scrollRect.horizontalNormalizedPosition = 1;
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.ButtonClick2);
	}
	
	void SubMenuScrollLeft()
	{
		 
		scrollRect.horizontalNormalizedPosition -= speedSubMenu*Time.deltaTime;
		if(scrollRect.horizontalNormalizedPosition <0) scrollRect.horizontalNormalizedPosition = 0;
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.ButtonClick2);
	}



	
 

}
