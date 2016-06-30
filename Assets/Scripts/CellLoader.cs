using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

// The purpose of this script tis to monitor the player's movement.  If the
// player moves beyond the bounds of a cell, then evaluate if we need to load
// another cell and unload a cell behind him.
public class CellLoader : MonoBehaviour
{
	const float cellWidth = 100;
	string[] cells = new string[]
	{
		"Cell_00",
		"Cell_01",
		"Cell_02",
		"Cell_03",
		"Cell_04",
		"Cell_05",
		"Cell_06",
		"Cell_07",
		"Cell_08",
		"Cell_09",
		"Cell_10",
		"Cell_11",
		"Cell_12",
		"Cell_13",
		"Cell_14",
		"Cell_15",
		"Cell_16",
		"Cell_17",
		"Cell_18",
		"Cell_19"
	};

	IEnumerator Start()
	{
		float change_in_rotation = 360f / (float) cells.Length;
		float radius = ( cellWidth * cells.Length ) / ( 2 * Mathf.PI );

		for ( int i = 0 ; i < cells.Length; ++i )
		{
			// Start the Load for this level
			yield return SceneManager.LoadSceneAsync(cells[i], LoadSceneMode.Additive);

			var scene = SceneManager.GetSceneByName(cells[i]);

			// Find GameObject Named
			var rootObjects = scene.GetRootGameObjects();

			// Find the base object
			GameObject parent = null;

			foreach (var obj in rootObjects)
			{
				if ( obj.name == "Parent" )
				{
					parent = obj;
				}
			}

			// Rotate parent
			if ( parent )
			{
				// Find Ground
				var ground = parent.transform.Find("Ground");

				if ( ground )
				{
					// Cool, rotate parent, then move ground down
					parent.transform.rotation = Quaternion.Euler( 0, 0, change_in_rotation * i );

					ground.transform.localPosition = new Vector3( 0, -radius, 0 );
				}
				else
				{
					Debug.LogWarning( "Loaded " + cells[0] + " but could not find Parent->Ground object in hierarchy");
				}
			}
			else
			{
				Debug.LogWarning( "Loaded " + cells[0] + " but could not find Parent object in hierarchy");
			}
		}
	}
}
