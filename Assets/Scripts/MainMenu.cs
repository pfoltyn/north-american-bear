using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
	public GameObject[] choiceSlots;
	public GameObject[] time0;
	public GameObject[] time1;
	public GameObject[] time2;

    void Start()
    {
		Utils.RandomiseAnimationSpeed(choiceSlots);
		Utils.TimeToMesh(PlayerPrefs.GetFloat(Utils.lvl4 + Utils.highScoreId, Utils.minScore), time0);
		Utils.TimeToMesh(PlayerPrefs.GetFloat(Utils.lvl5e + Utils.highScoreId, Utils.minScore), time1);
		Utils.TimeToMesh(PlayerPrefs.GetFloat(Utils.lvl5 + Utils.highScoreId, Utils.minScore), time2);
    }

	void OnGUI()
	{
		Utils.DetectTouch((GameObject gameObject) => {
			Animator animator = gameObject.GetComponent<Animator>();
			string levelToLoad = Utils.lvlSettings;
			if (choiceSlots[0] == gameObject)
			{
				animator.SetBool("Pressed", true);
				levelToLoad = Utils.lvl4;
			}
			else if (choiceSlots[1] == gameObject)
			{
				animator.SetBool("Pressed", true);
				levelToLoad = Utils.lvl5e;
			}
			else if (choiceSlots[2] == gameObject)
			{
				animator.SetBool("Pressed", true);
				levelToLoad = Utils.lvl5;
			}

			LevelLoader.levelToLoad = levelToLoad;
			LevelLoader.titleMesh = gameObject.GetComponent<MeshFilter>().mesh;
			if (PlayerPrefs.GetInt(levelToLoad + Utils.scoreId, 0) > 0)
			{
				levelToLoad = Utils.lvlLoader;
			}

			Fader fader = GameObject.FindGameObjectWithTag("Finish").GetComponent<Fader>();
			fader.Stop(() => Application.LoadLevel(levelToLoad));

			this.enabled = false;
		});
	}
}
