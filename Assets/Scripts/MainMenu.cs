using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
	public GameObject[] choiceSlots;
	public Renderer backgroundRenderer;
	public AudioSource clickAudio;

	private Fader fader;
	private float scrollSpeedY;
	private float scrollSpeedX;
	private bool once;
	private bool sounds;
	private bool movingBackground;

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
		sounds = PlayerPrefs.GetInt("sounds", 1) == 1;
		movingBackground = PlayerPrefs.GetInt("moving_background", 1) == 1;
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
					animator.SetBool("Pressed", true);
					fader.Stop(() => Application.LoadLevel("4letters"));
				}
				else if (choiceSlots[1] == animator.gameObject)
				{
					animator.SetBool("Pressed", true);
					fader.Stop(() => Application.LoadLevel("5letters_easy"));
				}
				else if (choiceSlots[2] == animator.gameObject)
				{
					animator.SetBool("Pressed", true);
					fader.Stop(() => Application.LoadLevel("5letters"));
				}
				else
				{
					fader.Stop(() => Application.LoadLevel("settings"));
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
    }
}
