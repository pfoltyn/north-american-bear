using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
	public GameObject[] choiceSlots;

    void Start()
    {
		Utils.RandomiseAnimationSpeed(choiceSlots);
    }

	void OnGUI()
	{
		Utils.DetectTouch((GameObject gameObject) => {
			Animator animator = gameObject.GetComponent<Animator>();
			string levelToLoad = "settings";
			if (choiceSlots[0] == gameObject)
			{
				animator.SetBool("Pressed", true);
				levelToLoad = "4letters";
			}
			else if (choiceSlots[1] == gameObject)
			{
				animator.SetBool("Pressed", true);
				levelToLoad = "5letters_easy";
			}
			else if (choiceSlots[2] == gameObject)
			{
				animator.SetBool("Pressed", true);
				levelToLoad = "5letters";
			}

			LevelLoader.levelToLoad = levelToLoad;
			LevelLoader.titleMesh = gameObject.GetComponent<MeshFilter>().mesh;
			if (PlayerPrefs.GetInt(levelToLoad + "score", 0) > 0)
			{
				levelToLoad = "level_loader";
			}

			Fader fader = GameObject.FindGameObjectWithTag("Finish").GetComponent<Fader>();
			fader.Stop(() => Application.LoadLevel(levelToLoad));

			this.enabled = false;
		});
	}
}
