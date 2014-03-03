using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : TransformObject
{
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

	public KeyCode KeyShoot = KeyCode.Space;

	public Projectile Proj = null;

	public Transform ProjectileParent = null;

	Projectile[] Projectiles = new Projectile[20];

	public bool IsAlive { get { return _Health > 0;  } }

	private int HighScore = 0;

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
#if UNITY_ANDROID || UNITY_IPHONE
		Angle += Input.acceleration.x * OrbitSpeed * Time.deltaTime;
#else
		Angle += Input.GetAxis("Horizontal") * OrbitSpeed * Time.deltaTime;
#endif
		Score += MoveSpeed * Time.deltaTime;

		FireTimer += Time.deltaTime;

		if (FireTimer >= FireDelay)
		{
#if UNITY_ANDROID || UNITY_IPHONE
			if (GetTapped())
#else
			if (Input.GetKey(KeyShoot))
#endif
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

		if (!IsAlive)
		{
			GUI.skin.box.alignment = TextAnchor.MiddleCenter;

			GUI.Box(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.5f - 220, 400, 100), "GAME OVER");

			if ((int)Score > HighScore)
				GUI.Box(new Rect(Screen.width * 0.5f - 100, Screen.height * 0.5f - 100, 200, 40), "New High Score!");

			if (GUI.Button(new Rect(Screen.width * 0.5f - 100, Screen.height * 0.5f - 40, 200, 80), "Retry"))
			{
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
		HighScore = PlayerPrefs.GetInt("HighScore", 0);

		if (Score > HighScore)
			PlayerPrefs.SetInt("HighScore", (int)Score);
	}

	bool GetTapped()
	{
		foreach (Touch T in Input.touches)
			if (T.phase == TouchPhase.Began)
				return true;

		return false;
	}
}