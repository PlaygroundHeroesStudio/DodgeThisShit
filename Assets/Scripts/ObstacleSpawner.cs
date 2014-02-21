using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
	public float Radius = 1.0f;

	public float SpawnDistance = 10.0f;

	private float SpawnTimer = 0.0f;

	public Obstacle[] SpawnObjects = null;

	private List<Obstacle> Spawned = new List<Obstacle>();

	void Update()
	{
		SpawnTimer += Player.Singleton.MoveSpeed * Time.deltaTime;

		if (SpawnTimer >= 0.0f)
		{
			Obstacle Spawning = null;

			int SpawnIndex = Random.Range(0, SpawnObjects.Length);

			foreach (Obstacle O in Spawned)
			{
				if (O.PrefabIndex == SpawnIndex && O.position.z + O.Length < Player.Singleton.CamTr.position.z)
				{
					Spawning = O;
					break;
				}
			}

			if (Spawning == null)
			{
				Spawning = (Obstacle)Instantiate(SpawnObjects[SpawnIndex]);

				if (Spawning.SpawnAtCentre)
					foreach (Obstacle O in Spawning.GetComponentsInChildren<Obstacle>())
						O.localPosition = new Vector3(O.localPosition.x * Radius, O.localPosition.y * Radius, O.localPosition.z);

				Spawned.Add(Spawning);
			}
			
			SetPosition(Spawning);

			SpawnTimer -= Spawning.Length;
		}
	}

	void SetPosition(Obstacle Spawning)
	{
		Spawning.localRotation = Quaternion.AngleAxis(Random.value * 360, Vector3.forward);

		if (Spawning.SpawnAtCentre)
			Spawning.localPosition = new Vector3(0.0f, 0.0f, Player.Singleton.position.z + SpawnDistance);
		else
			Spawning.localPosition = Spawning.localRotation * new Vector3(0.0f, -Radius, Player.Singleton.position.z + SpawnDistance);
	}
}