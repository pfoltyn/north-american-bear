using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
	public GameObject[] choiceSlots;
	public AudioSource clickAudio;

	private Fader fader;
	private bool once;
	private bool sounds;

    // Use this for initialization
    void Start()
    {
		once = true;
		fader = GameObject.FindGameObjectWithTag("Finish").GetComponent<Fader>();
		foreach (var slot in choiceSlots)
		{
			Animator animator = slot.GetComponent<Animator>();
			animator.SetFloat("Speed", Random.Range(0f, 1f));
		}
		sounds = PlayerPrefs.GetInt("sounds", 1) == 1;
    }

	void OnGUI()
	{
		if (once && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
			RaycastHit hit;
			
			if (Physics.Raycast(ray, out hit))
			{
				once = false;
				if (sounds)
				{
					clickAudio.Play();
				}
				Animator animator = hit.collider.GetComponent<Animator>();
				string levelToLoad = "settings";
				if (choiceSlots[0] == animator.gameObject)
				{
					animator.SetBool("Pressed", true);
					levelToLoad = "4letters";
				}
				else if (choiceSlots[1] == animator.gameObject)
				{
					animator.SetBool("Pressed", true);
					levelToLoad = "5letters_easy";
				}
				else if (choiceSlots[2] == animator.gameObject)
				{
					animator.SetBool("Pressed", true);
					levelToLoad = "5letters";
				}

				LevelLoader.levelToLoad = levelToLoad;
				LevelLoader.titleMesh = hit.collider.GetComponent<MeshFilter>().mesh;
				if (PlayerPrefs.GetInt(levelToLoad + "score", 0) == 0)
				{
					fader.Stop(() => Application.LoadLevel(levelToLoad));
				}
				else
				{
					fader.Stop(() => Application.LoadLevel("level_loader"));
				}
			}
		}
	}
}
