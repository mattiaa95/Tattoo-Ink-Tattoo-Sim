using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HospitalScene : MonoBehaviour {

	public bool bBloodPressureOK= false;
	public bool bHeartOK= false;
	public bool bTemperatureOK= false;
	public bool bCottonOK= false;
	public bool bInjectionOK= false;

	public GameObject ButtonExam1;
	public GameObject ButtonExam2;
	public GameObject ButtonExam3;
	public GameObject ButtonExam4;
	public GameObject ButtonExam5;
 
	public GameObject ButtonNext;

	public Animator ButtonsHolderAnim;
	public Animator MedicalRecord;
	public GameObject[] ExamCheck;

	bool bSecondExam = false;

	public bool bModelMan = true;

	public GameObject[] Characters;
	GameObject activeCharacter;
	public Image MouthOpen;

	public GameObject PupUpMoreGames;
	public MenuManager menuManager;

	void Awake()
	{
		bModelMan = true;
		if(GameData.selectedModel == 0 || GameData.selectedModel ==2) bModelMan = false;

//		Debug.Log("selectedModel "+ GameData.selectedModel + "  b" + bModelMan);
		Characters[GameData.selectedModel].SetActive(true);
		activeCharacter = Characters[GameData.selectedModel];
		for (int i = 0; i < Characters.Length; i++) {
			if( i !=GameData.selectedModel ) GameObject.Destroy( Characters[i] );
		}
	}

	// Use this for initialization
	void Start  () 
	{

		MouthOpen = activeCharacter.transform.Find("MouthOpen").GetComponent<Image>();
		 MouthOpen.enabled = false;
		ButtonNext.SetActive(false);

		ButtonExam1.SetActive(true);
		ButtonExam2.SetActive(true);
		ButtonExam3.SetActive(true);

		ButtonExam4.SetActive(false);
		ButtonExam5.SetActive(false);

		if(ButtonsHolderAnim== null) ButtonsHolderAnim = GameObject.Find("Canvas/Menus/MainMenu/BottomButtonsHolder").GetComponent<Animator>();
		ButtonsHolderAnim.SetBool("bShow",true);
 
		LevelTransition.Instance.ShowScene();
		BlockClicks.Instance.SetBlockAll(true);
		BlockClicks.Instance.SetBlockAllDelay(1f,false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ExamOver(int exam)
	{
		if(exam == 1) bBloodPressureOK = true;
		else if(exam == 2) bHeartOK = true;
		else if(exam == 3) {  MouthOpen.enabled = false; bTemperatureOK = true; }
		else if(exam == 4) bCottonOK = true;
		else if(exam == 5)  {  MouthOpen.enabled = false;bInjectionOK = true; }

		if(!bSecondExam && bBloodPressureOK && bHeartOK && bTemperatureOK )
		{
			bSecondExam = true;
			StartCoroutine("ChangeButtons");
		}

		if(exam <=3)
		{
			// animiranje zdravstvenog kartona
			MedicalRecord.Play ("MedicalRecordShow");
			StartCoroutine("CheckExamOK",exam);
		}
		else if(exam == 5)
		{
			StartCoroutine("AllExamsCompleted");
		}


	}

	IEnumerator CheckExamOK(int exam)
	{
		ExamCheck[exam-1].SetActive(true);
		yield return new WaitForSeconds(.3f);
		if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound( SoundManager.Instance.CheckButton);


		yield return new WaitForSeconds(1.2f);
		MedicalRecord.Play ("MedicalRecordHide");

	}


	IEnumerator ChangeButtons()
	{
		yield return new WaitForSeconds(1.1f);
		ButtonsHolderAnim.SetBool("bShow",false);
		//if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound( SoundManager.Instance.HideDecorationMenu);
		yield return new WaitForSeconds(0.5f);
	
		ButtonExam1.SetActive(false);
		ButtonExam2.SetActive(false);
		ButtonExam3.SetActive(false);
		
		ButtonExam4.SetActive(true);
		ButtonExam5.SetActive(true);
		ButtonsHolderAnim.SetBool("bShow",true);
		//if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound( SoundManager.Instance.ShowDecorationMenu);
	}


	IEnumerator AllExamsCompleted()
	{
		yield return new WaitForSeconds(0.5f);
		ButtonNext.SetActive(true);
		if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound( SoundManager.Instance.ParticlesSound);

		yield return new WaitForSeconds(3.5f);
		ButtonsHolderAnim.SetBool("bShow",false);


	} 

	public void ButtonHomeClicked()
	{
		BlockClicks.Instance.SetBlockAll(true);
		LevelTransition.Instance.HideSceneAndLoadNext("HomeScene") ;
		GameData.IncrementButtonHomeClickedCount();
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
	}


	public void ButtonNextClicked()
	{
		BlockClicks.Instance.SetBlockAll(true);
		 
		StartCoroutine("LoadNext");
		GameData.IncrementButtonNextClickedCount();
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
	}
	
	IEnumerator LoadNext()
	{
		yield return new WaitForSeconds(1);
		LevelTransition.Instance.HideSceneAndLoadNext("Select Place") ;
	}

	
	public void FreezeTime()
	{
		Time.timeScale = 0;
	}
	
	public void UnfreezeTime()
	{
		Time.timeScale = 1;
	}
 
	public void ButtonMoreGamesClicked()
	{

		BlockClicks.Instance.SetBlockAll(true);
		FreezeTime();
		menuManager.ShowPopUpMenu( PupUpMoreGames );
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
		
	}

}
