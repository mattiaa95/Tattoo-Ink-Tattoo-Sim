using UnityEngine;
using UnityEngine.UI;
using System.Collections;
 

public class HomeScene : MonoBehaviour {

	 
	public Image SoundOff;
 
	public MenuManager menuManager;
	public GameObject PupUpRate;
	public GameObject PupUpMoreGames;
	public GameObject ButtonRate;

	void Awake()
	{
		Input.multiTouchEnabled = false;
	}

	void Start () {

		if( PlayerPrefs.GetInt("alreadyRated",0) == 1) ButtonRate.SetActive(false);
		 

		Input.multiTouchEnabled = false;

		 
		
 
		LevelTransition.Instance.ShowScene();
		 

		BlockClicks.Instance.SetBlockAll(true);
		BlockClicks.Instance.SetBlockAllDelay(1f,false);
 
	 
		if(SoundManager.soundOn == 1)
		{
			SoundOff.enabled = false;
	 
		}
		else
		{
			SoundOff.enabled = true;
 
		}
	}

	
	public void ExitGame () 
    {
		Application.Quit();
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
 
	 
 
	public void EndWatchingVideo()
	{
	 
		//GameData.WatchVideoCounter = 0;
		//AddStar();
		//GameObject.Destroy(BtnWatchVideo);
	}


	public void btnPlayClick( )
	{
		 
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
		StopAllCoroutines();
		
		LevelTransition.Instance.HideSceneAndLoadNext("SelectModel");
		//Application.LoadLevel("SelectModel");
		BlockClicks.Instance.SetBlockAll(true);
		BlockClicks.Instance.SetBlockAllDelay(1f,false);
	}

	//********************************************

	
	public void btnRateClick( )
	{
		if(Rate.alreadyRated==0 )
		{
			menuManager.ShowPopUpMenu( PupUpRate );
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
		}
	}

	public  void LevelTransitionOn()
	{
		 
		if(Application.loadedLevelName == "HomeScene") 
		{
			

		}
		 
	}
	
	public   void LevelTransitionOff()
	{
		if(Application.loadedLevelName == "HomeScene") 
		{

		}
	}
 


 
	public void ButtonMoreGamesClicked()
	{
		BlockClicks.Instance.SetBlockAll(true);
		menuManager.ShowPopUpMenu( PupUpMoreGames );
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();

	}
 
}
