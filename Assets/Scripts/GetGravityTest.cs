using UnityEngine;
using System.Collections;

// Test is just to grab the CellLoad of the scene and then when the user presses
// g, print ouf the current gravity
public class GetGravityTest : MonoBehaviour
{

	CellLoader loader;

	// Use this for initialization
	void Start ()
	{
		loader = GameObject.FindObjectOfType<CellLoader>();

		if ( loader == null )
		{
			Debug.LogWarning("Failed to find CellLoader in scene");
		}

	}

	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown( KeyCode.G ) )
		{
			Debug.Log( loader.GetGravityAtPosition( transform.position ) );
		}
	}
}
