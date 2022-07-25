using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SelectModel : MonoBehaviour {

	//public Image SoundOn;
	public Image SoundOff;
 
	public MenuManager menuManager;
	public GameObject PupUpMoreGames;
 
	void Start () {
		
		Input.multiTouchEnabled = false;
		
		
	 
		LevelTransition.Instance.ShowScene();
		
		
		BlockClicks.Instance.SetBlockAll(true);
		BlockClicks.Instance.SetBlockAllDelay(1f,false);
		
	 
		
		if(SoundManager.soundOn == 1)
		{
			SoundOff.enabled = false;
			//SoundOn.enabled = true;
		}
		else
		{
			SoundOff.enabled = true;
			//SoundOn.enabled = false;
		}
	}
	
	
	 
	
	public void btnSoundClicked()
	{
		
		if(SoundManager.soundOn == 1)
		{
			SoundOff.enabled = true;
			//SoundOn.enabled = false;
			SoundManager.soundOn = 0;
			SoundManager.Instance.MuteAllSounds();
			
		}
		else
		{
			SoundOff.enabled = false;
			//SoundOn.enabled = true;
			SoundManager.soundOn = 1;
			SoundManager.Instance.UnmuteAllSounds();
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
			
		}
		
		
		if(SoundManager.musicOn == 1)
		{
			SoundManager.Instance.Stop_Music();
			SoundManager.musicOn = 0;
		}
		else
		{
			SoundManager.musicOn = 1;
			SoundManager.Instance.Play_Music();
		}
		
		PlayerPrefs.SetInt("SoundOn",SoundManager.soundOn);
		PlayerPrefs.SetInt("MusicOn",SoundManager.musicOn);
		PlayerPrefs.Save();
		
		
		BlockClicks.Instance.SetBlockAll(true);
		BlockClicks.Instance.SetBlockAllDelay(.3f,false);
		
	}
	
  
	 
	public void btnHomeClicked()
	{
		
//		if(SoundManager.Instance!=null) 
//		{
//			SoundManager.Instance.Play_ButtonClick();
//			SoundManager.Instance.Stop_Sound(SoundManager.Instance.CottonMachine);
//		}
//		StopAllCoroutines();
		
		LevelTransition.Instance.HideSceneAndLoadNext("HomeScene");
		BlockClicks.Instance.SetBlockAll(true);
		BlockClicks.Instance.SetBlockAllDelay(1f,false);
		
		GameData.IncrementButtonHomeClickedCount();
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
	}

 
	public void ButtonMoreGamesClicked()
	{
		BlockClicks.Instance.SetBlockAll(true);
		menuManager.ShowPopUpMenu( PupUpMoreGames );
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
		
	}
	
}