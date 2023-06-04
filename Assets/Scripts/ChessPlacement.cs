using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ChessPlacement: MonoBehaviour
{
    public Tile placementTilePrefab;
    public Vector2Int dimensions;
    float m_InvGridSize;
    public float gridSize = 1;
    public Tile[,] m_Tiles;
    bool[,] m_AvailableCells;
    ///<param name="worldLocation"><see cref="Vector3"/> indicating world space coordinates to convert.</param>
    ///<param name="sizeOffset"><see cref="Vector2"/> indicating size of object to center.</param>

    public Vector2 WorldToGrid(Vector3 worldLocation, Vector2Int sizeOffset)
    {
        Vector3 localLocation = transform.InverseTransformPoint(worldLocation);

        // Scale by inverse grid size
        localLocation *= m_InvGridSize;

        // Offset by half size
        var offset = new Vector3(sizeOffset.x * 0.5f, 0.0f, sizeOffset.y * 0.5f);
        localLocation -= offset;

        int xPos = Mathf.RoundToInt(localLocation.x);
        int yPos = Mathf.RoundToInt(localLocation.z);

        return new Vector2Int(xPos, yPos);
    }
    /// <summary>
    /// Returns the world coordinates corresponding to a grid location.
    /// </summary>
    /// <param name="gridPosition">The coordinate in grid space</param>
    /// <param name="sizeOffset"><see cref="Vector2"/> indicating size of object to center.</param>
    /// <returns>Vector3 containing world coordinates for specified grid cell.</returns>
    public Vector3 GridToWorld(Vector2Int gridPosition, Vector2Int sizeOffset)
    {
        // Calculate scaled local position
        Vector3 localPos = new Vector3(gridPosition.x + (sizeOffset.x * 0.5f), 0, gridPosition.y + (sizeOffset.y * 0.5f)) *
                           gridSize;

        return transform.TransformPoint(localPos);
    }

    /// <summary>
    /// Sets a cell range as being occupied by a tower.
    /// </summary>
    /// <param name="gridPos">The grid location</param>
    /// <param name="size">The size of the item</param>

    public void Occupy(Vector2Int gridPos, Vector2Int size)
    {
        Vector2Int extents = gridPos + size;

        // Validate the dimensions and size
        if ((size.x > dimensions.x) || (size.y > dimensions.y))
        {
            throw new ArgumentOutOfRangeException("size", "Given dimensions do not fit in our grid");
        }

        // Out of range of our bounds
        if ((gridPos.x < 0) || (gridPos.y < 0) ||
            (extents.x > dimensions.x) || (extents.y > dimensions.y))
        {
            throw new ArgumentOutOfRangeException("gridPos", "Given footprint is out of range of our grid");
        }

        // Fill those positions
        for (int y = gridPos.y; y < extents.y; y++)
        {
            for (int x = gridPos.x; x < extents.x; x++)
            {
                m_AvailableCells[x, y] = true;

                // If there's a placement tile, clear it
                if (m_Tiles != null && m_Tiles[x, y] != null)
                {
                    m_Tiles[x, y].SetState(TileState.Filled);
                }
            }
        }
    }

    protected virtual void Awake()
    {
        ResizeCollider();

        // Initialize empty bool array (defaults are false, which is what we want)
        m_AvailableCells = new bool[dimensions.x, dimensions.y];

        // Precalculate inverted grid size, to save a division every time we translate coords
        m_InvGridSize = 1 / gridSize;

        SetUpGrid();
    }

    /// <summary>
    /// Set collider's size and center
    /// </summary>
    void ResizeCollider()
    {
        var myCollider = GetComponent<BoxCollider>();
        Vector3 size = new Vector3(dimensions.x, 0, dimensions.y) * gridSize;
        myCollider.size = size;

        // Collider origin is our bottom-left corner
        myCollider.center = size * 0.5f;
    }

    /// <summary>
    /// Instantiates Tile Objects to visualise the grid and sets up the <see cref="m_AvailableCells" />
    /// </summary>
    protected void SetUpGrid()
    {
        Tile tileToUse = placementTilePrefab;

        if (tileToUse == null)
            return;

        // Create a container that will hold the cells.
        var tilesParent = new GameObject("Container");
        tilesParent.transform.parent = transform;
        tilesParent.transform.localPosition = Vector3.zero;
        tilesParent.transform.localRotation = Quaternion.identity;
        m_Tiles = new Tile[dimensions.x, dimensions.y];

        for (int y = 0; y < dimensions.y; y++)
        {
            for (int x = 0; x < dimensions.x; x++)
            {
                Vector3 targetPos = GridToWorld(new Vector2Int(x, y), new Vector2Int(1, 1));
                targetPos.y += 0.01f;
                Tile newTile = Instantiate(tileToUse);
                newTile.transform.parent = tilesParent.transform;
                newTile.transform.position = targetPos;
                newTile.transform.localRotation = Quaternion.identity;

                m_Tiles[x, y] = newTile;
                newTile.SetState(TileState.Empty);
                newTile.TileIndex = new Vector2Int(x, y);
            }
        }
    }

    public Tile[,] GetField()
    {
        return m_Tiles;
    }

#if UNITY_EDITOR
	/// <summary>
	/// On editor/inspector validation, make sure we size our collider correctly.
	/// Also make sure the collider component is hidden so nobody can mess with its settings to ensure its integrity.
	/// Also communicates the idea that the user should not need to modify those values ever.
	/// </summary>
	void OnValidate()
	{
		// Validate grid size
		if (gridSize <= 0)
		{
			Debug.LogError("Negative or zero grid size is invalid");
			gridSize = 1;
		}

		// Validate dimensions
		if (dimensions.x <= 0 ||
			dimensions.y <= 0)
		{
			Debug.LogError("Negative or zero grid dimensions are invalid");
			dimensions = new Vector2Int(Mathf.Max(dimensions.x, 1), Mathf.Max(dimensions.y, 1));
		}

		// Ensure collider is the correct size
		ResizeCollider();

		GetComponent<BoxCollider>().hideFlags = HideFlags.HideInInspector;
	}

	/// <summary>
	/// Draw the grid in the scene view
	/// </summary>
	void OnDrawGizmos()
	{
		Color prevCol = Gizmos.color;
		Gizmos.color = Color.cyan;

		Matrix4x4 originalMatrix = Gizmos.matrix;
		Gizmos.matrix = transform.localToWorldMatrix;

		// Draw local space flattened cubes
		for (int y = 0; y < dimensions.y; y++)
		{
			for (int x = 0; x < dimensions.x; x++)
			{
				var position = new Vector3((x + 0.5f) * gridSize, 0, (y + 0.5f) * gridSize);
				Gizmos.DrawWireCube(position, new Vector3(gridSize, 0, gridSize));
			}
		}

		Gizmos.matrix = originalMatrix;
		Gizmos.color = prevCol;

		// Draw icon too, in center of position
		Vector3 center = transform.TransformPoint(new Vector3(gridSize * dimensions.x * 0.5f,
															  1,
															  gridSize * dimensions.y * 0.5f));
		Gizmos.DrawIcon(center, "build_zone.png", true);
	}
#endif
}