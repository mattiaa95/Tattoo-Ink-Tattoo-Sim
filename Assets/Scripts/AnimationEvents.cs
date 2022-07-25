using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AnimationEvents : MonoBehaviour {

	public void CleaningAnimationFinished()
	{
		transform.parent.parent.SendMessage("CleaningAnimationFinised",SendMessageOptions.DontRequireReceiver);
	}

	public void InsertFlavorAnimationFinished()
	{
		Camera.main.SendMessage("StartTutorial",SendMessageOptions.DontRequireReceiver);
	}

	public void  MachineStartingAnimationFinished()
	{
		 
		Camera.main.SendMessage("MachineStarted",SendMessageOptions.DontRequireReceiver);
	}
 



	public void StartParticles()
	{
		transform.GetComponentInChildren<ParticleSystem>().Play();
	}
		
	 public void AnimThermometerIndicatorOn()
	{
		StartCoroutine("ThermometerIndicator");

	}
	 
	IEnumerator ThermometerIndicator()
	{
		Text thermometerInd = transform.GetChild(0).GetChild(0).GetComponent<Text>();
		for(int i = 0; i<10;i++)
		{
			yield return new WaitForSeconds(0.1f);
			thermometerInd.text  = ( 37+ Random.Range(-1.3f,1.3f)).ToString("00.0");

		}
		yield return new WaitForSeconds(0.1f);
		thermometerInd.text  = (36.4f+ Random.Range( .1f,.4f)).ToString("00.0");
	}

	public void AnimThermometerEnd()
	{
		transform.parent.GetComponent<Instrument>().AnimExamEnd(3);
		
	}

	public void AnimStethoscopeEnd()
	{
		transform.parent.GetComponent<Instrument>().AnimExamEnd(2);
		
	}

	public void AnimBloodPressureFingerPress()
	{
		transform.parent.GetComponent<Instrument>().AnimFingerPressEnd();
		
	}

	public void AnimBloodPressureEnd()
	{
		transform.parent.GetComponent<Instrument>().AnimExamEnd(1);
		
	}

	public void AnimDropFromBottleEnd()
	{
		transform.parent.SendMessage("AnimCottonBottleEnd",SendMessageOptions.DontRequireReceiver);
		
	}

	public void AnimCottonEnd()
	{
		transform.parent.SendMessage("AnimExamEnd",4,SendMessageOptions.DontRequireReceiver);
		
	}

	public void AnimInjectionEnd()
	{
		transform.parent.GetComponent<Instrument>().AnimExamEnd(5);
		
	}



	public void AnimCardiograph()
	{
//		Debug.Log("PULSE OK");
		 gameObject.SetActive(false);
	}


	public void AnimRemoveBloodEnd()
	{
		transform.parent.GetComponent<Cotton_RemoveTattoo>().AnimExamEnd(1);
		
	}

	public TattooMachine tattooMachine;
	public void AnimRemoveBloodStart()
	{
		tattooMachine.HideBlood();
	}
}
