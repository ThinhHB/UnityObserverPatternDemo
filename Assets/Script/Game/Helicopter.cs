using UnityEngine;
using System.Collections;
using DemoObserver;

public class Helicopter : MonoBehaviour
{
	#region Init, config
	[Header("Config helicopter")]
	[Range(1, 10)][SerializeField] int health = 3;
	[SerializeField] Vector2 speedRange = Vector2.zero;
	[SerializeField] GameObject explodeFxPrefab = null;
	[SerializeField] Vector2 limitXrange = Vector2.zero;

	// --- fields
	Rigidbody2D _myRigidbody;
	Transform _myTransform;

	void OnValidate()
	{
		CacheComponents();
		Common.Warning(explodeFxPrefab != null, "Helicopter, missing explodeFxPrefab");
		Common.Warning(_myRigidbody != null, "Helicopter, missing Rigidbody2D");
	}


	void CacheComponents()
	{
		_myRigidbody = GetComponent<Rigidbody2D>();
		_myTransform = transform;
	}


	void Awake()
	{
		CacheComponents();
		// destroy this script if missing data
		if (explodeFxPrefab == null || _myRigidbody == null)
		{
			Destroy(this);
		}
	}

	void Start()
	{
		_currentHP = health;
		// random velocity
		Vector2 velo = Vector2.zero;
		velo.x = Random.Range(speedRange.x, speedRange.y);
		_myRigidbody.velocity = velo;
	}

	#endregion



	#region Update

	void Update()
	{
		var position = _myTransform.position;
		if (position.x < limitXrange.x || position.x > limitXrange.y)
		{
			// raise Miss event
			this.PostEvent(EventID.OnHelicopterEscaped);
			// destroy heli
			Destroy(gameObject);
		}
	}

	#endregion



	#region Damage, lifeTime

	int _currentHP;

	/// <summary>
	/// This function will be call from BUllet script, when Collision occur.
	/// Reduce current HP, if HP lower than zero, then destroy this heli
	/// </summary>
	public void TakeDamage()
	{
		_currentHP--;
		if (_currentHP <= 0)
		{
			// create Fx
			Instantiate(explodeFxPrefab, transform.position, Quaternion.identity);
			// raise Dead event 
			this.PostEvent(EventID.OnHelicopterDead);
			// destroy heli
			Destroy(gameObject);
		}
	}

	#endregion
}