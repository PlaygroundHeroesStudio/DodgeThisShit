using UnityEngine;

public class Obstacle : TransformObject
{
	public float Length = 0.0f;

	public bool SpawnAtCentre = false;

	public int PrefabIndex { get; set; }
}