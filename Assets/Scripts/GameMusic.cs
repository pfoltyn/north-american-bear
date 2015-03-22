using UnityEngine;
using System.Collections;

public class GameMusic : MonoBehaviour {
	public AudioSource[] audioSources;

	void Start () {
		DontDestroyOnLoad(gameObject);
		foreach (var audioSource in audioSources) {
			DontDestroyOnLoad(audioSource.gameObject);
		}

		GetComponent<AudioSource>().mute = !Utils.music;
		Application.LoadLevel(Utils.lvlMenu);
	}
}
