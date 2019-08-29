using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public Camera cam;
    public float transitionDuration = 8f;

    public void move(float yDelta)
	{
		StartCoroutine(moveCamera(yDelta));
	}

	private IEnumerator moveCamera(float yDelta)
	{
		float t = 0.0f;
		Vector3 startingPos = cam.transform.position;
		Vector3 target = startingPos + Vector3.up * yDelta;
		while (t < 1.0f)
		{
			t += Time.deltaTime * (Time.timeScale / transitionDuration);

			cam.transform.position = Vector3.Lerp(startingPos, target, t);
			yield return 0;
		}

	}
}
