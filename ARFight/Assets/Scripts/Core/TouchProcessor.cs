using UnityEngine;
using System.Collections;

public class TouchProcessor : MonoBehaviour
{
	public static bool inputEnabled = true;

	public static event System.Action<GameObject> enemyTapEvent;

	void OnEnable ()
	{
		TouchManager.TapEvent += onTap;
	}

	void onTap (object s, TouchManagerEventArgs a)
	{
		if (inputEnabled) {
			RaycastHit raycastInfo = new RaycastHit ();
			if (Physics.Raycast (Camera.mainCamera.ScreenPointToRay (a.Touch.position), out raycastInfo)) {
				enemyTapEvent (raycastInfo.transform.gameObject);
			}
		}
	}

	void OnDisable ()
	{
		TouchManager.TapEvent -= onTap;
	}
}