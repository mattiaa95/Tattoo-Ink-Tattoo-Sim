using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class EscapeButtonManager : MonoBehaviour {

	bool bDisableEsc = false;
	public static  Stack<string> EscapeButonFunctionStack = new Stack<string>();

	void Start () {
		DontDestroyOnLoad (this.gameObject);
	
	}

	void OnLevelWasLoaded(int level) {

		EscapeButonFunctionStack.Clear();
		bDisableEsc = false;
		if(  Application.loadedLevelName == "HomeScene") AddEscapeButonFunction("ExitGame");
		if(  Application.loadedLevelName.StartsWith("Room"))  AddEscapeButonFunction("btnHomeClicked");
		if(  Application.loadedLevelName.StartsWith( "MiniGame") ) AddEscapeButonFunction("ButtonFinishClicked");

		//if(Application.loadedLevelName == "Map") AddEscapeButonFunction("btnBackClick");
		//if(Application.loadedLevelName == "Room") AddEscapeButonFunction("btnPauseClick");
	}
	 
	public static void  AddEscapeButonFunction( string functionName, string functionParam = "")
	{
		if(functionParam != "") functionName +="*"+functionParam;
		EscapeButonFunctionStack.Push(functionName);
	}




	void Update()
	{
		if( Input.GetKeyDown(KeyCode.P) )
		{
			//Debug.Log("esc stack count "+  EscapeButonFunctionStack.Count  );
			//if(  EscapeButonFunctionStack.Count > 0 ) Debug.Log(  EscapeButonFunctionStack.Peek() );
		}
		//if(  EscapeButonFunctionStack.Count > 0 ) Debug.Log( EscapeButonFunctionStack.Count + "    " + EscapeButonFunctionStack.Peek() );
		
		if( !bDisableEsc  && Input.GetKeyDown(KeyCode.Escape) )
		{
			//if(  EscapeButonFunctionStack.Count > 0 )  Debug.Log( EscapeButonFunctionStack.Count + "    " + EscapeButonFunctionStack.Peek() );
			
			if(EscapeButonFunctionStack.Count>0)
			{
				bDisableEsc = true;

				if( EscapeButonFunctionStack.Peek().Contains("*") )
				{
					string[] funcAndParam = EscapeButonFunctionStack.Peek().Split('*');
					if(funcAndParam[0] == "ClosePopUpMenuEsc") 
					{
						GameObject.Find("Canvas").SendMessage(funcAndParam[0],funcAndParam[1], SendMessageOptions.DontRequireReceiver);
					}
					else
					{
						Camera.main.SendMessage(funcAndParam[0],funcAndParam[1], SendMessageOptions.DontRequireReceiver);
						EscapeButonFunctionStack.Pop();
					}
				}
				else
				{
					if( EscapeButonFunctionStack.Count == 1 && EscapeButonFunctionStack.Peek() == "btnPauseClick" ) 
						Camera.main.SendMessage("btnPauseClick", SendMessageOptions.DontRequireReceiver); //pauza se ne uklanja iz staka ako je na prvom mestu
					else if(EscapeButonFunctionStack.Count >= 1 && EscapeButonFunctionStack.Peek() == "CloseDailyReward") 
					{
						GameObject.Find("PopUps/DailyReward").GetComponent <DailyRewards>().Collect();
					}
					else 
						Camera.main.SendMessage(EscapeButonFunctionStack.Pop(), SendMessageOptions.DontRequireReceiver);
				}
			} 
			StartCoroutine("DisableEsc");
		}
	}
	
	IEnumerator DisableEsc()
	{
		yield return new WaitForSeconds(2);
		bDisableEsc = false;
	}


}



