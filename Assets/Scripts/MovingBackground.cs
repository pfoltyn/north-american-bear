using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class MovingBackground : MonoBehaviour
{
	public Texture[] textures;

	private IEnumerator MoveBackground()
	{
		float scrollSpeedY = 0.02f;
		float scrollSpeedX = 0.02f;
		float counter = 0f;
		float offsetX = 0f;
		float offsetY = 0f;
		Renderer bgRenderer = GetComponent<Renderer>();

		bgRenderer.material.mainTexture = textures[(int)Time.time % textures.Length];

		for (;;)
		{
			offsetY = Mathf.Sin(2 * Mathf.PI * counter * scrollSpeedY);
			offsetX = Mathf.Cos(2 * Mathf.PI * counter * scrollSpeedX);
			bgRenderer.material.mainTextureOffset = new Vector2(offsetY, offsetX);
			bgRenderer.material.mainTextureScale = new Vector2(3 + offsetY / 2, 3 + offsetX / 2);
			counter += (Utils.movingBackground) ? Time.deltaTime : 0;
			yield return null;
		}
	}

    void Start()
    {
		StartCoroutine("MoveBackground");
    }
}
