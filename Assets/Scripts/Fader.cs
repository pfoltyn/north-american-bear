using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Fader : MonoBehaviour
{
	public delegate void OnEndDelegate();

	private float fadeSpeed;
	private bool sceneEnding;
	private bool sceneStarting;
	private Image image;
	private OnEndDelegate onEnd;

	private static GameObject fader;

	void Awake()
	{
		fader = GameObject.Find("Fader");
	}

	public static void FadeIn()
	{
		if (fader)
		{
			fader.GetComponent<Fader>().FadeInInternal();
		}
	}
	
	public static void FadeOut(OnEndDelegate onEnd)
	{
		if (fader)
		{
			fader.GetComponent<Fader>().FadeOutInternal(onEnd);
		}
	}

	void Start()
	{
		fadeSpeed = 4.0f;
		sceneEnding = false;
		sceneStarting = false;
		image = GetComponent<Image>();
		onEnd = null;
	}

	private void FadeInInternal()
	{
		sceneEnding = false;
		sceneStarting = true;
		image.enabled = true;
	}

	private void FadeOutInternal(OnEndDelegate onEnd)
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
