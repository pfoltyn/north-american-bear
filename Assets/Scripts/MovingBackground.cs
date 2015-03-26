using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class MovingBackground : MonoBehaviour
{
	public const string movBgId = "moving_background";

	public Texture[] textures;

	public static bool movingBackground;

	void Awake()
	{
		movingBackground = PlayerPrefs.GetInt(movBgId, 1) == 1;
	}

	private IEnumerator MoveBackground()
	{
		float scrollSpeedY = 0.02f;
		float scrollSpeedX = 0.02f;
		float counter = 0f;
		float offsetX = 0f;
		float offsetY = 0f;
		Renderer bgRenderer = GetComponent<Renderer>();
		Color addMe = new Color(.5f, .5f, .5f, 0f);
		Color[] colorArray = new Color[]{
			Color.blue + addMe, Color.cyan + addMe, Color.magenta + addMe,
			Color.green + addMe, Color.red + addMe, Color.yellow + addMe
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
			if ((Mathf.Abs(currentColor1.r - colorArray[(colorIndex1 + 1) % colorArray.Length].r) < 0.1f) &&
			    (Mathf.Abs(currentColor1.g - colorArray[(colorIndex1 + 1) % colorArray.Length].g) < 0.1f) &&
			    (Mathf.Abs(currentColor1.b - colorArray[(colorIndex1 + 1) % colorArray.Length].b) < 0.1f))
			{
				colorIndex1 = Random.Range(0, colorArray.Length);
			}
			if ((Mathf.Abs(currentColor0.r - colorArray[(colorIndex0 + 1) % colorArray.Length].r) < 0.1f) &&
			    (Mathf.Abs(currentColor0.g - colorArray[(colorIndex0 + 1) % colorArray.Length].g) < 0.1f) &&
			 	(Mathf.Abs(currentColor0.b - colorArray[(colorIndex0 + 1) % colorArray.Length].b) < 0.1f))
			{
				colorIndex0 = Random.Range(0, colorArray.Length);
			}
			for (int i = 0; i < uv.Length; i++)
				colors[i] = Color.Lerp(currentColor0, currentColor1, uv[i].x);
			mesh.colors = colors;

			offsetY = Mathf.Sin(2 * Mathf.PI * counter * scrollSpeedY);
			offsetX = Mathf.Cos(2 * Mathf.PI * counter * scrollSpeedX);
			bgRenderer.material.mainTextureOffset = new Vector2(offsetY, offsetX);
			bgRenderer.material.mainTextureScale = new Vector2(3 + offsetY / 2, 3 + offsetX / 2);
			counter += (movingBackground) ? Time.deltaTime : 0;

			yield return null;
		}
	}

    void Start()
    {
		StartCoroutine(MoveBackground());
    }
}
