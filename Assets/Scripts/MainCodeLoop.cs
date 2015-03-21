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
	public int scoreLimit;

	private Dictionary<GameObject, char> slotToLetter;
    private string[] words;
	private string[] allWords;
	private string word;
	private int letterIndex;
	private float timeOffset;
	private Fader fader;
	private string lvlName;

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
			if (Utils.letterToMesh.ContainsKey(c))
			{
				MeshFilter meshFilter = bigSlots[bigSlotIndex].GetComponent<MeshFilter>();
				meshFilter.mesh = Utils.letterToMesh[c];
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

		scoreSlots[0].GetComponent<MeshFilter>().mesh = Utils.letterToMesh[(char)('0' + score / 10)];
		scoreSlots[1].GetComponent<MeshFilter>().mesh = Utils.letterToMesh[(char)('0' + score % 10)];
		scoreSlots[2].GetComponent<MeshFilter>().mesh = Utils.letterToMesh[(char)('0' + scoreLimit / 10)];
		scoreSlots[3].GetComponent<MeshFilter>().mesh = Utils.letterToMesh[(char)('0' + scoreLimit % 10)];
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

    void Start()
    {
		lvlName = Application.loadedLevelName;
		word = "";
		letterIndex = 0;

		wordIndex = PlayerPrefs.GetInt(lvlName + "wordIndex", 0);
		seed = PlayerPrefs.GetInt(lvlName + "seed", 42);
		score = PlayerPrefs.GetInt(lvlName + "score", 0);
		timeOffset = PlayerPrefs.GetFloat(lvlName + "time", 0f);
		fader = GameObject.FindGameObjectWithTag("Finish").GetComponent<Fader>();

		allWords = textAssetFull.text.Split('\n');
		allWords = allWords.Select(x => x.Trim()).ToArray();
        words = textAsset.text.Split('\n');
		words = words.Select(x => x.Trim()).ToArray();
		Reshuffle(words);

		slotToLetter = new Dictionary<GameObject, char>();

		foreach (var slot in bigSlots)
		{
			MeshFilter meshFilter = slot.GetComponent<MeshFilter>();
			char letter = (char)Random.Range('a', 'z');
			meshFilter.mesh = Utils.letterToMesh[letter];
			slotToLetter[slot] = letter;
		}
		
		Utils.RandomiseAnimationSpeed(bigSlots);
		Utils.RandomiseAnimationSpeed(menuItems);
		InitLetters();
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
			slot.GetComponent<MeshFilter>().mesh = Utils.letterToMesh['_'];
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
			fader.Stop(() => Application.LoadLevel("menu"));
			enabled = false;
			break;
		case 1:
			if (letterIndex > 0)
			{
				letterIndex--;
				meshFilter = smallSlots[letterIndex].GetComponent<MeshFilter>();
				meshFilter.mesh = Utils.letterToMesh['_'];
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
			meshFilter.mesh = Utils.letterToMesh[letter];
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
		Utils.DetectTouch((GameObject gameObject) => {
			Animator animator = gameObject.GetComponent<Animator>();
		    if (menuItems.Contains(gameObject))
			{
				HandleMenuClick(gameObject);
			}
			else if (animator.GetBool("Pressed") == false)
			{
				animator.SetBool("Pressed", true);
				char letter = slotToLetter[gameObject];
				MeshFilter meshFilter = smallSlots[letterIndex].GetComponent<MeshFilter>();
				meshFilter.mesh = Utils.letterToMesh[letter];
				letterIndex++;
				word += letter;
			}
		});
	}

    void Update()
    {
		theTime = timeOffset + Time.timeSinceLevelLoad;
		Utils.TimeToMesh(theTime, timeSlots);

		if (letterIndex >= smallSlots.Length)
		{
			if (System.Array.IndexOf(allWords, word) >= 0)
			{
				Utils.PlaySuccess();
				wordIndex = (wordIndex + 1) % words.Length;
				score++;
				InitLetters();
			}
			else
			{
				Utils.PlayFail();
			}

			ResetPlayersGuess();

			if (wordIndex >= scoreLimit)
			{
				Gameover.endScore = theTime;
				if (PlayerPrefs.GetFloat(lvlName + "highScore", 3599.999f) > theTime)
				{
					PlayerPrefs.SetFloat(lvlName + "highScore", theTime);
				}
				OnDestroy();
				fader.Stop(() => Application.LoadLevel("gameover"));
				enabled = false;
			}
		}
    }

	void OnDestroy()
	{
		PlayerPrefs.SetInt(lvlName + "wordIndex", wordIndex);
		PlayerPrefs.SetInt(lvlName + "seed", seed);
		PlayerPrefs.SetInt(lvlName + "score", score);
		PlayerPrefs.SetFloat(lvlName + "time", theTime);
		PlayerPrefs.Save();
	}
}
