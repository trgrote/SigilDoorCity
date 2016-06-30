using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

// Load the Cells for LOD
// This will eventually load the LOD versions of each cell, in the shape of a
// torus
public class CellLoader : MonoBehaviour
{
	// Width in Units for each call ( needs to be uniform across all cells
	const float cellWidth = 100;

	// List of all cells, I could remove this and replace it with a search and
	// load
	string[] cells = new string[]
	{
		"Cell_000_000",
		"Cell_001_000",
		"Cell_002_000",
		"Cell_003_000",
		"Cell_004_000",
		"Cell_005_000",
		"Cell_006_000",
		"Cell_007_000",
		"Cell_008_000",
		"Cell_009_000",
		"Cell_010_000",
		"Cell_011_000",
		"Cell_012_000",
		"Cell_013_000",
		"Cell_014_000",
		"Cell_015_000",
		"Cell_016_000",
		"Cell_017_000",
		"Cell_018_000",
		"Cell_019_000"
	};

	// Load all the cells in the list and then rotate and move them to place
	// them correctly
	// It Should load the cells starting from the bottom of the circle, in a
	// counter clockwise direction, so cell_01 will be to the right of cell_00,
	// and the last cell will be the left of cell_00.
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
