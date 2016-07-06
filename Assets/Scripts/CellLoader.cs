using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

// Load the Cells for LOD
// This will eventually load the LOD versions of each cell, in the shape of a
// torus
public class CellLoader : MonoBehaviour
{
	// Width in Units for each call ( needs to be uniform across all cells
	const float cellWidth = 100;
	const float cellDepth = 100;

	// According to lore, the ratio of width to the depth is 1 cell depth for
	// every 10 cells in width
	// How many cells exist longways
	const int cellGridWidth = 100;
	// How many cells exist shortways
	const int cellGridDepth = 10;

	// IN Degrees, how big is the hole that is on the inside of the torus
	const float innerRingHoleSize = 45;

	float innerRingRadius = 0.0f;

	float outerRingRadius = 0.0f;

	// Distance from the center of the torus to the center of the outer ring
	// NOT distance from the center of the torus to the outer most edge (R)
	float distanceToCenterOfOuterRing
	{
		get
		{
			return innerRingRadius - outerRingRadius;
		}
	}

	// Return the Gravity force direction at the given position of Sigil
	public Vector3 GetGravityAtPosition(Vector3 position)
	{
		// Find unit vector from center out of the torus to current position
		// Multiply that vector by the inner ring radius.  That should be the
		// center of the outer ring. This should be just X and Y coordinates
		Vector3 closestCenter = position;
		closestCenter.z = 0;   // Not concerned with z atm
		closestCenter.Normalize();   // Direction from center of radius to position cast to just the x-y plane
		closestCenter = closestCenter * distanceToCenterOfOuterRing;    // multiply direction from center by radius size to get the closest center of the outer radius

		// Then find the unit vector from center to the center of the outer ring
		// that was found in the previous step.
		// That should be a unit vecotr pointing from the center of the outer
		// ring to the position given.
		Vector3 gravityDirection = position - closestCenter;
		gravityDirection.Normalize();

		return gravityDirection;
	}

	// Return all cells in the cell folder
	string[] GetAllCellsInDirectory()
	{
		string folderName = Application.dataPath + "/Scenes/Cells";
		var dirInfo = new DirectoryInfo(folderName);
		var allFileInfos = dirInfo.GetFiles("*.unity", SearchOption.AllDirectories);
		var allFileNames = new string[ allFileInfos.Length ];

		for ( int i = 0 ; i < allFileInfos.Length ; ++i )
		{
			var fileInfo = allFileInfos[ i ];
			allFileNames[ i ] = fileInfo.Name;
		}

		return allFileNames;
	}

	// Return a map containing a mapping of [X,Y] => Cell Scene
	// Will Throw a little hissy fit if it can't find a cell named correctly
	Dictionary<Vector2i, Scene> GetAllCells( int width, int height )
	{
		Dictionary<Vector2i, Scene> rval = new Dictionary<Vector2i, Scene>();

		for ( int col = 0 ; col < width ; ++col )
		{
			for ( int row = 0 ; row < height ; ++row )
			{
				// Format SHOULD be Cell_XXX_YYY.unity
				string sceneName = String.Format(
					"Cell_{0}_{1}",
					row.ToString().PadLeft( 3, '0' ),
					col.ToString().PadLeft( 3, '0' )
				);

				var scene = SceneManager.GetSceneByName( sceneName );

				// Make sure the scene is valid
				if ( scene.IsValid() )
				{
					// Add the Scene to the dictionary
					rval[ new Vector2i( row, col ) ] = scene;
				}
				else
				{
					Debug.LogWarning( "Failed to find scene " + sceneName );
				}
			}
		}

		return rval;
	}

	Dictionary<Vector2i, Scene> cellMap = new Dictionary<Vector2i, Scene>();

