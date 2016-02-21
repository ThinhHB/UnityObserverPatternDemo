using UnityEngine;
using System.Collections;
using DemoObserver;

public class Bullet : MonoBehaviour
{
	#region Init, config
	[Header("Config bullet")]
	[Range(2f, 10f)][SerializeField] float velocity = 3f;
	[SerializeField] GameObject hitFxPrefab = null;

	//---- fileds
	Rigidbody2D _myRigidbody;
	Collider2D _myCollider;

	void OnValidate()
	{
		CacheComponents();
		Common.Warning(_myRigidbody != null, "BUllet, must attach to object that have Rigidbody2D !!!");
		Common.Warning(_myCollider != null, "BUllet, must attach to object that have Collider2D !!!");
		Common.Warning(_myCollider.isTrigger, "BUllet, Collider2D must mark IsTrigger = true");
		Common.Warning(hitFxPrefab != null, "BUllet, missing hitFxPrefab");
	}


	void CacheComponents()
	{
		_myRigidbody = GetComponent<Rigidbody2D>();
		_myCollider = GetComponent<Collider2D>();
	}


	void Awake()
	{
		CacheComponents();
		if (_myRigidbody == null || _myCollider == null || hitFxPrefab == null)
		{
			// destroy this component, it won't work if missing rigidbody component
			DestroyImmediate(this);
		}
		else
		{
			_myCollider.isTrigger = true;
		}
	}


	void Start()
	{
		// Assign velocity for bullet, the direction already be set in Marine script
		var velo = transform.up * velocity;
		_myRigidbody.velocity = velo;
	}

	#endregion



	#region Collision handle

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.tag == ObjectsTag.Helicopter)
		{
			var heliScript = col.gameObject.GetComponent<Helicopter>();
			if (heliScript != null)//hit helicopter
			{
				// give damage on helicopter
				heliScript.TakeDamage();
				// raise hit event
				this.PostEvent(EventID.OnBulletHit);
				// destroy
				Instantiate(hitFxPrefab, transform.position, Quaternion.identity);
				Destroy(gameObject);
			}
		}
	}

	#endregion
}