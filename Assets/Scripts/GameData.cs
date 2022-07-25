using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameData : MonoBehaviour {


	public static int selectedModel = 0;
	public static int selectedLocation = 0;



	public static int SelectedShape = 0;
	public static int SelectedStick = 0;
	public static int SelectedFlavor = 0;
 

	public static Texture2D[] UradjeneTetovaze = new Texture2D[4];

	public static Vector3[] UradjeneTetovazeScale = new Vector3[4];
	public static float[] UradjeneTetovazeRotZ  = new float[4];


	//podesavanja za tetovaze
	public static float  locationRotZ = 0;
	public static Vector3  locationScale  = Vector3.one; 

 

  
	public static string Unlocked = "dafE1A";
	 
 

	public static string sTestiranje = "";

  
	public static void Init()
	{
		//-----------------------------------------------------------------
 
# if UNITY_EDITOR
		if( true  ) 
		{
			sTestiranje = "Test;"
			 // + "SpecialOffer;"
			// 	+ "OverrideShopCall;"  
			//	+ "TestPopUpTransaction;"	
			//	+ "WatchVideo;"
			//	+ "FreeStar;";
			//	+ "InternetOff;"
			 ;

			Debug.Log("TESTIRANJE UKLJUCENO: " + sTestiranje);
		}
		//-----------------------------------------------------------------------
#endif
 
		//GetUnlocekedItems();
 
	}

	public static void SaveUnlocekedItemsToPP(int dataNo, ref bool[] unlockedItems )
	{
		string data= "";
		for(int i=0; i<unlockedItems.Length;i++)
		{
			if(unlockedItems[i])
				data += (i.ToString()+";");

		}

		data = data.Remove(data.Length-1,1);
		//Debug.Log(data);

		PlayerPrefs.SetString("Data"+dataNo.ToString(),data);
	}

	 
	 
 

	/// <summary>
	/// Gets the unloceked items.
	/// </summary>
	/// <param name="group">1 tetovaze , 2 boje.</param>
	public static void GetUnlocekedItems( int group, ref bool[] unlockedItems )
	{
		string unlocked  = PlayerPrefs.GetString("Data"+group.ToString(),"");
		if(unlocked != "")
			SetUnlockedFromString(ref unlockedItems, unlocked);

	}


	 

	static void SetUnlockedFromString( ref bool[] unlockedItems, string data)
	{
		if(data != "")
		{
			string[] pom = data.Split(';');
			for(int i = 0; i< pom.Length;i++)
			{
				int item = 0;
				int.TryParse(pom[i],out item);
				if(item < unlockedItems.Length) unlockedItems[item] = true;
			}
		}
	}

	public static void ClearTattoos()
	{
		UradjeneTetovaze[0] = null;
		UradjeneTetovaze[1] = null;
		UradjeneTetovaze[2] = null;
		UradjeneTetovaze[3] = null;

	}
 
	public static void IncrementButtonNextClickedCount()
	{
		Yodo1Ads.Instance.ShowInterstitial();
	}
 

	public static void IncrementButtonHomeClickedCount()
	{
		Yodo1Ads.Instance.ShowInterstitial();
	}

 
}

public class GameItemData 
{
	public string name = "";
	public int stars = 0;
	public bool unlocked = false;
	public string code = "";

	public GameItemData( string name, int stars, string code)
	{
		this.name = name;
		this.stars = stars;
		this.code = code;
	}
	
} 


	
