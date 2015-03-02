using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainCodeLoop : MonoBehaviour {
	public TextAsset textAsset;
	public GameObject[] bigLetters;
	public GameObject[] smallLetters;

	[System.Serializable]
	public class Letter2MeshEntry
	{
		public string letter;
		public GameObject mesh;
	}
	public Letter2MeshEntry[] letterToMeshList;
	
	private Dictionary<string, GameObject> letter2Mesh;
	private string[] words;

	// Use this for initialization
	void Start () {
		words = textAsset.text.Split('\n');
		letter2Mesh = new Dictionary<string, GameObject>();
		foreach (var entry in letterToMeshList) {
			letter2Mesh[entry.letter] = entry.mesh;
		}
		int wordIndex = Random.Range(0, words.Length);
		foreach (var c in words[wordIndex]) {
			if (c != '\r') {
				Debug.Log(c);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
