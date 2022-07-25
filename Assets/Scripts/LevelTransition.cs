using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
 
public class LevelTransition : MonoBehaviour {

 
	Animator anim;
 
	public static LevelTransition Instance;
	static string nextLevelName = "";
	bool bLoadScene = false;  

	void Start () 
	{
		DontDestroyOnLoad(this.gameObject);
		anim = transform.GetComponent<Animator>();
	}

	void Awake()
	{
		if(nextLevelName != "" &&  Application.loadedLevelName == "TransitionScene") 
		{
//			Debug.Log("Transition SCENE");
			Application.LoadLevel(nextLevelName);
			 
			return;
		}
		else
		{
			if(Instance !=null && Instance != this ) GameObject.Destroy(gameObject);
			else 
                Instance = this;
		}
	}



	public void HideSceneAndLoadNext(string levelName)
	{
		if(bLoadScene) return;
		bLoadScene = true;
		nextLevelName = levelName;
		StopAllCoroutines();
		//StartCoroutine(SetBlockAll(0,true));
		BlockClicks.Instance.SetBlockAll(true);

		StartCoroutine("LoadScene" , levelName);
		anim.SetBool("bClose",true);

	}

	public void ShowScene()
	{
		//StartCoroutine(SetBlockAll(0,true));
		if(anim !=null )anim.SetBool("bClose",false);

		//if(SceneManager.GetActiveScene().name == "HomeScene") { StartCoroutine(SetBlockAll(6f,false));}
		//else 
		//	StartCoroutine(SetBlockAll(1.0f,false));
		BlockClicks.Instance.SetBlockAll(true);
		BlockClicks.Instance.SetBlockAllDelay(1f,false);
	}

	 
	IEnumerator LoadScene (string levelName)
	{
		yield return new WaitForSeconds(1.2f);
		bLoadScene = false;
 
		Application.LoadLevel("TransitionScene");
	}

	public void HideAndShowSceneWithoutLoading( )
	{
		StopAllCoroutines();
		//StartCoroutine(SetBlockAll(0,true));
		BlockClicks.Instance.SetBlockAll(true);
		 
		anim.SetBool("bClose",true);
		StartCoroutine("WaitHideAndShowScene");
	}

	IEnumerator WaitHideAndShowScene ( )
	{
		yield return new WaitForSeconds(1.2f);
		anim.SetBool("bClose",false);
		//StartCoroutine(SetBlockAll(1,false));
		BlockClicks.Instance.SetBlockAll(true);
		BlockClicks.Instance.SetBlockAllDelay(1f,false);
	}
}

