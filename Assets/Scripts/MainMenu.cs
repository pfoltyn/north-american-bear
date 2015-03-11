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
				clickAudio.Play();
				Animator animator = hit.collider.GetComponent<Animator>();
				animator.SetBool("Pressed", true);
				if (choiceSlots[0] == animator.gameObject)
				{
					fader.Stop(() => Application.LoadLevel("4letters"));
				}
				else if (choiceSlots[1] == animator.gameObject)
				{
					fader.Stop(() => Application.LoadLevel("5letters_easy"));
				}
				else
				{
					fader.Stop(() => Application.LoadLevel("5letters"));
				}
			}
		}
	}
	
    void Update()
    {
		float offsetY = Mathf.Sin(2 * Mathf.PI * Time.time * scrollSpeedY);
		float offsetX = Mathf.Cos(2 * Mathf.PI * Time.time * scrollSpeedX);
		backgroundRenderer.material.mainTextureOffset = new Vector2(offsetY, offsetX);
    }
}
