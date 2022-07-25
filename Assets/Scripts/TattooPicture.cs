using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class TattooPicture : MonoBehaviour {

	public Image [] Tattoos;

	public Sprite []  locationModels;
 
	public RawImage []  TattoosMan;
	public RawImage []  TattoosWoman;
	

	public GameObject[] Panels;
	
    Animator animChecked;
	
	bool bMan =false;
	int bodySideNo = 0;

	public GameObject PupUpMoreGames;
	public MenuManager menuManager;
	

	IEnumerator Start () {
		
	 
		
		if(GameData.selectedModel<0) GameData.selectedModel = 0;
		if(GameData.selectedModel == 1 || GameData.selectedModel == 3) bMan = true;
		
		int offset= (GameData.selectedModel)*4;
		for(int i = 0; i<4;i++)
		{
			Tattoos[i].sprite = locationModels[offset+i];
		}
		
		Input.multiTouchEnabled = false;
		

		
	

		for (int i = 0; i < 4; i++) 
		{

			if(i ==  (GameData.selectedLocation-1) ) 
			{
				Panels[i].SetActive(true);
				if(bMan) 
				{
					TattoosMan[(GameData.selectedLocation-1)].gameObject.SetActive(true);
					TattoosWoman[(GameData.selectedLocation-1)].gameObject.SetActive(false);

					TattoosMan[i].texture = GameData.UradjeneTetovaze[i] ;
					TattoosMan[i].transform.localRotation  =  Quaternion.Euler(0, 0,  GameData.locationRotZ);
					TattoosMan[i].transform.localScale = .8f*GameData.locationScale;
				}
				else
				{
					TattoosMan[(GameData.selectedLocation-1)].gameObject.SetActive(false);
					TattoosWoman[(GameData.selectedLocation-1)].gameObject.SetActive(true);

					TattoosWoman[i].texture = GameData.UradjeneTetovaze[i] ;
					TattoosWoman[i].transform.localRotation  =  Quaternion.Euler(0, 0,  GameData.locationRotZ);
					TattoosWoman[i].transform.localScale = .8f*GameData.locationScale;
				}
				animChecked = Panels[i].transform.GetChild(0).GetChild(6).GetComponent<Animator>();

			}
			else Panels[i].SetActive(false);
		}

		yield return  new WaitForSeconds(.3f);
		LevelTransition.Instance.ShowScene();
		yield return  new WaitForSeconds(1f);
		if(animChecked!=null) animChecked.Play("Checked");
		BlockClicks.Instance.SetBlockAll(true);
		BlockClicks.Instance.SetBlockAllDelay(1f,false);

		if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.ParticlesSound);

 
	}
	
	public void ButtonHomeClicked()
	{
		BlockClicks.Instance.SetBlockAll(true);
		LevelTransition.Instance.HideSceneAndLoadNext("HomeScene") ;
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
		GameData.IncrementButtonHomeClickedCount();
	}

	public void ButtonNextClicked()
	{
		//SoundManager.Instance.Play_Sound(SoundManager.Instance.SelectChild);
		//Tattoos[GameData.selectedLocation-1].transform.parent.parent.GetComponent<Animator>().SetBool("bSelected",true);
		Tattoos[GameData.selectedLocation-1].transform.parent.parent.GetComponent<Animator>().Play("selected");
		BlockClicks.Instance.SetBlockAll(true);
		StartCoroutine("LoadNext");
		GameData.IncrementButtonNextClickedCount();
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
	}

	IEnumerator LoadNext()
	{
		yield return new WaitForSeconds(1);
		LevelTransition.Instance.HideSceneAndLoadNext("Select Place");
	}

	public void ButtonMoreGamesClicked()
	{
		BlockClicks.Instance.SetBlockAll(true);
		menuManager.ShowPopUpMenu( PupUpMoreGames );
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
		
	}


}
