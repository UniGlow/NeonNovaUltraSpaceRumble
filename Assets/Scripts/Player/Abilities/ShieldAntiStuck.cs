using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>

[RequireComponent(typeof(Collider))]
public class ShieldAntiStuck : MonoBehaviour 
{

	#region Variable Declarations
	// Serialized Fields
	
	[SerializeField] float physicsRelativeXShapeOffset = 0f;
	[SerializeField] float physicsRelativeYShapeOffset = 0f;
	[SerializeField] Vector3 physicsCubeDimensions = Vector3.zero;
	// Private
	bool blocked = false;
	Collider shieldCollider = null;
	#endregion



	#region Public Properties

	#endregion



	#region Unity Event Functions
	private void Awake()
	{
		if (shieldCollider == null)
			shieldCollider = GetComponent<Collider>();
	}
	private void OnEnable () 
	{
		Debug.Log("Starting Anti-Stuck Protocol");
		Collider[] touchingColliders = Physics.OverlapBox(transform.position + (transform.forward * physicsRelativeXShapeOffset) + (transform.up * physicsRelativeYShapeOffset), physicsCubeDimensions, Quaternion.LookRotation(transform.forward, transform.up));
		if (touchingColliders.Length != 0)
		{
			foreach(Collider c in touchingColliders)
			{
				if(c.tag == Constants.TAG_BOSS)
				{
					blocked = true;
				}
			}
		}
		else
			blocked = false;

		if (blocked)
			shieldCollider.enabled = false;
		else
			shieldCollider.enabled = true;
	}

	private void FixedUpdate()
	{
		if (blocked) {
			Collider[] touchingColliders = Physics.OverlapBox(transform.position + (transform.forward * physicsRelativeXShapeOffset) + (transform.up * physicsRelativeYShapeOffset), physicsCubeDimensions, Quaternion.LookRotation(transform.forward, transform.up));
			if (touchingColliders.Length != 0)
			{
				foreach (Collider c in touchingColliders)
				{
					blocked = false;
					if (c.tag == Constants.TAG_BOSS)
					{
						blocked = true;
					}
				}
			}
			else
				blocked = false;
		}
		else shieldCollider.enabled = true;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireCube(transform.position + (transform.forward * physicsRelativeXShapeOffset) + (transform.up * physicsRelativeYShapeOffset), physicsCubeDimensions);
	}
	#endregion



	#region Public Functions

	#endregion



	#region Private Functions

	#endregion



	#region GameEvent Raiser

	#endregion



	#region Coroutines

	#endregion
}

