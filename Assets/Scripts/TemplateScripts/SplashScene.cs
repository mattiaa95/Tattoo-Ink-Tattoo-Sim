using UnityEngine;
using System.Collections;
using UnityEngine.UI;
 


public class SplashScene : MonoBehaviour {
	
	int appStartedNumber;
	AsyncOperation progress = null;
	Image progressBar;
	float myProgress=0;
	string sceneToLoad;

  


	// Use this for initialization
	void Start ()
	{
//		GameData.Init();
//		Shop.InitShop();

//		if(PlayerPrefs.HasKey("TutorialCompleted"))
//		{
		sceneToLoad =  "HomeScene";
//		}
//		else
//			sceneToLoad = "TutorialLevel";
		
		progressBar = GameObject.Find("ProgressBar").GetComponent<Image>();
		if(PlayerPrefs.HasKey("appStartedNumber"))
		{
			appStartedNumber = PlayerPrefs.GetInt("appStartedNumber");
		}
		else
		{
			appStartedNumber = 0;
		}
		appStartedNumber++;
		PlayerPrefs.SetInt("appStartedNumber",appStartedNumber);

		StartCoroutine(LoadScene());
	}
	

	IEnumerator LoadScene()
	{
		
		while(myProgress < 1)
		{
			myProgress += 0.05f;
			progressBar.fillAmount=myProgress;
			yield return new WaitForSeconds(0.15f);
		}

  
		Application.LoadLevel(sceneToLoad);
	}
 
}
