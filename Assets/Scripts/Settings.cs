using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Settings : MonoBehaviour
{
	public GameObject[] choiceSlots;
	public Renderer backgroundRenderer;
	public AudioSource clickAudio;
	public Mesh Empty;
	public Mesh Ticked;

	private Fader fader;
	private float scrollSpeedY;
	private float scrollSpeedX;
	private bool once;
	private bool sounds;
	private bool music;
	private bool movingBackground;
	private GameObject gameMusic;

    // Use this for initialization
    void Start()
    {
		once = true;
		scrollSpeedY = 0.02f;
		scrollSpeedX = 0.02f;
		fader = GameObject.FindGameObjectWithTag("Finish").GetComponent<Fader>();
		foreach (var slot in choiceSlots)
		{
			Animator animator = slot.GetComponent<Animator>();
			animator.SetFloat("Speed", Random.Range(0f, 1f));
		}

		gameMusic = GameObject.Find("GameMusic");

		sounds = PlayerPrefs.GetInt("sounds", 1) == 1;
		if (!sounds)
		{
			ToggleMesh(choiceSlots[1].GetComponent<MeshFilter>(), "sounds");
		}
		music = PlayerPrefs.GetInt("music", 1) == 1;
		if (!music)
		{
			ToggleMesh(choiceSlots[2].GetComponent<MeshFilter>(), "music");
		}
		movingBackground = PlayerPrefs.GetInt("moving_background", 1) == 1;
		if (!movingBackground)
		{
			ToggleMesh(choiceSlots[3].GetComponent<MeshFilter>(), "moving_background");
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
				MeshFilter meshFilter = animator.gameObject.GetComponent<MeshFilter>();
				if (choiceSlots[0] == animator.gameObject)
				{
					fader.Stop(() => Application.LoadLevel("menu"));
				}
				else if (choiceSlots[1] == animator.gameObject)
				{
					sounds = ToggleMesh(meshFilter, "sounds");
				}
				else if (choiceSlots[2] == animator.gameObject)
				{
					music = ToggleMesh(meshFilter, "music");
				}
				else if (choiceSlots[3] == animator.gameObject)
				{
					movingBackground = ToggleMesh(meshFilter, "moving_background");
				}
			}
		}
	}
	
    void Update()
    {
		if (movingBackground)
		{
			float offsetY = Mathf.Sin(2 * Mathf.PI * Time.time * scrollSpeedY);
			float offsetX = Mathf.Cos(2 * Mathf.PI * Time.time * scrollSpeedX);
			backgroundRenderer.material.mainTextureOffset = new Vector2(offsetY, offsetX);
			backgroundRenderer.material.mainTextureScale = new Vector2(3 + offsetY, 3 + offsetX);
		}

		if (gameMusic)
		{
			gameMusic.GetComponent<AudioSource>().mute = !music;
		}

		if (Input.touchCount == 0)
		{
			once = true;
		}
    }
}
