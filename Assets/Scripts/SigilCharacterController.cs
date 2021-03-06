﻿using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.Utility;
using UnityStandardAssets.CrossPlatformInput;

// The point of this class is to help a cutter move along the inner surface of
// the Cage.
// Movement is based off of the current 'up' direction which is based off the
// current gravity
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SigilGravityController))]
public class SigilCharacterController : MonoBehaviour
{
	SigilGravityController _gravity;

	// Camera
	[SerializeField] private MouseLook _mouseLook;
	private Camera _camera;
	private Transform _cameraPivot;

	private Rigidbody _rigidbody;

	// Magnitude of Gravity
	// TODO: Move this into like a Sigil Global Class
	private float _gravityForce = 50;   // Not really gravity, just velocity

	private float _speed = 5;
	private float _maxSpeed = 20f;

#region Monobehavior

	void Start()
	{
		_cameraPivot = transform.FindChild("CameraPivot");
		_gravity = GetComponent<SigilGravityController>();
		_rigidbody = GetComponent<Rigidbody>();
		_camera = Camera.main;   // Get the camera from the scene
		_mouseLook.Init( transform, _camera.transform );

		UpdateRotation( _gravity.Gravity );
	}

	void Update()
	{
		RotateView();    // Rotate Camera Based off Mouse Input
	}

	void FixedUpdate()
	{

		// HandleMovement
		var input = GetInput();

		Vector3 desiredMove = _cameraPivot.forward * input.y + _cameraPivot.right * input.x;

		// preserve down movement
		var downVelocity = Vector3.Scale(_cameraPivot.up.Inverse(), _rigidbody.velocity);   // add down velocity back into desired speed to preserve downward movement from gravity

		//if (desiredMove.sqrMagnitude > 0)
		//{
			//desiredMove *= _speed;
		//}

		//desiredMove += downVelocity;

		// Apply Gravity
		//desiredMove += _cameraPivot.up.Inverse() * _gravityForce * Time.fixedDeltaTime;
		//desiredMove += downVelocity * _gravityForce * Time.fixedDeltaTime;

		_rigidbody.MovePosition( transform.position + desiredMove * _speed );
		//_rigidbody.velocity = desiredMove;

		// Apply Gravitas
		UpdateRotation( _gravity.Gravity );
		//_rigidbody.AddForce( _gravity.Gravity * _gravityForce );
	}

#endregion

	// Update rotation based off current gravity
	void UpdateRotation(Vector3 gravity)
	{
		// Now to set my transform rotation so that up is actually up
		transform.up = gravity.Inverse();
	}

	// Return Unit Vector of Input
	private Vector2 GetInput()
	{
		float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
		float vertical   = CrossPlatformInputManager.GetAxis("Vertical");

		return new Vector2( horizontal, vertical ).normalized;
	}

	private void RotateView()
	{
		_mouseLook.LookRotation( _cameraPivot, _camera.transform );
	}

	void TestRotation()
	{
		var gravity = _gravity.Gravity;
		Debug.Log( gravity );
		UpdateRotation(gravity);
	}
}
