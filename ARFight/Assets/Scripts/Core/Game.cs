using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {
	public Player player;
	public static Game instance;
	void Awake()
	{
		instance = this;
	}

	void OnEnable()
	{
		TouchProcessor.enemyTapEvent += OnEnemyTap;
	}

	void OnDisable()
	{
		TouchProcessor.enemyTapEvent -= OnEnemyTap;	
	}

	public void OnEnemyTap(GameObject o)
	{
		o.GetComponent<Enemy>().Die();
	}
}
