using UnityEngine;
using System.Collections;

public class Fireworks : MonoBehaviour {
	public Transform firework;

	private float closestDistance = 5.0f;
	private float furthestDistance = 9.0f;
	private bool once = true;

	IEnumerator ShootFirework(Vector2 touchPosition)
	{
		Ray ray = Camera.main.ScreenPointToRay(touchPosition);
		Vector3 point = ray.GetPoint(Random.Range(closestDistance, furthestDistance));
		Instantiate(firework, point, Quaternion.identity);
		yield return new WaitForSeconds(.5f);
	}

	IEnumerator IndependenceDay()
	{
		for (;;)
		{
			Vector2 point = new Vector2(Random.Range(10f, Screen.width - 10f), Random.Range(100f, Screen.height - 10f));
			yield return StartCoroutine(ShootFirework(point));
		}
	}

	public void FireAtWill()
	{
		StartCoroutine(IndependenceDay());
	}

	void OnGUI()
	{
		if (once && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
		{
			StartCoroutine(ShootFirework(Input.GetTouch(0).position));
		}
		once = Input.touchCount == 0;
	}
}
