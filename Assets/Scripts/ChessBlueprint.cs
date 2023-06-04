using System.Collections;
using UnityEngine;

[System.Serializable]

public class ChessBlueprint : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int _cost;
    [SerializeField] private int _radiusBlocker = 0;

    [Header("Placement")]
    [SerializeField] private Vector3 _position;
    [SerializeField] private Vector3 _rotation;

    protected Node Node;

    public int Cost => _cost;
    public int RadiusBlocker => _radiusBlocker;

    public Vector3 Position => _position;
    public Vector3 Rotation => _rotation;
    public Node PlacementNode => Node;

    public void SetNode(Node node)
    {
        Node = node;
    }

    public virtual void MakeEffect()
    {

    }
}
