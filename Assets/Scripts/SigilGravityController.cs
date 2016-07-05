using UnityEngine;
using System.Collections;

// This controller will be placed on objects who want their gravity dynamically
// chnaged based off their location in Sigil
[RequireComponent(typeof(Rigidbody))]
public class SigilGravityController : MonoBehaviour
{
	CellLoader loader;

	// Return the Up directrion based off the current transform
	public Vector3 Up
	{
		get
		{
			return loader.GetGravityAtPosition( transform.position ).Inverse();
		}
	}

	public Vector3 Down
	{
		get
		{
			return loader.GetGravityAtPosition( transform.position ).normalized;
		}
	}

	public Vector3 Gravity
	{
		get
		{
			return loader.GetGravityAtPosition( transform.position );
		}
	}

	void Awake()
	{
		loader = GameObject.FindObjectOfType<CellLoader>();

		if ( loader == null )
		{
			Debug.LogWarning("Failed to find CellLoader in scene");
		}
	}

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void FixedUpdate ()
	{
		// Add Force based off current gravity

		// Rotate Collider so the camera will always bo parallel to the ground
	}
}
