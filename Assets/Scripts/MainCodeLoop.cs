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
	private string lvlName;
	private bool pause;

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

	private void Reshuffle()
	{
		Random.seed = seed;
		for (int t = 0; t < words.Length; t++)
		{
			string tmp = words[t];
			int r = Random.Range(t, words.Length);
			words[t] = words[r];
			words[r] = tmp;
		}
	}

	void Awake()
	{
		Fader.FadeIn();
	}

    void Start()
    {
		pause = true;
		lvlName = LevelLoader.levelToLoad;
		word = "";
		letterIndex = 0;

		seed = PlayerPrefs.GetInt(Utils.seedId, 0);
		wordIndex = PlayerPrefs.GetInt(lvlName + Utils.wordId, 0);
		score = PlayerPrefs.GetInt(lvlName + Utils.scoreId, 0);
		timeOffset = PlayerPrefs.GetFloat(lvlName + Utils.timeId, 0f);

		allWords = textAssetFull.text.Split('\n');
		allWords = allWords.Select(x => x.Trim()).ToArray();
        words = textAsset.text.Split('\n');
		words = words.Select(x => x.Trim()).ToArray();
		words = words.Where(x => x.Length > 0).ToArray();
		Reshuffle();

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
			Fader.FadeOut(() => Application.LoadLevel(Utils.lvlMenu));
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
					Animator animator = slot.GetComponent<Animator>();
					if (letter == slotToLetter[slot] && animator.GetBool("Pressed") == true)
					{
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
			pause = false;
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

	private void GameHasEnded(float time)
	{
		Gameover.endScore = time;
		Utils.NewGame(time, lvlName);
		Fader.FadeOut(() => Application.LoadLevel(Utils.lvlGameover));
		enabled = false;
	}

    void Update()
    {
		if (!pause)
		{
			theTime = timeOffset + Time.timeSinceLevelLoad;
		}
		else
		{
			timeOffset = -Time.timeSinceLevelLoad;;
		}
		PlayerPrefs.SetFloat(lvlName + Utils.timeId, theTime);
		Utils.TimeToMesh(theTime, timeSlots);

		if (theTime >= Utils.minScore)
		{
			GameHasEnded(Utils.minScore);
		}

		if (letterIndex >= smallSlots.Length)
		{
			if (System.Array.IndexOf(allWords, word) >= 0)
			{
				GameMusic.PlaySuccess();
				wordIndex = (wordIndex + 1) % scoreLimit;
				score = (score + 1) % scoreLimit;

				if (wordIndex == 0)
				{
					GameHasEnded(theTime);
				}

				InitLetters();

				PlayerPrefs.SetInt(Utils.seedId, seed + 1);
				PlayerPrefs.SetInt(lvlName + Utils.wordId, wordIndex);
				PlayerPrefs.SetInt(lvlName + Utils.scoreId, score);
			}
			else
			{
				GameMusic.PlayFail();
			}

			ResetPlayersGuess();
		}
    }

	void OnDestroy()
	{
		if (score == 0)
		{
			PlayerPrefs.SetFloat(lvlName + Utils.timeId, 0f);
		}
		PlayerPrefs.Save();
	}
}
