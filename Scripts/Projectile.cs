using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	[SerializeField]
	private float speed;
	private Player player;
	[SerializeField]
	private GameObject hitEffect;
	private Vector3 target;
	private Vector3 ini;
	private Vector3 vToTarget;


	private void Start()
	{
		player = FindObjectOfType<Player>();
		target = player.transform.position;
		ini = transform.position;
		target.y += 1f;
		vToTarget = target - ini;
	}

	private void Update()
	{
		transform.position = Vector3.MoveTowards(transform.position, ini + 2 * vToTarget, speed * Time.deltaTime);

		if(ini -  transform.position == 2 * (ini - target))
		{
			// Destroys projectile once it reaches twice the distance it was aiming at
			DestroyProjectile();
		}
	}

	private void OnTriggerEnter(Collider collided)
	{
		// destroys projectile when it collides with another object
		if(collided.CompareTag("Player"))
		{
			// Damages player when hit by projectile
			player.DamagePlayer(30);
		}
		if(!collided.CompareTag("Enemy"))
		{
			// Prevents projectile from being destroyed by hitting enemy itself
			DestroyProjectile();
		}
	}

	void DestroyProjectile()
	{
		Instantiate(hitEffect, transform.position, Quaternion.identity);
		Destroy(gameObject);
	}
}
