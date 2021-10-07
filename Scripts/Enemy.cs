using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character {

	private Player pplayer;
	private GameObject player;
	[SerializeField, Range(0,360)]
	// stores the field of view that the enemy can detect player within
	private float fovAngle;
	[SerializeField]
	// stores the distance that the player can be detected within
	private float fovDistance;
	public float getFOVDistance()
	{
		return fovDistance;
	}
	public void setFOVDistance(float _fovDistance)
	{
		fovDistance = _fovDistance;
	}
	public LayerMask playerMask;
	public LayerMask obstacleMask;

	// used to represent whether the player can be detected
	private bool playerinRange;
	private bool playerinAngle;

	private Vector3 vectorToPlayer;
	private bool playerVisible;
	[SerializeField]
	private float hoverHeight = 1.2f;
	[SerializeField]
	private float hoverForce = 65.0f;
	[SerializeField]
	private float movementSpeed;
	[SerializeField]
	// Stores the closest distance that the enemy will move towards the player
	private float closestApproach = 1f;
	[SerializeField]
	// How long the enemy will move before changing direction
	private float wanderTime;
	public GameObject projectile;
	private int visibleCount;
	private Vector3 playerUpPosition;

	private void Start()
	{
		StartCoroutine("SearchWithDelay", .2f);

	}

	private void Update()
	{

	}

	public void Awake()
	{
		pplayer = FindObjectOfType<Player>();
		player = pplayer.gameObject;
	}

	private void FixedUpdate()
	{
		Rigidbody rigidbody = GetComponent<Rigidbody>();
		Ray ray = new Ray(transform.position, Vector3.down);
		RaycastHit hit;
		playerUpPosition = new Vector3(player.transform.position.x, player.transform.position.y + 1f, player.transform.position.z);

		if (!playerVisible)
		{
			// If player isn't visible, walks in random direction for a few seconds
			if (wanderTime > 0)
			{
				this.transform.Translate(Vector3.forward * movementSpeed);
				wanderTime -= Time.deltaTime;
			}
			else
			{
				//Debug.Log("New Patrol");
				wanderTime = Random.Range(3.0f, 10.0f);
				//Debug.Log("Time: " + wanderTime);
				int rangle = Random.Range(30, 360);
				transform.eulerAngles = new Vector3(0, rangle, 0);
				//Debug.Log("Angle: " + rangle);
			}

		}
		else
		{
			// moves towards player when detected
			wanderTime = 0;
			transform.LookAt(player.transform);
			Vector3 desiredPosition = playerUpPosition- closestApproach * vectorToPlayer.normalized;
			transform.position = Vector3.MoveTowards(transform.position, desiredPosition, Time.deltaTime);
		}
		if (Physics.Raycast(ray, out hit, hoverHeight))
		{
			// allows enemy to hover
			float heightPercent = (hoverHeight - hit.distance) / hoverHeight;
			Vector3 appliedHoverForce = Vector3.up * heightPercent * hoverForce;
			rigidbody.AddForce(appliedHoverForce, ForceMode.Acceleration);
		}
		transform.position += 0.03f * new Vector3(0,Mathf.Cos(Time.deltaTime),0);
	}

	void CheckPlayerRange()
	{
		// checks if player is within minimum sight distance
		if(Vector3.Magnitude(vectorToPlayer) < fovDistance )
		{
			playerinRange = true;
		}
		else
		{
			playerinRange = false;
		}
	}

	void CheckPlayerAngle()
	{
		// checks whether the player is within the field of view angle
		if(Vector3.Angle(transform.forward, vectorToPlayer.normalized) < fovAngle / 2)
		{
			playerinAngle = true;
		}
		else
		{
			playerinAngle = false;
		}
	}

	IEnumerator SearchWithDelay(float delay)
	{
		while(true)
		{
			yield return new WaitForSeconds(delay);
			Detect();
		}
	}

	public void Detect ()
	{
		vectorToPlayer = player.transform.position - this.transform.position;
		CheckPlayerRange();
		CheckPlayerAngle();
		if (playerinAngle && playerinRange)
		{ 
			// player is visible
			if(!Physics.Raycast(transform.position, vectorToPlayer.normalized, Vector3.Magnitude(vectorToPlayer), obstacleMask))
			{ 
				playerVisible = true;
				//Debug.Log("Detected");
				visibleCount++;
			}
			else
			{
				visibleCount = 0;
				//Debug.Log("Now Undetected");
				playerVisible = false;
			}
		}
		else
		{
			// player isn't visible
			visibleCount = 0;
			playerVisible = false;
		}
		if(visibleCount > 0 && visibleCount % 5 == 0)
		{
			// fires projectile every 2 seconds player is visible for
			Fire();
		}
	}

	public void Fire()
	{
		// creates projectile
		Instantiate(projectile, transform.position, Quaternion.identity);
	}

	public Vector3 DirectionFromAngle(float angle, bool angleIsGlobal)
	{
		if(!angleIsGlobal)
		{
			angle += transform.eulerAngles.y;
		}
		return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle* Mathf.Deg2Rad));
	}
}
