using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InkMenuScript : MonoBehaviour {
	
	public TattooSalonGameplay gameplay;

	public HorizontalLayoutGroup hlgDecotrations;
	public Transform InkButtonsHolder;
 
	public ScrollRect scrollRect;
	 
	public Color[] InkColors;
	bool [] unlocked = {  true,false,true,false,true,false,true,false,true,false,   true,false,true,false,true,false,true,false,true,false   ,true,false   }; // 22 //da li je bocica otkljucana
	int inkToUnlock = 0;
  
	void Awake () 
	{
		for (int i = 0; i < InkButtonsHolder.childCount; i++) 
		{
			InkButtonsHolder.GetChild(i).GetChild(0).GetComponent<Image>().color = InkColors[i];
		}
	}

	void Start () 
	{
		GameData.GetUnlocekedItems(2, ref unlocked);
		SetButtons();
	}
 
 
	void SetButtons()
	{
		for (int i = 0; i < InkButtonsHolder.childCount; i++) 
		{
			InkButtonsHolder.GetChild(i).GetChild(2).GetComponent<Image>().enabled =  !unlocked[i];
		}
	}
 
	//--------------------kreiranje dekoracije---------------------------------------------------------------
	public void ButtonSelectDecoration(int inkIndex)
	{
		inkIndex--;
		BlockClicks.Instance.SetBlockAll(true);
		BlockClicks.Instance.SetBlockAllDelay(.5f,false);


		if(  !unlocked[inkIndex] ) 
		{
			inkToUnlock = inkIndex;
            Yodo1Ads.Instance.bVideoInk = true;
            Yodo1Ads.Instance.bVideoTattoo = false;
            Yodo1Ads.Instance.ShowVideoReward();
			return; 
			
		}
		else
		{
			//Debug.Log("select"); 
			gameplay.SetInk( InkColors[inkIndex]);
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
		}

		 

	}

	public void EndWatchingVideoDecoration()
	{
		//Debug.Log("UNLOCK DECORATION: "+   ",  "+ inkToUnlock);
		unlocked[inkToUnlock] = true;
 
		InkButtonsHolder.GetChild(inkToUnlock).GetChild(2).GetComponent<Image>().enabled =  !unlocked[inkToUnlock];
		GameData.SaveUnlocekedItemsToPP(2 , ref unlocked );
		if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound( SoundManager.Instance.UnlockedDecoration);

	}

	 
 
	string scrollMenu = "";
	float speedSubMenu = 1;

	public void ButtonPointerDown(string scrollMenu)
	{
		this.scrollMenu = scrollMenu;

		if(InkButtonsHolder.childCount > 4 ) speedSubMenu =  4f/(float)(InkButtonsHolder.childCount-4);
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
