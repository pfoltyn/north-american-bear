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
	public AudioSource clickAudio;
	public MeshFilter titleMeshFilter;

	[System.Serializable]
	public class LetterToMeshEntry
	{
		public string letter;
		public Mesh mesh;
	}
	public LetterToMeshEntry[] letterToMeshList;
	
	private Dictionary<char, Mesh> letterToMesh;
	private Fader fader;
	private bool once;
	private bool sounds;
	private int highscore;

    // Use this for initialization
    void Start()
    {
		once = true;
		highscore = PlayerPrefs.GetInt(levelToLoad + "highScore", 0);
		fader = GameObject.FindGameObjectWithTag("Finish").GetComponent<Fader>();
		foreach (var slot in choiceSlots)
		{
			Animator animator = slot.GetComponent<Animator>();
			animator.SetFloat("Speed", Random.Range(0f, 1f));
		}

		letterToMesh = new Dictionary<char, Mesh>();
		foreach (var item in letterToMeshList)
		{
			letterToMesh[item.letter[0]] = item.mesh;
		}

		titleMeshFilter.mesh = titleMesh;

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
				if (choiceSlots[0] == animator.gameObject)
				{
					fader.Stop(() => Application.LoadLevel("menu"));
				}
				else if (choiceSlots[1] == animator.gameObject)
				{
					PlayerPrefs.SetInt(levelToLoad + "wordIndex", 0);
					PlayerPrefs.SetInt(levelToLoad + "seed", Random.Range(0, 255));
					PlayerPrefs.SetInt(levelToLoad + "score", 0);
					fader.Stop(() => Application.LoadLevel(levelToLoad));
				}
				else if (choiceSlots[2] == animator.gameObject)
				{
					fader.Stop(() => Application.LoadLevel(levelToLoad));
				}
			}
		}
	}
	
    void Update()
    {
		if (highscore > 0)
		{
			scoreSlots[0].GetComponent<MeshFilter>().mesh = letterToMesh[(char)('0' + highscore / 100)];
			scoreSlots[1].GetComponent<MeshFilter>().mesh = letterToMesh[(char)('0' + (highscore % 100) / 10)];
			scoreSlots[2].GetComponent<MeshFilter>().mesh = letterToMesh[(char)('0' + (highscore % 100) % 10)];
		}

		once = Input.touchCount == 0;
    }
}
