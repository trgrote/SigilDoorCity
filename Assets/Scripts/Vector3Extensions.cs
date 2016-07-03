using UnityEngine;
using System.Collections;

static public class Vector3Extensions
{
	// Return INverse of vector
	static public Vector3 Inverse( this Vector3 vec )
	{
		return new Vector3( -vec.x, -vec.y, -vec.z );
	}

	// Invert the vector ( modifes vector )
	static public void Invert( this Vector3 vec )
	{
		vec.x = -vec.x;
		vec.y = -vec.y;
		vec.z = -vec.z;
	}

}
