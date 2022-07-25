using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ApplyStencil : MonoBehaviour ,IBeginDragHandler , IDragHandler, IEndDragHandler,  IPointerDownHandler 
{

	public TattooSalonGameplay gameplay;
	public ProgressBar progressBar;
	Animator animStencilPaper;

	bool bPeelStencil = false;
	Vector3 diffPos;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		 if(gameplay.Phase == 5 && !bPeelStencil ) //vracanje papira 
		{
			animMagnitude = Mathf.Lerp(animMagnitude,0, Time.deltaTime*5);
			
			animStencilPaper.Play("PeelPaper",-1,animMagnitude);
			animStencilPaper.speed =0;
		}

	}



	public void OnPointerDown( PointerEventData eventData)
	{
		if(gameplay.Phase == 2 || gameplay.Phase == 3 )
		{
			Debug.Log("APPLY");
			progressBar.SetProgress(progressBar.Value + 0.01f);
			gameplay.Phase = 3;

		}

		if(gameplay.Phase == 4 )
		{
			animStencilPaper = gameplay.animStencilPaper;
			gameplay.Phase = 5;
			gameplay.animTutorial.gameObject.SetActive(false);
		}
 
	}
	
	 
	 
	public void OnBeginDrag (PointerEventData eventData)
	{
 
		if( gameplay.Phase == 5) 
		{
			diffPos  =gameplay.TutorialPositions[1].position - Camera.main.ScreenToWorldPoint(Input.mousePosition)   ;
			diffPos = new Vector3(diffPos.x,diffPos.y,0);
 
			if(diffPos.magnitude < 1.05f) //pokretanje slidanja papira
			{
				animMagnitude =0;
				bPeelStencil = true;

			}
		}
	}
	
	public void OnEndDrag (PointerEventData eventData)
	{
		if( gameplay.Phase == 5) bPeelStencil = false;
	}
	

	float animMagnitude = 0;

	public void OnDrag (PointerEventData eventData)
	{
		if(gameplay.Phase == 2 || gameplay.Phase == 3 )
		{
			if( eventData.pointerCurrentRaycast.gameObject!=null && eventData.pointerCurrentRaycast.gameObject.name == transform.name)
			{
				if(progressBar.Value<1)
				{
					progressBar.SetProgress(progressBar.Value + Time.deltaTime*0.7f); //brzina stavljanaj stencila
					if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.Cotton);
				}
				else
				{
					gameplay.StencilApplied();
				}
			}
		}
		else if(bPeelStencil )
		{
			Vector3 pom  = (gameplay.TutorialPositions[1].position - Camera.main.ScreenToWorldPoint(Input.mousePosition));
			pom = new Vector3(pom.x,pom.y,0);
		 
			if((diffPos  -pom).x<0  && (diffPos  -pom).y>0)
			{
				animMagnitude =   Mathf.Lerp(animMagnitude,(diffPos  -pom).magnitude*2 , Time.deltaTime );      //brzina skidanja papira
 
				if(animMagnitude>0.4f ) //animacija krece automatski do kraja
				{
				 
					bPeelStencil = false;
					animStencilPaper.Play("PeelPaper",-1,animMagnitude);
					animStencilPaper.speed =1;
					gameplay.Phase = 6;
					gameplay.StencilPelledOff();
				}
				else  //animacija prati prst
				{
					animStencilPaper.Play("PeelPaper",-1,animMagnitude);				 
					animStencilPaper.speed =0;
				}
			}
			else
			{
				animMagnitude = Mathf.Lerp(animMagnitude,0, Time.deltaTime*5); //vracanje na pocetnu poziciju
				 
				animStencilPaper.Play("PeelPaper",-1,animMagnitude);
				animStencilPaper.speed =0;
			}

			 

		}
	}














}
