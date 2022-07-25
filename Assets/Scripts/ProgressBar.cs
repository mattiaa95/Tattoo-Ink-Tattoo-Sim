using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProgressBar : MonoBehaviour {


	public Image BarBottom;
	public Image BarTop;

	RectTransform rtBarTop;

	public int BarTopMin = 23;
	public int BarTopMax = 384;
	 
	public float Value = 0; //0-1
	float startX;

	void Awake () {
		rtBarTop = BarTop.GetComponent<RectTransform>();
		startX = rtBarTop.sizeDelta.x;
		SetProgress (0);
	}

	/// <summary>
	/// Sets the progress.
	/// </summary>
	/// <param name="value">Value (0-1).</param>
	public void SetProgress(float value)
	{
		if(value>1) value = 1;
		if(value<0) value = 0;
		 Value = value;
		if(value > 0.009f)
		{

			if(!BarBottom.enabled)
			{
				BarBottom.enabled = true;
				BarTop.enabled = true;
			}
			rtBarTop.sizeDelta = new Vector2(startX, value*(BarTopMax-BarTopMin)+BarTopMin);
		}
		else
		{
			BarBottom.enabled = false;
			BarTop.enabled = false;
		}
	}
	 
	void Update1 () {
		Value += Time.deltaTime*.5f;
		if(Value >1) Value = -.1f;
		SetProgress(Value);
	}
}
