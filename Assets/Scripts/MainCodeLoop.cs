using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class MainCodeLoop : MonoBehaviour
{
    public TextAsset textAsset;
	public GameObject[] bigSlots;
	public GameObject[] smallSlots;
	public GameObject[] scoreSlots;

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
	private int letterIndex;
	private string word;
	private int score;

	private void InitLetters()
	{
		int wordIndex = Random.Range(0, words.Length);
		int smallSlotIndex = 0;
		HashSet<int> bigSlotIndices = new HashSet<int>(Enumerable.Range(0, bigSlots.Length));
		int bigSlotIndex = bigSlotIndices.ToArray()[Random.Range(0, bigSlotIndices.Count)];
		foreach (var c in words[wordIndex])
		{
			if (c == '\r')
			{
				break;
			}
			
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
	}

    // Use this for initialization
    void Start()
    {
		score = 0;
		word = "";
		letterIndex = 0;
        words = textAsset.text.Split('\n');
		slotToLetter = new Dictionary<GameObject, char>();
        letterToMesh = new Dictionary<char, Mesh>();
        foreach (var entry in letterToMeshList)
        {
			letterToMesh[entry.letter[0]] = entry.mesh;
        }

		InitLetters();

		foreach (var letterObj in bigSlots)
		{
			Animator animator = letterObj.GetComponent<Animator>();
			animator.SetFloat("Speed", Random.Range(0f, 1f));
		}
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
			RaycastHit hit;
			
			if (Physics.Raycast(ray, out hit))
			{
				Animator animator = hit.collider.GetComponent<Animator>();
				if (animator.GetBool("Pressed") == false)
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

		if ((Input.touchCount == 0) && (letterIndex >= smallSlots.Length)) {
			foreach (var slot in bigSlots)
			{
				Animator animator = slot.GetComponent<Animator>();
				animator.SetBool("Pressed", false);
			}

			foreach (var slot in smallSlots)
			{
				slot.GetComponent<MeshFilter>().mesh = letterToMesh['_'];
			}

			if (System.Array.IndexOf(words, word + '\r') >= 0)
			{
				InitLetters();
				score++;
				// HACK! Need to make it scale to more than two digits.
				if (score < 100)
				{
					scoreSlots[0].GetComponent<MeshFilter>().mesh = letterToMesh[(char)('0' + score / 10)];
					scoreSlots[1].GetComponent<MeshFilter>().mesh = letterToMesh[(char)('0' + score % 10)];
				}
			}

			letterIndex = 0;
			word = "";
		}
    }
}
