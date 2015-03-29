using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class LevelLoader : MonoBehaviour
{
	public static Mesh titleMesh;
	public static string levelToLoad;

	public GameObject[] scoreSlots;
	public GameObject[] choiceSlots;
	public MeshFilter titleMeshFilter;

	private float highscore;

	void Awake()
	{
		Lang.Translate();
		Fader.FadeIn();
	}

    void Start()
    {
		highscore = PlayerPrefs.GetFloat(levelToLoad + Utils.highScoreId, Utils.minScore);
		titleMeshFilter.mesh = titleMesh;
		Utils.RandomiseAnimationSpeed(choiceSlots);
		Utils.TimeToMesh(highscore, scoreSlots);
    }

	void OnGUI()
	{
		Utils.DetectTouch((GameObject gameObject) => {
			if (choiceSlots[0] == gameObject)
			{
				Fader.FadeOut(() => Application.LoadLevel(Utils.lvlMenu));
			}
			else if (choiceSlots[1] == gameObject)
			{
				Utils.NewGame(Utils.minScore, levelToLoad);
				Fader.FadeOut(() => Application.LoadLevel(levelToLoad));
			}
			else if (choiceSlots[2] == gameObject)
			{
				Fader.FadeOut(() => Application.LoadLevel(levelToLoad));
			}
			enabled = false;
		});
	}
}
