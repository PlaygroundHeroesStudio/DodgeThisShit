using UnityEngine;

public class TransformObject : MonoBehaviour
{
	protected GameObject GO = null;

	protected Transform Tr = null;

	public Vector3 localPosition { get { return Tr.localPosition; } set { Tr.localPosition = value; } }
	public Vector3 position { get { return Tr.position; } set { Tr.position = value; } }
	
	public Quaternion localRotation { get { return Tr.localRotation; } set { Tr.localRotation = value; } }
	public Quaternion rotation { get { return Tr.rotation; } set { Tr.rotation = value; } }
	
	public Transform parent { get { return Tr.parent; } set { Tr.parent = value; } }
	
	public bool IsActive { get { return GO.activeSelf && GO.activeInHierarchy; } set { GO.SetActive(value); } }

	protected virtual void Awake()
	{
		GO = gameObject;
		Tr = transform;
	}
}