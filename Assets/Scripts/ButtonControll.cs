using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonControll : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler    
{
	public bool changeInteractable = true;
	bool bPointerIn = false;
	bool bPointerUp= true;
	Animator anim;
	Button btn;
	void Start () {
		anim = transform.GetComponent<Animator>();
		btn = transform.GetComponent<Button>();
	}


	public void OnPointerDown( PointerEventData eventData)
	{
		if(!changeInteractable && !btn.interactable ) return;
		if(bPointerUp )
		{
			btn.interactable = true;
			bPointerIn = true;
			bPointerUp = false;
			anim.SetBool("bPointerIn",bPointerIn );
		}
	}

	public void OnPointerUp( PointerEventData eventData)
	{
		bPointerUp = true;
		bPointerIn = false;
		anim.SetBool("bPointerIn",bPointerIn );
	}

	public void OnPointerExit( PointerEventData eventData)
	{
		bPointerIn = false;
		anim.SetBool("bPointerIn",bPointerIn );
		if(changeInteractable)
		{
			btn.interactable = false;
			anim.SetTrigger("Highlighted" );
	   }
	}

}
