using UnityEngine;
using System.Collections;
using DemoObserver;

public class Bullet : MonoBehaviour
{
	#region Init, config
	[Header("Config bullet")]
	[Range(2f, 10f)][SerializeField] float velocity = 3f;

	//---- fileds
	Rigidbody2D _myRigidbody;

	void OnValidate()
	{
		CacheComponents();
		Common.Warning(_myRigidbody != null, "BUllet, must attach to object that have Rigidbody2D !!!");
	}


	void CacheComponents()
	{
		_myRigidbody = GetComponent<Rigidbody2D>();
	}


	void Awake()
	{
		CacheComponents();
		if (_myRigidbody == null)
		{
			// destroy this component, it won't work if missing rigidbody component
			DestroyImmediate(this);
		}
	}


	void Start()
	{
		// Assign velocity for bullet, the direction already be set in Marine script
		var velo = transform.up * velocity;
		_myRigidbody.velocity = velo;
	}

	#endregion
}