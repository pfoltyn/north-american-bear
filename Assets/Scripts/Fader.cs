using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Fader : MonoBehaviour {
	public float fadeSpeed;
	public bool sceneEnding;
	public string nextSceneName;
	private bool sceneStarting;
	
	void Start()
	{
		GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height * 2);
		fadeSpeed = 4.0f;
		sceneEnding = false;
		nextSceneName = "";
		sceneStarting = true;
	}

	void Update()
	{
		if (sceneStarting)
		{
			StartScene();
		}
		else if (sceneEnding)
		{
			EndScene();
		}
	}
	
	private void FadeToClear()
	{
		GetComponent<Image>().color = Color.Lerp(GetComponent<Image>().color, Color.clear, fadeSpeed * Time.deltaTime);
	}

	private void FadeToBlack()
	{
		GetComponent<Image>().color = Color.Lerp(GetComponent<Image>().color, Color.black, fadeSpeed * Time.deltaTime);
	}

	private void StartScene()
	{
		FadeToClear();
		if(GetComponent<Image>().color.a <= 0.01f)
		{
			GetComponent<Image>().color = Color.clear;
			GetComponent<Image>().enabled = false;
			sceneStarting = false;
		}
	}

	private void EndScene()
	{
		GetComponent<Image>().enabled = true;
		FadeToBlack();
		if (GetComponent<Image>().color.a >= 0.99f)
		{
			Application.LoadLevel(nextSceneName);
		}
	}
}
