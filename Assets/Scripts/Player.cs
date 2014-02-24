using UnityEngine;

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
public class Player : TransformObject
{
	private enum ControlTypes
	{
		EightDir,
		Orbit
	}

	public static Player Singleton = null;

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

	public float FireDelay = 0.25f;

	float FireTimer = 0.0f;

	public KeyCode KeyUp = KeyCode.W;
	public KeyCode KeyDown = KeyCode.S;
	public KeyCode KeyLeft = KeyCode.A;
	public KeyCode KeyRight = KeyCode.D;
	public KeyCode KeyShoot = KeyCode.Space;

	public Projectile Proj = null;

	public Transform ProjectileParent = null;

	Projectile[] Projectiles = new Projectile[20];

	private ControlTypes CtrlType = ControlTypes.Orbit;

	public DodgeDir Dir { get; set; }

	protected override void Awake()
	{
		Singleton = this;

		base.Awake();

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

		FireTimer += Time.deltaTime;

		if (FireTimer > FireDelay)
		{
			if (Input.GetKey(KeyShoot))
				Shoot();
			else
				FireTimer = FireDelay;
		}
	}

	void FixedUpdate()
	{
		Quaternion Rot = Quaternion.AngleAxis(Angle, Vector3.forward);
		Vector3 Pos = Rot * new Vector3(0.0f, -Radius, position.z + MoveSpeed * Time.fixedDeltaTime);
		
		Transform Parent = Tr.parent;
		
		if (Parent != null)
		{
			Rot *= Parent.rotation;
			Pos = Parent.localToWorldMatrix.MultiplyPoint(Pos);
		}
		
		Rigid.MoveRotation(Rot);
		Rigid.MovePosition(Pos);
	}

	void OrbitControls()
	{
		if (Input.GetKey(KeyLeft))
			Angle -= OrbitSpeed * Time.deltaTime;
		else if (Input.GetKey(KeyRight))
			Angle += OrbitSpeed * Time.deltaTime;
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

	void Shoot()
	{
		for (int P = 0; P < Projectiles.Length; P++)
		{
			if (Projectiles[P] == null)
			{
				Projectiles[P] = (Projectile)Instantiate(Proj, Tr.position + Tr.forward, Quaternion.identity);
				Projectiles[P].parent = ProjectileParent;
				FireTimer -= FireDelay;
				break;
			}

			if (!Projectiles[P].IsActive)
			{
				Projectiles[P].position = Tr.position + Tr.forward;
				Projectiles[P].IsActive = true;
				FireTimer -= FireDelay;
				break;
			}
		}
	}

	void OnTriggerEnter(Collider C)
	{
		Obstacle O = C.GetComponent<Obstacle>();

		if (O != null)
		{
			O.IsActive = false;
			
			Health--;
		}
	}

	void OnGUI()
	{
		GUI.skin.box.alignment = TextAnchor.MiddleLeft;
		GUI.skin.box.fontSize = (int)(Screen.width * 0.025f);

		GUI.Box(new Rect(Screen.width * 0.05f, Screen.height * 0.05f, Screen.width * 0.2f, Screen.height * 0.15f), string.Concat("Health: ", _Health.ToString(), "\nScore: ", Score.ToString("0")));

		if (Health <= 0)
		{
			GUI.skin.box.alignment = TextAnchor.MiddleCenter;

			GUI.Box(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.5f - 220, 400, 100), "GAME OVER");

			if ((int)Score > PlayerPrefs.GetInt("HighScore", 0))
				GUI.Box(new Rect(Screen.width * 0.5f - 100, Screen.height * 0.5f - 100, 200, 40), "New High Score!");

			if (GUI.Button(new Rect(Screen.width * 0.5f - 100, Screen.height * 0.5f - 40, 200, 80), "Retry"))
			{
				if (Score > PlayerPrefs.GetInt("HighScore", 0))
					PlayerPrefs.SetInt("HighScore", (int)Score);

				Score = 0.0f;
				Health = MaxHealth;
				Application.LoadLevel(Application.loadedLevel);
				Time.timeScale = 1.0f;
			}
			
			if (GUI.Button(new Rect(Screen.width * 0.5f - 100, Screen.height * 0.5f + 60, 200, 80), "Main Menu"))
			{
				Application.LoadLevel("Main Menu");
				Time.timeScale = 1.0f;
			}
		}
	}

	void OnDeath()
	{
		Time.timeScale = 0.0f;
	}
}