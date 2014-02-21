﻿using UnityEngine;

public enum DodgeDir
{
	None = 0,
	Up = 1 << 0,
	Down = 1 << 1,
	Left = 1 << 2,
	Right = 1 << 3,
	UpLeft = Up | Left,
	DownLeft = Down | Left,
	UpRight = Up | Right,
	DownRight = Down | Right
}

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
	private enum ControlTypes
	{
		EightDir,
		Orbit
	}

	public static Player Singleton = null;

	private Transform Tr = null;

	private Rigidbody Rigid = null;

	public Camera Cam = null;

	private Transform _CamTr = null;

	public Transform CamTr { get { return _CamTr; } }

	public int MaxHealth = 3;

	private int _Health = 0;

	public int Health
	{
		get { return _Health; }
		set
		{
			_Health = value;

			if (_Health <= 0)
			{
				_Health = 0;
				OnDeath();
			}
			else if (_Health > MaxHealth)
				_Health = MaxHealth;
		}
	}

	public float MoveSpeed = 15.0f;
	public float OrbitSpeed = 180.0f;

	public float Radius = 1.0f;

	private float Angle = 0.0f;

	public float Score = 0.0f;

	public KeyCode KeyUp = KeyCode.W;
	public KeyCode KeyDown = KeyCode.S;
	public KeyCode KeyLeft = KeyCode.A;
	public KeyCode KeyRight = KeyCode.D;

	private ControlTypes CtrlType = ControlTypes.Orbit;

	public DodgeDir Dir { get; set; }

	public Vector3 localPosition { get { return Tr.localPosition; } set { Tr.localPosition = value; } }
	public Vector3 position { get { return Tr.position; } set { Tr.position = value; } }

	void Awake()
	{
		Singleton = this;
		DontDestroyOnLoad(this);

		Tr = transform;
		Rigid = rigidbody;

		_Health = MaxHealth;

		_CamTr = Cam.transform;
	}

	void Update()
	{
		if (CtrlType == ControlTypes.EightDir)
			EightDirControls();
		else
			OrbitControls();

		Score += MoveSpeed * Time.deltaTime;
	}

	void OrbitControls()
	{
		if (Input.GetKey(KeyLeft))
			Angle -= OrbitSpeed * Time.deltaTime;
		else if (Input.GetKey(KeyRight))
			Angle += OrbitSpeed * Time.deltaTime;
		
		Quaternion Rot = Quaternion.AngleAxis(Angle, Vector3.forward);
		Vector3 Pos = Rot * new Vector3(0.0f, -Radius, Tr.localPosition.z + MoveSpeed * Time.deltaTime);
		
		Transform Parent = Tr.parent;
		
		if (Parent != null)
		{
			Rot *= Parent.rotation;
			Pos = Parent.localToWorldMatrix.MultiplyPoint(Pos);
		}
		
		Rigid.MoveRotation(Rot);
		Rigid.MovePosition(Pos);
	}

	void EightDirControls()
	{
		Dir = DodgeDir.None;
		
		Vector3 Temp = Vector3.zero;
		
		if (Input.GetKey(KeyUp))
		{
			Dir |= DodgeDir.Up;
			Temp.y += MoveSpeed * Time.deltaTime;

			if (Temp.y > Radius)
				Temp.y = Radius;
		}
		else if (Input.GetKey(KeyDown))
		{
			Dir |= DodgeDir.Down;
			Temp.y -= MoveSpeed * Time.deltaTime;
			
			if (Temp.y < -Radius)
				Temp.y = -Radius;
		}
		
		if (Input.GetKey(KeyLeft))
		{
			Dir |= DodgeDir.Left;
			Temp.x -= MoveSpeed * Time.deltaTime;
			
			if (Temp.x < -Radius)
				Temp.x = -Radius;
		}
		else if (Input.GetKey(KeyRight))
		{
			Dir |= DodgeDir.Right;
			Temp.x += MoveSpeed * Time.deltaTime;
			
			if (Temp.x > Radius)
				Temp.x = Radius;
		}

		Temp.z = Tr.localPosition.z;
		Tr.localPosition = Temp;
	}

	void OnTriggerEnter(Collider C)
	{
		if (C.GetComponent<Obstacle>() != null)
		{
			C.gameObject.SetActive(false);
			
			Health--;
		}
	}

	void OnGUI()
	{
		GUI.skin.box.alignment = TextAnchor.MiddleLeft;

		GUI.Box(new Rect(Screen.width * 0.05f, Screen.height * 0.05f, 80, 40), string.Concat("Health: ", _Health.ToString(), "\nScore: ", Score.ToString("0")));

		if (Health <= 0 && GUI.Button(new Rect(Screen.width * 0.5f - 100, Screen.height * 0.5f - 40, 200, 80), "Game Over\nRetry"))
		{
			Score = 0.0f;
			Health = MaxHealth;
			Application.LoadLevel(Application.loadedLevel);
			Time.timeScale = 1.0f;
		}
	}

	void OnDeath()
	{
		Time.timeScale = 0.0f;
	}
}