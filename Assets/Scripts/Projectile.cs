using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : TransformObject
{
	Rigidbody Rigid = null;

	public float MoveSpeed = 20.0f;

	public float Duration = 3.0f;

	float Timer = 0.0f;

	protected override void Awake()
	{
		base.Awake();
		Rigid = rigidbody;
	}
	
	void FixedUpdate()
	{
		Rigid.MovePosition(Rigid.position + Tr.forward * MoveSpeed * Time.fixedDeltaTime);

		Timer += Time.fixedDeltaTime;

		if (Timer > Duration)
			IsActive = false;
	}

	void OnTriggerEnter(Collider C)
	{
		Obstacle O = C.GetComponent<Obstacle>();

		if (O != null)
		{
			O.IsActive = false;
			IsActive = false;
		}
	}

	void OnDisable()
	{
		Timer = 0.0f;
	}
}