using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/**
  * Scene: Sve
  * Object: Menu objekti
  * Description: Skripta zaduzena za Menu-je
  **/
public class Menu : MonoBehaviour {


	private Animator _animtor;
	CanvasGroup BlockAll;

//	public bool IsOpen
//	{
//		get
//		{
//			return _animtor.GetBool("IsOpen");
//		}
//		set
//		{
//			_animtor.SetBool("IsOpen", value);
//		}
//	}

	// Use this for initialization
	public void Awake () 
	{
		_animtor = GetComponent<Animator> ();

		var rect = GetComponent<RectTransform> ();
		rect.offsetMax = rect.offsetMin = new Vector2 (0, 0);
	}

	public void  Start()
	{
		BlockAll = GameObject.Find("Canvas/BlockAll").GetComponent<CanvasGroup>();
	}

	public void ResetObject()
	{
		gameObject.SetActive (false);
	}
	
	public void DisableObject(string gameObjectName)
	{
		GameObject gameObject= GameObject.Find (gameObjectName);
		if (gameObject != null) 
		{
			if (gameObject.activeSelf) 
			{
				gameObject.SetActive (false);
			}
		}
	}


	public void OpenMenu()
	{
	 
		if(BlockAll == null)   BlockAll = GameObject.Find("BlockAll").transform.GetComponent<CanvasGroup>();
		_animtor.SetTrigger("tOpen");
		_animtor.ResetTrigger("tClose");
		BlockAll.blocksRaycasts = true;
	}

	public void CloseMenu()
	{
		if(BlockAll == null)   BlockAll = GameObject.Find("BlockAll").transform.GetComponent<CanvasGroup>();
		_animtor.SetTrigger("tClose");
		_animtor.ResetTrigger("tOpen");
		BlockAll.blocksRaycasts = true;
	}

	public void MenuClosed()
	{
		if(BlockAll == null)   BlockAll = GameObject.Find("BlockAll").GetComponent<CanvasGroup>();
//		Debug.Log("Menu closed");
		BlockAll.blocksRaycasts = false;
		ResetObject();
	}

	public void MenuOpened()
	{
		if(BlockAll == null)   BlockAll = GameObject.Find("BlockAll").GetComponent<CanvasGroup>();
	//	Debug.Log("Menu opened");
		BlockAll.blocksRaycasts = false;
	}
	
}
