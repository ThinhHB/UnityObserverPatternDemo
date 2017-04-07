using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DemoObserver;

public class UITextManager : MonoBehaviour
{
	#region Init, config

	[SerializeField] Text shootText = null;
	[SerializeField] Text bulletHitText = null;
	[SerializeField] Text heliDeadText = null;
	[SerializeField] Text heliEscapedText = null;

	void OnValidate()
	{
		Common.Warning(shootText != null, "UITextManager, misisng shootText");
		Common.Warning(bulletHitText != null, "UITextManager, misisng bulletHitText");
		Common.Warning(heliDeadText != null, "UITextManager, misisng heliDeadText");
		Common.Warning(heliEscapedText != null, "UITextManager, misisng heliEscapedText");
	}


	void Awake()
	{
		// if missing data config, then destroy this script
		if (shootText == null || bulletHitText == null || heliDeadText == null || heliEscapedText == null)
		{
			DestroyImmediate(this);
		}
	}


	// Use this for initialization
	void Start ()
	{
		// register to receive events
		this.RegisterListener(EventID.OnMarineShoot, (param) => OnMarineShoot());
		this.RegisterListener(EventID.OnBulletHit, (param) => OnBulletHit());
		this.RegisterListener(EventID.OnHelicopterDead, (param) => OnHelicopterDead());
		this.RegisterListener(EventID.OnHelicopterEscaped, (param) => OnHelicopterEscaped());
	}

	#endregion



	#region Event callback

	int _shootCount = 0;
	int _bulletHitCount = 0;
	int _heliDeadCount = 0;
	int _heliEscapedCount = 0;

	const string SHOOT_TEXT_PREFIX = "Shoot : ";
	const string BULLET_HIT_TEXT_PREFIX = "Hit : ";
	const string HELI_DEAD_TEXT_PREFIX = "Kill : ";
	const string HELI_ESCAPED_TEXT_PREFIX = "Miss : ";

	void OnMarineShoot()
	{
		_shootCount++;
		shootText.text = SHOOT_TEXT_PREFIX + _shootCount;
	}

	void OnBulletHit()
	{
		_bulletHitCount++;
		bulletHitText.text = BULLET_HIT_TEXT_PREFIX + _bulletHitCount;
	}

	void OnHelicopterDead()
	{
		_heliDeadCount++;
		heliDeadText.text = HELI_DEAD_TEXT_PREFIX + _heliDeadCount;
	}

	void OnHelicopterEscaped()
	{
		_heliEscapedCount++;
		heliEscapedText.text = HELI_ESCAPED_TEXT_PREFIX + _heliEscapedCount;
	}

	#endregion
}