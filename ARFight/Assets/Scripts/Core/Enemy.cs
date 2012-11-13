using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	public void SetTarget(Transform transform)
	{
		iTween.MoveTo(gameObject,transform.position,20f);
	}
	public void Die()
	{
		Destroy(gameObject);
	}
}
