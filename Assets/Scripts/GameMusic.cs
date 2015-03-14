using UnityEngine;
using System.Collections;

public class GameMusic : MonoBehaviour {
	void Start () {
		GameObject gameMusic = GameObject.Find("GameMusic");
		DontDestroyOnLoad(gameMusic);
		gameMusic.GetComponent<AudioSource>().mute = PlayerPrefs.GetInt("music", 1) == 0;
		Application.LoadLevel("menu");
	}
}
