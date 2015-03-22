using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Settings : MonoBehaviour
{
	public GameObject[] choiceSlots;
	public Mesh Empty;
	public Mesh Ticked;

	private GameObject gameMusic;
	
    void Start()
    {
		Utils.RandomiseAnimationSpeed(choiceSlots);

		gameMusic = GameObject.Find("GameMusic");

		if (!Utils.sounds)
		{
			ToggleMesh(choiceSlots[1].GetComponent<MeshFilter>(), Utils.soundsId);
		}
		if (!Utils.music)
		{
			ToggleMesh(choiceSlots[2].GetComponent<MeshFilter>(), Utils.musicId);
		}
		if (!Utils.movingBackground)
		{
			ToggleMesh(choiceSlots[3].GetComponent<MeshFilter>(), Utils.movBgId);
		}
    }

	private bool ToggleMesh(MeshFilter meshFilter, string name)
	{
		bool result;
		if (meshFilter.mesh.vertexCount == Empty.vertexCount)
		{
			meshFilter.mesh = Ticked;
			PlayerPrefs.SetInt(name, 1);
			result = true;
		}
		else
		{
			meshFilter.mesh = Empty;
			PlayerPrefs.SetInt(name, 0);
			result = false;
		}
		PlayerPrefs.Save();
		return result;
	}

	void OnGUI()
	{
		Utils.DetectTouch((GameObject gameObject) => {
			MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
			if (choiceSlots[0] == gameObject)
			{
				Fader fader = GameObject.FindGameObjectWithTag("Finish").GetComponent<Fader>();
				fader.Stop(() => Application.LoadLevel(Utils.lvlMenu));
				enabled = false;
			}
			else if (choiceSlots[1] == gameObject)
			{
				Utils.sounds = ToggleMesh(meshFilter, Utils.soundsId);
			}
			else if (choiceSlots[2] == gameObject)
			{
				Utils.music = ToggleMesh(meshFilter, Utils.musicId);
				gameMusic.GetComponent<AudioSource>().mute = !Utils.music;
			}
			else if (choiceSlots[3] == gameObject)
			{
				Utils.movingBackground = ToggleMesh(meshFilter, Utils.movBgId);
			}
		});
	}
}
