using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour {
	public GameObject enemyPrefab;
	public float minDelay;
	public float maxDelay;
	private float nextSpawnTime = 0f;
	void Update () {
		if(Time.timeSinceLevelLoad > nextSpawnTime)
		{
			nextSpawnTime = Time.timeSinceLevelLoad + UnityEngine.Random.Range(minDelay,maxDelay);
			Spawn();
		}
	}

	void Spawn()
	{
		GameObject go = (GameObject)GameObject.Instantiate(enemyPrefab,transform.position + new Vector3(UnityEngine.Random.Range(0, 5),UnityEngine.Random.Range(0, 5),UnityEngine.Random.Range(0, 5)),Quaternion.identity);
		go.GetComponent<Enemy>().SetTarget(Game.instance.player.transform);
	}
}
