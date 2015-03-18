using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class MainCodeLoop : MonoBehaviour
{
    public TextAsset textAsset;
	public TextAsset textAssetFull;
	public GameObject[] timeSlots;
	public GameObject[] bigSlots;
	public GameObject[] smallSlots;
	public GameObject[] scoreSlots;
	public GameObject[] menuItems;
	public AudioSource failAudio;
	public AudioSource successAudio;
	public AudioSource clickAudio;
	public int scoreLimit;

    [System.Serializable]
    public class LetterToMeshEntry
    {
        public string letter;
        public Mesh mesh;
    }
    public LetterToMeshEntry[] letterToMeshList;

    private Dictionary<char, Mesh> letterToMesh;
	private Dictionary<GameObject, char> slotToLetter;
    private string[] words;
	private string[] allWords;
	private string word;
	private int letterIndex;
	private bool once;
	private bool sounds;
	private float timeOffset;

	private int wordIndex;
	private int seed;
	private int score;
	private float theTime;

	private void InitLetters()
	{
		int smallSlotIndex = 0;
		HashSet<int> bigSlotIndices = new HashSet<int>(Enumerable.Range(0, bigSlots.Length));
		int bigSlotIndex = bigSlotIndices.ToArray()[Random.Range(0, bigSlotIndices.Count)];
		foreach (var c in words[wordIndex])
		{
			if (letterToMesh.ContainsKey(c))
			{
				MeshFilter meshFilter = bigSlots[bigSlotIndex].GetComponent<MeshFilter>();
				meshFilter.mesh = letterToMesh[c];
				slotToLetter[bigSlots[bigSlotIndex]] = c;
			}
			else
			{
				Debug.Log(string.Format("Character {0} not found in dictionary.", c));
			}
			
			smallSlotIndex++;
			bigSlotIndices.Remove(bigSlotIndex);
			if (bigSlotIndices.Count > 0)
			{
				bigSlotIndex = bigSlotIndices.ToArray()[Random.Range(0, bigSlotIndices.Count)];
			}
		}

		scoreSlots[0].GetComponent<MeshFilter>().mesh = letterToMesh[(char)('0' + score / 10)];
		scoreSlots[1].GetComponent<MeshFilter>().mesh = letterToMesh[(char)('0' + score % 10)];
		scoreSlots[2].GetComponent<MeshFilter>().mesh = letterToMesh[(char)('0' + scoreLimit / 10)];
		scoreSlots[3].GetComponent<MeshFilter>().mesh = letterToMesh[(char)('0' + scoreLimit % 10)];
	}

	private void Reshuffle(string[] texts)
	{
		Random.seed = seed;
		for (int t = 0; t < texts.Length; t++ )
		{
			string tmp = texts[t];
			int r = Random.Range(t, texts.Length);
			texts[t] = texts[r];
			texts[r] = tmp;
		}
	}

    // Use this for initialization
    void Start()
    {
		word = "";
		letterIndex = 0;

		string name = Application.loadedLevelName;
		wordIndex = PlayerPrefs.GetInt(name + "wordIndex", 0);
		seed = PlayerPrefs.GetInt(name + "seed", 42);
		score = PlayerPrefs.GetInt(name + "score", 0);
		timeOffset = PlayerPrefs.GetFloat(name + "time", 0f);

		allWords = textAssetFull.text.Split('\n');
		allWords = allWords.Select(x => x.Trim()).ToArray();
        words = textAsset.text.Split('\n');
		words = words.Select(x => x.Trim()).ToArray();
		Reshuffle(words);

		slotToLetter = new Dictionary<GameObject, char>();
        letterToMesh = new Dictionary<char, Mesh>();
		foreach (var item in letterToMeshList)
		{
			letterToMesh[item.letter[0]] = item.mesh;
		}

		foreach (var slot in bigSlots)
		{
			Animator animator = slot.GetComponent<Animator>();
			animator.SetFloat("Speed", Random.Range(0f, 1f));

			MeshFilter meshFilter = slot.GetComponent<MeshFilter>();
			char letter = (char)Random.Range('a', 'z');
			meshFilter.mesh = letterToMesh[letter];
			slotToLetter[slot] = letter;
		}

		foreach (var item in menuItems) {
			Animator animator = item.GetComponent<Animator>();
			animator.SetFloat("Speed", Random.Range(0f, 1f));
		}

		InitLetters();

		sounds = PlayerPrefs.GetInt("sounds", 1) == 1;
    }

	private void ResetPlayersGuess()
	{
		foreach (var slot in bigSlots)
		{
			Animator animator = slot.GetComponent<Animator>();
			animator.SetBool("Pressed", false);
		}
		
		foreach (var slot in smallSlots)
		{
			slot.GetComponent<MeshFilter>().mesh = letterToMesh['_'];
		}
		
		letterIndex = 0;
		word = "";
	}

	private void HandleMenuClick(GameObject hitGo)
	{
		MeshFilter meshFilter;
		char letter;
		int idx = System.Array.IndexOf(menuItems, hitGo);
		switch (idx) {
		case 0:
			GameObject go = GameObject.FindGameObjectWithTag("Finish");
			go.GetComponent<Fader>().Stop(() => Application.LoadLevel("menu"));
			break;
		case 1:
			if (letterIndex > 0)
			{
				letterIndex--;
				meshFilter = smallSlots[letterIndex].GetComponent<MeshFilter>();
				meshFilter.mesh = letterToMesh['_'];
				letter = word[word.Length - 1];
				word = word.Remove(word.Length - 1);
				foreach (var slot in bigSlots)
				{
					if (letter == slotToLetter[slot])
					{
						Animator animator = slot.GetComponent<Animator>();
						animator.SetBool("Pressed", false);
						break;
					}
				}
			}
			break;
		case 2:
			ResetPlayersGuess();
			letter = words[wordIndex][0];
			meshFilter = smallSlots[0].GetComponent<MeshFilter>();
			meshFilter.mesh = letterToMesh[letter];
			letterIndex++;
			word += letter;
			foreach (var slot in bigSlots)
			{
				if (letter == slotToLetter[slot])
				{
					Animator animator = slot.GetComponent<Animator>();
					animator.SetBool("Pressed", true);
					break;
				}
			}
			break;
		default:
			break;
		}
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
				if (menuItems.Contains(hit.collider.gameObject))
				{
					HandleMenuClick(hit.collider.gameObject);
				}
				else if (animator.GetBool("Pressed") == false)
				{
					animator.SetBool("Pressed", true);
					char letter = slotToLetter[hit.collider.gameObject];
					MeshFilter meshFilter = smallSlots[letterIndex].GetComponent<MeshFilter>();
					meshFilter.mesh = letterToMesh[letter];
					letterIndex++;
					word += letter;
				}
			}
		}
	}

    // Update is called once per frame
    void Update()
    {
		theTime = timeOffset + Time.timeSinceLevelLoad;
		timeSlots[0].GetComponent<MeshFilter>().mesh =
			letterToMesh[(char)('0' + (int)(theTime / 60) / 10)];
		timeSlots[1].GetComponent<MeshFilter>().mesh =
			letterToMesh[(char)('0' + (int)(theTime / 60) % 10)];
		timeSlots[2].GetComponent<MeshFilter>().mesh =
			letterToMesh[(char)('0' + (int)(theTime % 60) / 10)];
		timeSlots[3].GetComponent<MeshFilter>().mesh =
			letterToMesh[(char)('0' + (int)(theTime % 60) % 10)];
		timeSlots[4].GetComponent<MeshFilter>().mesh =
			letterToMesh[(char)('0' + (int)((theTime - (int)theTime) * 100) / 10)];
		timeSlots[5].GetComponent<MeshFilter>().mesh =
			letterToMesh[(char)('0' + (int)((theTime - (int)theTime) * 100) % 10)];

		if (Input.touchCount == 0)
		{
			once = true;
		}

		if (letterIndex >= smallSlots.Length)
		{
			if (System.Array.IndexOf(allWords, word) >= 0)
			{
				if (sounds)
				{
					successAudio.Play();
				}
				wordIndex = (wordIndex + 1) % words.Length;
				score++;
				InitLetters();
			}
			else if (sounds)
			{
				failAudio.Play();
			}

			ResetPlayersGuess();
		}
    }

	void OnDestroy()
	{
		string name = Application.loadedLevelName;
		PlayerPrefs.SetInt(name + "wordIndex", wordIndex);
		PlayerPrefs.SetInt(name + "seed", seed);
		PlayerPrefs.SetInt(name + "score", score);
		PlayerPrefs.SetFloat(name + "time", (score > 0) ? theTime : 0f);
		if (PlayerPrefs.GetInt(name + "highScore", 0) < score)
		{
			PlayerPrefs.SetInt(name + "highScore", score);
		}
		PlayerPrefs.Save();
	}
}
