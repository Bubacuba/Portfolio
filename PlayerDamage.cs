using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
	private void OnCollisionEnter2D(Collision2D collision)
	{	
		if (collision.gameObject.name == "Player")
		{
			Debug.Log("Collision Detcted");
			collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(1);
		}
	}
}
