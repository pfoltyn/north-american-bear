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
		Color[] colorArray = new Color[]{
			Color.blue, Color.cyan, Color.magenta, Color.green, Color.red, Color.yellow
		};
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		Vector2[] uv = mesh.uv;
		Color[] colors = new Color[uv.Length];
		int colorIndex0 = 0;
		int colorIndex1 = 1;
		Color currentColor0 = colorArray[colorIndex0];
		Color currentColor1 = colorArray[colorIndex1];

		bgRenderer.material.mainTexture = textures[(int)Time.time % textures.Length];

		for (;;)
		{
			currentColor0 = Color.Lerp(currentColor0, colorArray[(colorIndex0 + 1) % colorArray.Length], Time.deltaTime);
			currentColor1 = Color.Lerp(currentColor1, colorArray[(colorIndex1 + 1) % colorArray.Length], Time.deltaTime);
			if ((Mathf.Abs(currentColor1.r - colorArray[(colorIndex1 + 1) % colorArray.Length].r) < 0.01f) &&
			    (Mathf.Abs(currentColor1.g - colorArray[(colorIndex1 + 1) % colorArray.Length].g) < 0.01f) &&
			    (Mathf.Abs(currentColor1.b - colorArray[(colorIndex1 + 1) % colorArray.Length].b) < 0.01f))
			{
				currentColor1 = colorArray[(colorIndex1 + 1) % colorArray.Length];
				colorIndex1++;
			}
			if ((Mathf.Abs(currentColor0.r - colorArray[(colorIndex0 + 1) % colorArray.Length].r) < 0.01f) &&
			    (Mathf.Abs(currentColor0.g - colorArray[(colorIndex0 + 1) % colorArray.Length].g) < 0.01f) &&
			 	(Mathf.Abs(currentColor0.b - colorArray[(colorIndex0 + 1) % colorArray.Length].b) < 0.01f))
			{
				currentColor0 = colorArray[(colorIndex0 + 1) % colorArray.Length];
				colorIndex0++;
			}
			for (int i = 0; i < uv.Length; i++)
				colors[i] = Color.Lerp(currentColor0, currentColor1, uv[i].x);
			mesh.colors = colors;

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