	// Load all the Cells into the Main Scene one by one
	IEnumerator LoadAllCells()
	{
		// Iterate over all what we think should be loaded
		for ( int col = 0 ; col < cellGridWidth ; ++col )
		{
			for ( int row = 0 ; row < cellGridDepth ; ++row )
			{
				// Format SHOULD be Cell_XXX_YYY.unity
				string sceneName = String.Format(
					"Cell_{0}_{1}",
					col.ToString().PadLeft( 3, '0' ),
					row.ToString().PadLeft( 3, '0' )
				);

				// Load this bad boy up
				yield return SceneManager.LoadSceneAsync( sceneName, LoadSceneMode.Additive);

				var scene = SceneManager.GetSceneByName( sceneName );

				// Make sure the scene is valid
				if ( scene.IsValid() )
				{
					// Add the Scene to the dictionary
					cellMap[ new Vector2i( col, row) ] = scene;

					// Do all this nasty stuff to it

					// The Parent Object is the base most object in a scene, here we
					// rotate the cell along the z-axis.  This siumlates the cell's
					// rotation around the center of Sigil
					//
					// The OuterRingPivot is supposed to be the inside point of the
					// outer ring.  This point is pushed down to the center of the
					// outer ring and then rotated along the x-axis to rotate the cell
					// along the inner width of the torus
					//
					// The Ground object is the plane on which all the cell geometry
					// sits.  It's pushed down by the radius of the outer ring.

					// Find the base object
					var rootObjects = scene.GetRootGameObjects();
					GameObject parent = null;

					// Find Parent in the Cell
					foreach (var obj in rootObjects)
					{
						if ( obj.name == "Parent" )
						{
							parent = obj;
						}
					}

					if ( parent )
					{
						var outerRingPivot = parent.transform.Find("OuterRingPivot");

						if ( outerRingPivot )
						{
							// Find Ground
							var ground = outerRingPivot.transform.Find("Ground");

							if ( ground )
							{
								//  Cool, I got everything

								// First Rotate the parent
								parent.transform.rotation = Quaternion.Euler( 0, 0, change_in_rotation * col );

								// Move the OuterRingPivotDown by InnerRing Radius -
								// OuterRingRadius so that is located in the middle of
								// the outer ring
								outerRingPivot.transform.localPosition = new Vector3(0, -(innerRingRadius - outerRingRadius), 0);
								outerRingPivot.transform.localRotation = Quaternion.Euler(
									180 - ( ( innerRingHoleSize / 2 ) + ( change_in_depth_rotation / 2 ) + ( change_in_depth_rotation * row ) ),
									0,
									0
								);

								// rotate pivot along x-axis so it fits up against the wall

								// Push Ground Down by the radius of the outer circle
								ground.transform.localPosition = new Vector3( 0, -outerRingRadius, 0 );
							}
							else
							{
								Debug.LogWarning( "Loaded " + sceneName + " but could not find Parent->OuterRingPivot->Ground object in hierarchy");
							}
						}
						else
						{
							Debug.LogWarning( "Loaded " + sceneName + " but could not find Parent->OuterRingPivot object in hierarchy");
						}
					}
					else
					{
						Debug.LogWarning( "Loaded " + sceneName + " but could not find Parent object in hierarchy");
					}
				}
				else
				{
					Debug.LogWarning( "Failed to find scene " + sceneName );
				}
			}
		}
	}

	float change_in_rotation;
	float change_in_depth_rotation;

	// Load all the cells in the list and then rotate and move them to place
	// them correctly
	// It Should load the cells starting from the bottom of the circle, in a
	// counter clockwise direction, so cell_01 will be to the right of cell_00,
	// and the last cell will be the left of cell_00.
	// TODO: Afraid going to run into a race condition with external references
	void Start()
	{
		change_in_rotation = 360f / (float) cellGridWidth;
		innerRingRadius = ( cellWidth * cellGridWidth ) / ( 2 * Mathf.PI );

		// Calulate rotaion within the outer ring
		float intialCircumfrence = cellDepth * cellGridDepth;    // Circumfrence if the hole wasn't there
		float units_to_degrees = intialCircumfrence / (360f - innerRingHoleSize);    // Get Ratio of Units to Degrees ( m/d )
		float missing_circumfrence = units_to_degrees * innerRingHoleSize;  // Based off the above ratio, how much circumfrence would the missing chunk have taken? d * (m/d) = m
		float total_depth_circumfrence = missing_circumfrence + intialCircumfrence;  // Combine the width of all the cells + the missing peice to get the full circumfrence

		outerRingRadius = total_depth_circumfrence / ( 2 * Mathf.PI );   // Get the radius of the out ring
		change_in_depth_rotation = ( 360f - innerRingHoleSize ) / (float) cellGridDepth;   // The remaining degrees given out evently amongst the surrounding cells

		StartCoroutine( LoadAllCells() );
	}
}
