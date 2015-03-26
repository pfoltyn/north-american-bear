using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Settings : MonoBehaviour
{
	public GameObject[] choiceSlots;
	public Mesh Empty;
	public Mesh Ticked;

	void Awake()
	{
		Fader.FadeIn();
	}

    void Start()
    {
		Utils.RandomiseAnimationSpeed(choiceSlots);

		if (!GameMusic.sounds)
		{
			ToggleMesh(choiceSlots[1].GetComponent<MeshFilter>(), GameMusic.soundsId);
		}
		if (!GameMusic.music)
		{
			ToggleMesh(choiceSlots[2].GetComponent<MeshFilter>(), GameMusic.musicId);
		}
		if (!MovingBackground.movingBackground)
		{
			ToggleMesh(choiceSlots[3].GetComponent<MeshFilter>(), MovingBackground.movBgId);
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
				Fader.FadeOut(() => Application.LoadLevel(Utils.lvlMenu));
				enabled = false;
			}
			else if (choiceSlots[1] == gameObject)
			{
				GameMusic.sounds = ToggleMesh(meshFilter, GameMusic.soundsId);
			}
			else if (choiceSlots[2] == gameObject)
			{
				GameMusic.music = ToggleMesh(meshFilter, GameMusic.musicId);
				GameMusic.ChangeMusic();
			}
			else if (choiceSlots[3] == gameObject)
			{
				MovingBackground.movingBackground = ToggleMesh(meshFilter, MovingBackground.movBgId);
			}
		});
	}
}
