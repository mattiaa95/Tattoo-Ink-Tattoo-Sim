using UnityEngine;
using System.Collections;

/**
  * Scene:All
  * Object:SoundManager
  * Description: Skripta zaduzena za zvuke u apliakciji, njihovo pustanje, gasenje itd...
  **/
public class SoundManager : MonoBehaviour {

	public static int musicOn = 1;
	public static int soundOn = 1;
	public static bool forceTurnOff = false;


	 
	public AudioSource gameplayMusic;
	public AudioSource ButtonClick;
	public AudioSource ButtonClick2;
	public AudioSource PopUpShow;
	public AudioSource PopUpHide;
 
	public AudioSource SelectDecoration;
	 
	public AudioSource CottonDone;

	public AudioSource Coins;

	public AudioSource UnlockedDecoration;
	public AudioSource ShowDecorationMenu;
	public AudioSource HideDecorationMenu;
 
	public AudioSource CheckButton;
	public AudioSource ParticlesSound;
	public AudioSource Temperature;
	public AudioSource BloodPressure;
	public AudioSource Cotton;
	public AudioSource Needle;
	public AudioSource RemoveStencil;
	public AudioSource TattooMachine;
	public AudioSource Laser;


	float OriginalMusicVolume;

	static SoundManager instance;

	public static SoundManager Instance
	{
		get
		{
			if(instance == null)
			{
				instance = GameObject.FindObjectOfType(typeof(SoundManager)) as SoundManager;
			}

			return instance;
		}
	}

	void Start () 
	{

		OriginalMusicVolume = gameplayMusic.volume;
		DontDestroyOnLoad(this.gameObject);

		if(PlayerPrefs.HasKey("SoundOn"))
		{
			soundOn = PlayerPrefs.GetInt("SoundOn",1);
			if(SoundManager.soundOn == 0) MuteAllSounds();
			else UnmuteAllSounds();
		}
		else
		{
			SetSound(true);
		}

		musicOn = PlayerPrefs.GetInt("MusicOn",1);
		if(musicOn == 1)Play_Music();
		else Stop_Music();

		Screen.sleepTimeout = SleepTimeout.NeverSleep; 

	}

	public void SetSound(bool bEnabled)
	{
		if(bEnabled)
		{
			PlayerPrefs.SetInt("SoundOn", 1);
			UnmuteAllSounds();
		}
		else
		{
			PlayerPrefs.SetInt("SoundOn", 0);
			MuteAllSounds();
		}

		soundOn = PlayerPrefs.GetInt("SoundOn");
	}

	public void Play_ButtonClick()
	{
		if(ButtonClick.clip != null && soundOn == 1)
			ButtonClick.Play();
	}

//	public void Play_MenuMusic()
//	{
//		if(menuMusic.clip != null && musicOn == 1)
//			menuMusic.Play();
//	}
//
//	public void Stop_MenuMusic()
//	{
//		if(menuMusic.clip != null && musicOn == 1)
//			menuMusic.Stop();
//	}

	public void Play_Music()
	{
		if(gameplayMusic.clip != null && musicOn == 1 && !gameplayMusic.isPlaying)
		{
			gameplayMusic.volume = OriginalMusicVolume;
			gameplayMusic.Play();
		}
	}

	public void Stop_Music()
	{
		if(gameplayMusic.clip != null && musicOn == 1)
		{
			StartCoroutine(FadeOut(gameplayMusic, 0.1f));
		}
	}

 
 
//	public void Play_TaskCompleted()
//	{
//		if(ElementCompleted.clip != null&& soundOn == 1)
//			ElementCompleted.Play();
//	}

 

	public void Play_PopUpShow(float time = 0)
	{
		if(PopUpShow.clip != null && soundOn == 1)
			StartCoroutine(PlayClip(PopUpShow,time));
			 
	}

	public void Play_PopUpHide(float time = 0)
	{
		if(PopUpHide.clip != null && soundOn == 1)
			StartCoroutine(PlayClip(PopUpHide,time));
		
	}

	IEnumerator PlayClip(AudioSource Clip, float time)
	{
		yield return new WaitForSeconds(time);
		Clip.Play();
	}



	/// <summary>
	/// Corutine-a koja za odredjeni AudioSource, kroz prosledjeno vreme, utisava AudioSource do 0, gasi taj AudioSource, a zatim vraca pocetni Volume na pocetan kako bi AudioSource mogao opet da se koristi
	/// </summary>
	/// <param name="sound">AudioSource koji treba smanjiti/param>
	/// <param name="time">Vreme za koje treba smanjiti Volume/param>
	IEnumerator FadeOut(AudioSource sound, float time)
	{
		float originalVolume = sound.volume;

		if(sound.name == gameplayMusic.name) originalVolume = OriginalMusicVolume;


		while(sound.volume > 0.05f)
		{
			sound.volume = Mathf.MoveTowards(sound.volume, 0, time);
			yield return null;
		}
		sound.Stop();
		sound.volume = originalVolume;
	}

	/// <summary>
	/// F-ja koja Mute-uje sve zvuke koja su deca SoundManager-a
	/// </summary>
	public void MuteAllSounds()
	{
		foreach (Transform t in transform)
		{
			t.GetComponent<AudioSource>().mute = true;
		}
	}

	/// <summary>
	/// F-ja koja Unmute-uje sve zvuke koja su deca SoundManager-a
	/// </summary>
	public void UnmuteAllSounds()
	{
		foreach (Transform t in transform)
		{
			t.GetComponent<AudioSource>().mute = false;
		}
	}

	public void	Play_Sound(AudioSource sound)
	{
		if(!sound.isPlaying  && soundOn == 1) 
			sound.Play();
	}

	public void	StopAndPlay_Sound(AudioSource sound)
	{
		if(sound.isPlaying)
			sound.Stop();

		if( soundOn == 1) 
			sound.Play();
	}
	
	public void	Stop_Sound(AudioSource sound)
	{
		
		if(sound.isPlaying)
			sound.Stop();
	}
	
	 
	
}
