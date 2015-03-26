using UnityEngine;
using System.Collections;

public class GameMusic : MonoBehaviour
{
	public const string musicId = "music";
	public const string soundsId = "sounds";

	public static bool music;
	public static bool sounds;

	private static GameObject failAudio;
	private static GameObject successAudio;
	private static GameObject clickAudio;
	private static GameObject gameMusic;

	void Awake()
	{
		music = PlayerPrefs.GetInt(musicId, 1) == 1;
		sounds = PlayerPrefs.GetInt(soundsId, 1) == 1;
		
		failAudio = GameObject.Find("Fail");
		successAudio = GameObject.Find("Success");
		clickAudio = GameObject.Find("Click");
		gameMusic = GameObject.Find("GameMusic");

		ChangeMusic();
	}

	private static void PlayAudioSource(GameObject source)
	{
		if (source && sounds)
		{
			source.GetComponent<AudioSource>().Play();
		}
	}
	
	public static void PlayFail()
	{
		PlayAudioSource(failAudio);
	}
	
	public static void PlaySuccess()
	{
		PlayAudioSource(successAudio);
	}
	
	public static void PlayClick()
	{
		PlayAudioSource(clickAudio);
	}

	public static void ChangeMusic()
	{
		if (gameMusic)
		{
			gameMusic.GetComponent<AudioSource>().mute = !music;
		}
	}

	void Start()
	{
		GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
		foreach (var go in allObjects)
			if (go.activeInHierarchy)
				DontDestroyOnLoad(go);

		Application.LoadLevel(Utils.lvlMenu);
	}
}
