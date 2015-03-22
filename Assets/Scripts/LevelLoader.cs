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
			Fader fader = GameObject.FindGameObjectWithTag("Finish").GetComponent<Fader>();
			if (choiceSlots[0] == gameObject)
			{
				fader.Stop(() => Application.LoadLevel(Utils.lvlMenu));
			}
			else if (choiceSlots[1] == gameObject)
			{
				PlayerPrefs.SetInt(levelToLoad + Utils.wordId, 0);
				PlayerPrefs.SetInt(levelToLoad + Utils.seedId, Random.Range(0, Utils.maxSeed));
				PlayerPrefs.SetInt(levelToLoad + Utils.scoreId, 0);
				PlayerPrefs.SetFloat(levelToLoad + Utils.timeId, 0f);
				fader.Stop(() => Application.LoadLevel(levelToLoad));
			}
			else if (choiceSlots[2] == gameObject)
			{
				fader.Stop(() => Application.LoadLevel(levelToLoad));
			}
			enabled = false;
		});
	}
}
