using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Gameover : MonoBehaviour
{
	public static float endScore;

	public GameObject[] scoreSlots;
	public GameObject[] recordSlots;
	public GameObject[] choiceSlots;
	public MeshFilter titleMeshFilter;

	private float highscore;

    void Start()
    {
		highscore = PlayerPrefs.GetFloat(LevelLoader.levelToLoad + "highScore", 3599.999f);
		titleMeshFilter.mesh = LevelLoader.titleMesh;
		Utils.RandomiseAnimationSpeed(choiceSlots);
		Utils.TimeToMesh(highscore, recordSlots);
		Utils.TimeToMesh(endScore, scoreSlots);
		if (endScore <= highscore)
		{
			GameObject.Find("Fireworks").GetComponent<Fireworks>().FireAtWill();
		}
    }
	
	void OnGUI()
	{
		Utils.DetectTouch((GameObject gameObject) => {
			string levelToLoad = "menu";
			if (choiceSlots[0] != gameObject)
			{
				levelToLoad = LevelLoader.levelToLoad;
			}
			Fader fader = GameObject.FindGameObjectWithTag("Finish").GetComponent<Fader>();
			fader.Stop(() => Application.LoadLevel(levelToLoad));
			enabled = false;
		});
	}
}
