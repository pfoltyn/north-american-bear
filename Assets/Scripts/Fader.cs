using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Fader : MonoBehaviour {
	public delegate void OnEndDelegate();

	private float fadeSpeed;
	private bool sceneEnding;
	private bool sceneStarting;
	private Image image;
	private OnEndDelegate onEnd;

	void Start()
	{
		GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height * 2);
		fadeSpeed = 4.0f;
		sceneEnding = false;
		sceneStarting = true;
		image = GetComponent<Image>();
	}

	public void Stop(OnEndDelegate onEnd)
	{
		sceneStarting = false;
		sceneEnding = true;
		image.enabled = true;
		this.onEnd = onEnd;
	}

	void Update()
	{
		if (sceneStarting)
		{
			image.color = Color.Lerp(image.color, Color.clear, fadeSpeed * Time.deltaTime);
			if (image.color.a <= 0.01f)
			{
				image.color = Color.clear;
				image.enabled = false;
				sceneStarting = false;
			}
		}
		else if (sceneEnding)
		{
			image.color = Color.Lerp(image.color, Color.black, fadeSpeed * Time.deltaTime);
			if (image.color.a >= 0.99f)
			{
				image.color = Color.black;
				sceneEnding = false;
				onEnd();
			}
		}
	}
}
