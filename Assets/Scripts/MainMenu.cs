using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
	public GameObject[] choiceSlots;
	public Renderer backgroundRenderer;
	public AudioSource clickAudio;

	private float scrollSpeedY;
	private float scrollSpeedX;

    // Use this for initialization
    void Start()
    {
		scrollSpeedY = 0.02f;
		scrollSpeedX = 0.02f;
		foreach (var slot in choiceSlots)
		{
			Animator animator = slot.GetComponent<Animator>();
			animator.SetFloat("Speed", Random.Range(0f, 1f));
		}
    }

    // Update is called once per frame
    void Update()
    {
		float offsetY = Mathf.Sin(2 * Mathf.PI * Time.time * scrollSpeedY);
		float offsetX = Mathf.Cos(2 * Mathf.PI * Time.time * scrollSpeedX);
		backgroundRenderer.material.mainTextureOffset = new Vector2(offsetY, offsetX);

		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
			RaycastHit hit;
			
			if (Physics.Raycast(ray, out hit))
			{
				clickAudio.Play();
				GameObject go = GameObject.FindGameObjectWithTag("Finish");
				Animator animator = hit.collider.GetComponent<Animator>();
				if (choiceSlots[0] == animator.gameObject)
				{
					go.GetComponent<Fader>().nextSceneName = "4letters";
					go.GetComponent<Fader>().sceneEnding = true;
				}
				else if (choiceSlots[1] == animator.gameObject)
				{
					go.GetComponent<Fader>().nextSceneName = "5letters_easy";
					go.GetComponent<Fader>().sceneEnding = true;
				}
				else
				{
					go.GetComponent<Fader>().nextSceneName = "5letters";
					go.GetComponent<Fader>().sceneEnding = true;
				}
				animator.SetBool("Pressed", true);
			}
		}
    }
}
