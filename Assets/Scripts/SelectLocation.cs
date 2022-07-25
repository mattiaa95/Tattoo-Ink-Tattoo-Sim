using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SelectLocation : MonoBehaviour {

	public Image [] Tattoos;
	public Sprite FrameSelected;
	public Sprite NormalFrame;

	public Sprite []  locationModels;

	public RawImage []  TattoosMan;
	public RawImage []  TattoosWoman;

	//za ikonice na dugmicima za izbor lokacije 
	public RawImage []  TattoosMan2;
	public RawImage []  TattoosWoman2;



	public CanvasGroup [] bodySides;

	public GameObject[] Panels;

	public GameObject ButtonNext;
	public GameObject ButtonFlipModel;

	bool bMan =false;
	int bodySideNo = 0;



	void Start () {

		ButtonNext.SetActive(false);
		ButtonFlipModel.SetActive(false);

		if(GameData.selectedModel<0) GameData.selectedModel = 0;
		if(GameData.selectedModel == 1 || GameData.selectedModel == 3) bMan = true;

		int offset= (GameData.selectedModel)*4;
		for(int i = 0; i<4;i++)
		{
			Tattoos[i].sprite = locationModels[offset+i];
		}

		Input.multiTouchEnabled = false;
 
		LevelTransition.Instance.ShowScene();
 
		BlockClicks.Instance.SetBlockAll(true);
		BlockClicks.Instance.SetBlockAllDelay(1f,false);

		int tattoosDone = 0;

		for (int i = 0; i < 4; i++) 
		{
			if(GameData.UradjeneTetovaze[i] !=null)	
			{
				if(bMan) 
				{
					TattoosMan[i].texture = GameData.UradjeneTetovaze[i] ;
					TattoosMan[i].transform.localScale = .7f* GameData.UradjeneTetovazeScale[i];
					TattoosMan[i].transform.localRotation =  Quaternion.Euler(0, 0,   GameData.UradjeneTetovazeRotZ[i]); 

					TattoosMan2[i].gameObject.SetActive(true);
					TattoosMan2[i].texture = GameData.UradjeneTetovaze[i] ;
					TattoosMan2[i].transform.localScale =  GameData.UradjeneTetovazeScale[i];
					TattoosMan2[i].transform.localRotation =  Quaternion.Euler(0, 0,   GameData.UradjeneTetovazeRotZ[i]); 
				}
				else
				{
					TattoosWoman[i].texture = GameData.UradjeneTetovaze[i] ;
					TattoosWoman[i].transform.localScale = .7f* GameData.UradjeneTetovazeScale[i];
					TattoosWoman[i].transform.localRotation = Quaternion.Euler(0, 0,   GameData.UradjeneTetovazeRotZ[i]); 

					TattoosWoman2[i].gameObject.SetActive(true);
					TattoosWoman2[i].texture = GameData.UradjeneTetovaze[i] ;
					TattoosWoman2[i].transform.localScale =  GameData.UradjeneTetovazeScale[i];
					TattoosWoman2[i].transform.localRotation =  Quaternion.Euler(0, 0,   GameData.UradjeneTetovazeRotZ[i]); 
				}
				tattoosDone++;
			}
			else 
			{
				if(bMan) TattoosMan[i].color = new Color(1,1,1,0);
				else TattoosWoman[i].color = new Color(1,1,1,0);
			}

			Panels[i].transform.GetChild(0).GetChild(4).gameObject.SetActive( (GameData.UradjeneTetovaze[i] !=null) );

		}
		if( tattoosDone == 4)
		{
			ButtonNext.SetActive(false);
			ButtonFlipModel.SetActive(true);

			for (int i = 0; i < 4; i++) 
			{ 
				Panels[i].gameObject.SetActive(false);
			}
			Panels[4].gameObject.SetActive(true);

			if(bMan)
			{
				bodySideNo = 0;
				bodySides[0].gameObject.SetActive(true);
				bodySides[0].alpha = 1;
				bodySides[1].alpha = 0; 
			
				bodySides[1].gameObject.SetActive(false);
				bodySides[2].gameObject.SetActive(false);
				bodySides[3].gameObject.SetActive(false);
			}
			else
			{
				bodySideNo = 2;
				bodySides[2].gameObject.SetActive(true);
				bodySides[2].alpha = 1;
				bodySides[3].alpha = 0;

				bodySides[1].gameObject.SetActive(false);
				bodySides[0].gameObject.SetActive(false);
				bodySides[3].gameObject.SetActive(false);
			}
		}
		else
		{
			Panels[4].gameObject.SetActive(false);
		}
	}
	
 

	public void SelectTattooLocation(int location)
	{
		
		BlockClicks.Instance.SetBlockAll(true);
		BlockClicks.Instance.SetBlockAllDelay(.3f,false);
 
		//prethodno selektovani se vraca
		if(ButtonNext.activeSelf == true) Tattoos[GameData.selectedLocation-1].transform.parent.GetComponent<Image>().sprite =  NormalFrame;

		GameData.selectedLocation = location;
		Tattoos[location-1].transform.parent.GetComponent<Image>().sprite =  FrameSelected;

	
		ButtonNext.SetActive(true);
	 
		if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound( SoundManager.Instance.CheckButton);
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


	public void ButtonChangeSideClicked()
	{
		BlockClicks.Instance.SetBlockAll(true);
		BlockClicks.Instance.SetBlockAllDelay(.4f,false);
		int nextBS;

		if(bMan)
		{
			if(bodySideNo == 1) nextBS = 0;
			else nextBS = 1;

			bodySides[0].gameObject.SetActive(true);
			bodySides[0].alpha = 1;
			bodySides[1].alpha = 0;
		}
		else
		{
			if(bodySideNo == 3) nextBS = 2;
			else nextBS = 3;

			bodySides[2].gameObject.SetActive(true);
			bodySides[2].alpha = 1;
			bodySides[3].alpha = 0;
		}
		StopAllCoroutines();
		StartCoroutine(ChangeSide(bodySideNo,nextBS));
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
	}

	IEnumerator ChangeSide(int curent, int next)
	{
		float alpha = 0;
		bodySides[next].gameObject.SetActive(true);
		bodySides[next].alpha = 0;
		while(alpha<1)
		{
			alpha += Time.fixedDeltaTime*2;

			bodySides[curent].alpha = 1-alpha;
			bodySides[next].alpha =  alpha;
			yield return new WaitForFixedUpdate();


		}
		bodySides[bodySideNo].alpha = 0;
		bodySides[bodySideNo].gameObject.SetActive(false);
		bodySideNo = next;
	}
	
	IEnumerator LoadNext()
	{
		yield return new WaitForSeconds(1);
		LevelTransition.Instance.HideSceneAndLoadNext("TattooSalon");
	}


	public void ButtonHomeClicked()
	{
		BlockClicks.Instance.SetBlockAll(true);
		LevelTransition.Instance.HideSceneAndLoadNext("HomeScene") ;
		GameData.IncrementButtonHomeClickedCount();
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
	}
 
}
