using UnityEngine;

public class Obstacle : MonoBehaviour
{
	protected Transform Tr = null;

	public float Length = 0.0f;

	public bool SpawnAtCentre = false;

	public int PrefabIndex { get; set; }
	
	public Vector3 localPosition { get { return Tr.localPosition; } set { Tr.localPosition = value; } }
	public Vector3 position { get { return Tr.position; } set { Tr.position = value; } }
	
	public Quaternion localRotation { get { return Tr.localRotation; } set { Tr.localRotation = value; } }
	public Quaternion rotation { get { return Tr.rotation; } set { Tr.rotation = value; } }

	public Transform parent { get { return Tr.parent; } set { Tr.parent = value; } }
	
	protected virtual void Awake()
	{
		Tr = transform;
	}
}