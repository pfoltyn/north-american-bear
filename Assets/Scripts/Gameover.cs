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

	void Awake()
	{
		Fader.FadeIn();
	}

    void Start()
    {
		highscore = PlayerPrefs.GetFloat(LevelLoader.levelToLoad + Utils.highScoreId, Utils.minScore);
		titleMeshFilter.mesh = LevelLoader.titleMesh;
		Utils.RandomiseAnimationSpeed(choiceSlots);
		Utils.TimeToMesh(highscore, recordSlots);
		Utils.TimeToMesh(endScore, scoreSlots);
		if (endScore <= highscore)
		{
			Fireworks.FireAtWill();
		}
    }
	
	void OnGUI()
	{
		Utils.DetectTouch((GameObject gameObject) => {
			string levelToLoad = Utils.lvlMenu;
			if (choiceSlots[0] != gameObject)
			{
				levelToLoad = LevelLoader.levelToLoad;
			}
			Fader.FadeOut(() => {
				Fireworks.HoldYourFire();
				UnityAdsHelper.ShowAdds(levelToLoad);
				Application.LoadLevel(levelToLoad);
			});
			enabled = false;
		});
	}
}
