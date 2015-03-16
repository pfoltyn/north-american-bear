using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class MovingBackground : MonoBehaviour
{
	public Texture[] textures;
	
	private float counter;
	private float scrollSpeedY;
	private float scrollSpeedX;
	private Renderer bgRenderer;

	private void MoveBackground()
	{
		float offsetY = Mathf.Sin(2 * Mathf.PI * counter * scrollSpeedY);
		float offsetX = Mathf.Cos(2 * Mathf.PI * counter * scrollSpeedX);
		bgRenderer.material.mainTextureOffset = new Vector2(offsetY, offsetX);
		bgRenderer.material.mainTextureScale = new Vector2(3 + offsetY / 2, 3 + offsetX / 2);
		counter += Time.deltaTime;
	}

    void Start()
    {
		scrollSpeedY = 0.02f;
		scrollSpeedX = 0.02f;
		counter = 0f;
		bgRenderer = GetComponent<Renderer>();
		bgRenderer.material.mainTexture = textures[(int)Time.time % textures.Length];
		MoveBackground();
    }

    void Update()
    {
		if (PlayerPrefs.GetInt("moving_background", 1) == 1)
		{
			MoveBackground();
		}
    }
}
