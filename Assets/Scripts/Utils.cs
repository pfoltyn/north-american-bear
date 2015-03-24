using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Utils
{
	public const int maxSeed = 65535;
	public const float minScore = 3599.999f;

	public const string highScoreId = "highScore";
	public const string scoreId = "score";
	public const string timeId = "time";
	public const string seedId = "seed";
	public const string wordId = "wordIndex";
	
	public const string adsTimeId = "adverts";
	public const string musicId = "music";
	public const string movBgId = "moving_background";
	public const string soundsId = "sounds";

	public const string lvlMenu = "menu";
	public const string lvl4 = "4letters";
	public const string lvl5e = "5letters_easy";
	public const string lvl5 = "5letters";
	public const string lvlSettings = "settings";
	public const string lvlLoader = "level_loader";
	public const string lvlGameover = "gameover";

	public delegate void OnTouch(GameObject gameObject);
	private static bool once = true;

	public static bool music = PlayerPrefs.GetInt(musicId, 1) == 1;
	public static bool movingBackground = PlayerPrefs.GetInt(movBgId, 1) == 1;
	public static bool sounds = PlayerPrefs.GetInt(soundsId, 1) == 1;

	public static Dictionary<char, Mesh> letterToMesh = new Dictionary<char, Mesh>(){
		{'0', Resources.Load("0", typeof(Mesh)) as Mesh},
		{'1', Resources.Load("1", typeof(Mesh)) as Mesh},
		{'2', Resources.Load("2", typeof(Mesh)) as Mesh},
		{'3', Resources.Load("3", typeof(Mesh)) as Mesh},
		{'4', Resources.Load("4", typeof(Mesh)) as Mesh},
		{'5', Resources.Load("5", typeof(Mesh)) as Mesh},
		{'6', Resources.Load("6", typeof(Mesh)) as Mesh},
		{'7', Resources.Load("7", typeof(Mesh)) as Mesh},
		{'8', Resources.Load("8", typeof(Mesh)) as Mesh},
		{'9', Resources.Load("9", typeof(Mesh)) as Mesh},
		{'_', Resources.Load("_", typeof(Mesh)) as Mesh},
		{'a', Resources.Load("a", typeof(Mesh)) as Mesh},
		{'ą', Resources.Load("a_", typeof(Mesh)) as Mesh},
		{'b', Resources.Load("b", typeof(Mesh)) as Mesh},
		{'c', Resources.Load("c", typeof(Mesh)) as Mesh},
		{'ć', Resources.Load("c_", typeof(Mesh)) as Mesh},
		{'d', Resources.Load("d", typeof(Mesh)) as Mesh},
		{'e', Resources.Load("e", typeof(Mesh)) as Mesh},
		{'ę', Resources.Load("e__", typeof(Mesh)) as Mesh},
		{'f', Resources.Load("f", typeof(Mesh)) as Mesh},
		{'g', Resources.Load("g", typeof(Mesh)) as Mesh},
		{'h', Resources.Load("h", typeof(Mesh)) as Mesh},
		{'i', Resources.Load("i", typeof(Mesh)) as Mesh},
		{'j', Resources.Load("j", typeof(Mesh)) as Mesh},
		{'k', Resources.Load("k", typeof(Mesh)) as Mesh},
		{'l', Resources.Load("l", typeof(Mesh)) as Mesh},
		{'ł', Resources.Load("l_", typeof(Mesh)) as Mesh},
		{'m', Resources.Load("m", typeof(Mesh)) as Mesh},
		{'n', Resources.Load("n", typeof(Mesh)) as Mesh},
		{'ń', Resources.Load("n_", typeof(Mesh)) as Mesh},
		{'o', Resources.Load("o", typeof(Mesh)) as Mesh},
		{'ó', Resources.Load("o_", typeof(Mesh)) as Mesh},
		{'p', Resources.Load("p", typeof(Mesh)) as Mesh},
		{'r', Resources.Load("r", typeof(Mesh)) as Mesh},
		{'s', Resources.Load("s", typeof(Mesh)) as Mesh},
		{'ś', Resources.Load("s_", typeof(Mesh)) as Mesh},
		{'t', Resources.Load("t", typeof(Mesh)) as Mesh},
		{'u', Resources.Load("u", typeof(Mesh)) as Mesh},
		{'w', Resources.Load("w", typeof(Mesh)) as Mesh},
		{'v', Resources.Load("v", typeof(Mesh)) as Mesh},
		{'q', Resources.Load("q", typeof(Mesh)) as Mesh},
		{'x', Resources.Load("x", typeof(Mesh)) as Mesh},
		{'y', Resources.Load("y", typeof(Mesh)) as Mesh},
		{'z', Resources.Load("z", typeof(Mesh)) as Mesh},
		{'ż', Resources.Load("z_", typeof(Mesh)) as Mesh},
		{'ź', Resources.Load("z__", typeof(Mesh)) as Mesh},
		{':', Resources.Load("collon", typeof(Mesh)) as Mesh},
		{'/', Resources.Load("slash", typeof(Mesh)) as Mesh},
	};

	private static GameObject failAudio = GameObject.Find("Fail");
	private static GameObject successAudio = GameObject.Find("Success");
	private static GameObject clickAudio = GameObject.Find("Click");

	public static void TimeToMesh(float number, GameObject[] mesh)
	{
		mesh[0].GetComponent<MeshFilter>().mesh =
			letterToMesh[(char)('0' + (int)(number / 60) / 10)];
		mesh[1].GetComponent<MeshFilter>().mesh =
			letterToMesh[(char)('0' + (int)(number / 60) % 10)];
		mesh[2].GetComponent<MeshFilter>().mesh =
			letterToMesh[(char)('0' + (int)(number % 60) / 10)];
		mesh[3].GetComponent<MeshFilter>().mesh =
			letterToMesh[(char)('0' + (int)(number % 60) % 10)];
		mesh[4].GetComponent<MeshFilter>().mesh =
			letterToMesh[(char)('0' + (int)((number - (int)number) * 100) / 10)];
		mesh[5].GetComponent<MeshFilter>().mesh =
			letterToMesh[(char)('0' + (int)((number - (int)number) * 100) % 10)];
	}

	public static void DetectTouch(OnTouch onTouchDelegate)
	{
		if (once && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
			RaycastHit hit;
			
			if (Physics.Raycast(ray, out hit))
			{
				PlayClick();
				onTouchDelegate(hit.collider.gameObject);
			}
		}
		once = Input.touchCount == 0;
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

	public static void RandomiseAnimationSpeed(GameObject[] gameObjects)
	{
		foreach (var gameObject in gameObjects)
		{
			Animator animator = gameObject.GetComponent<Animator>();
			animator.SetFloat("Speed", Random.Range(0f, 1f));
		}
	}

	public static void NewGame(float time, string lvlName)
	{
		if (PlayerPrefs.GetFloat(lvlName + highScoreId, minScore) > time)
		{
			PlayerPrefs.SetFloat(lvlName + highScoreId, time);
		}
		PlayerPrefs.SetInt(lvlName + wordId, 0);
		PlayerPrefs.SetInt(lvlName + scoreId, 0);
		PlayerPrefs.SetFloat(lvlName + timeId, 0f);
		PlayerPrefs.SetInt(seedId, PlayerPrefs.GetInt(seedId, 0) + 1);
		PlayerPrefs.Save();
	}

	public static void ShowAdds(string levelToLoad)
	{
		float adsTime = PlayerPrefs.GetFloat(adsTimeId, 0f);
		if (adsTime > Time.time)
		{
			adsTime = Time.time;
			PlayerPrefs.SetFloat(adsTimeId, adsTime);
		}
		if ((levelToLoad != lvlSettings) &&
		    (levelToLoad != lvlMenu) &&
		    (Mathf.Abs(Time.time - adsTime) > 60f))
		{
			PlayerPrefs.SetFloat(adsTimeId, Time.time);
			UnityAdsHelper.ShowAd();
		}
		PlayerPrefs.Save();
	}
}
