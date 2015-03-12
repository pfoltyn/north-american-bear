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

	private void ToggleMesh(MeshFilter meshFilter)
	{
		if (meshFilter.mesh.vertexCount == Empty.vertexCount)
		{
			meshFilter.mesh = Ticked;
		}
		else
		{
			meshFilter.mesh = Empty;
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
				MeshFilter meshFilter = animator.gameObject.GetComponent<MeshFilter>();
				if (choiceSlots[0] == animator.gameObject)
				{
					fader.Stop(() => Application.LoadLevel("menu"));
				}
				else if (choiceSlots[1] == animator.gameObject)
				{
					ToggleMesh(meshFilter);
				}
				else if (choiceSlots[2] == animator.gameObject)
				{
					ToggleMesh(meshFilter);
				}
				else if (choiceSlots[3] == animator.gameObject)
				{
					ToggleMesh(meshFilter);
				}
			}
		}
	}
	
    void Update()
    {
		float offsetY = Mathf.Sin(2 * Mathf.PI * Time.time * scrollSpeedY);
		float offsetX = Mathf.Cos(2 * Mathf.PI * Time.time * scrollSpeedX);
		backgroundRenderer.material.mainTextureOffset = new Vector2(offsetY, offsetX);
		backgroundRenderer.material.mainTextureScale = new Vector2(3 + offsetY, 3 + offsetX);

		if (Input.touchCount == 0)
		{
			once = true;
		}
    }
}
