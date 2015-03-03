using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class MainCodeLoop : MonoBehaviour
{
    public TextAsset textAsset;
	public GameObject[] bigLetters;
	public GameObject[] smallLetters;

    [System.Serializable]
    public class LetterToMeshEntry
    {
        public string letter;
        public Mesh mesh;
    }
    public LetterToMeshEntry[] letterToMeshList;

    private Dictionary<char, Mesh> letterToMesh;
    private string[] words;

	private void InitLetters()
	{
		int wordIndex = Random.Range(0, words.Length);
		int smallLetterIndex = 0;
		HashSet<int> bigLetterIndices = new HashSet<int>(Enumerable.Range(0, bigLetters.Length));
		int bigLetterIndex = bigLetterIndices.ToArray()[Random.Range(0, bigLetterIndices.Count)];
		foreach (var c in words[wordIndex])
		{
			if (c == '\r')
			{
				break;
			}
			
			if (letterToMesh.ContainsKey(c))
			{
				MeshFilter meshFilter = bigLetters[bigLetterIndex].GetComponent<MeshFilter>();
				meshFilter.mesh = letterToMesh[c];
				
				meshFilter = smallLetters[smallLetterIndex].GetComponent<MeshFilter>();
				meshFilter.mesh = letterToMesh[c];
			}
			else
			{
				Debug.Log(string.Format("Character {0} not found in dictionary.", c));
			}
			
			smallLetterIndex++;
			bigLetterIndices.Remove(bigLetterIndex);
			if (bigLetterIndices.Count > 0)
			{
				bigLetterIndex = bigLetterIndices.ToArray()[Random.Range(0, bigLetterIndices.Count)];
			}
		}
	}

    // Use this for initialization
    void Start()
    {
        words = textAsset.text.Split('\n');
        letterToMesh = new Dictionary<char, Mesh>();
        foreach (var entry in letterToMeshList)
        {
			letterToMesh[entry.letter[0]] = entry.mesh;
        }

		InitLetters();

		foreach (var letterObj in bigLetters)
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
				hit.collider.GetComponent<Animator>().SetBool("Pressed", true);
			}
		}

		if (Input.touchCount == 0) {
			foreach (var letterObj in bigLetters)
			{
				Animator animator = letterObj.GetComponent<Animator>();
				animator.SetBool("Pressed", false);
			}
		}
    }
}
