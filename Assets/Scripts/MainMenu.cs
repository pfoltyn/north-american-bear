using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
	public GameObject[] choiceSlots;
	public Renderer backgroundRenderer;
	public AudioSource clickAudio;

    // Use this for initialization
    void Start()
    {
		foreach (var slot in choiceSlots)
		{
			Animator animator = slot.GetComponent<Animator>();
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
				clickAudio.Play();
				Animator animator = hit.collider.GetComponent<Animator>();
				animator.SetBool("Pressed", true);
				if (choiceSlots[0] == animator.gameObject)
				{
					Application.LoadLevel("4letters");
				}
				else if (choiceSlots[1] == animator.gameObject)
				{
					Application.LoadLevel("5letters_easy");
				}
				else
				{
					Application.LoadLevel("5letters");
				}
			}
		}
    }
}
