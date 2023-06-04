using System.Collections.Generic;
using UnityEngine;

public enum TileState
{
    Filled,
    Empty,
    Infected
}

public class Tile : MonoBehaviour
{
    public Material emptyMaterial;
    public Material filledMaterial;
    public Material infectedMaterial;
    public Renderer tileRenderer;

    [HideInInspector]
    public Vector2Int TileIndex;
    private void Start()
    {
        tileRenderer = GetComponent<MeshRenderer>();
    }
    public void SetState(TileState newState)
    {
        switch (newState)
        {
            case TileState.Filled:
                if (tileRenderer != null && filledMaterial != null)
                    tileRenderer.sharedMaterial = filledMaterial;

                break;

            case TileState.Empty:
                if (tileRenderer != null && emptyMaterial != null)
                    tileRenderer.sharedMaterial = emptyMaterial;

                break;

            case TileState.Infected:
                if (tileRenderer != null & infectedMaterial != null)
                    tileRenderer.sharedMaterial = infectedMaterial;
                break;
        }
    }

    public Node TakeNode()
    {
        return GetComponent<Node>();
    }
}