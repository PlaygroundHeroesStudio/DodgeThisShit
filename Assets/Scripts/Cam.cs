using UnityEngine;
using System.Collections;

public class Cam : MonoBehaviour
{
	private Transform Tr = null;

	public Transform Target = null;

	public Vector3 Offset = Vector3.zero;

	void Awake()
	{
		Tr = transform;
	}

	void Update()
	{
		if (Target == null)
			return;

		Tr.position = Target.position + Offset;
	}
}