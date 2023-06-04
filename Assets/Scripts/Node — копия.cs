using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour
{
    [SerializeField] private Color hoverColor;
    [SerializeField] private List<Material> _buffedMaterial;

    [SerializeField] private Material _millMaterial;
    [SerializeField] private Material _infectedMaterial;
    [SerializeField] private int _maximumAbility = 4;

    private Vector2Int _indexOfNode;
    private bool _hasEffect = false;
    private int _countOfAbility = 0;
    private BuildManager _buildManager;
    private float _darkness = 0f;

    public int CountOfAbility => _countOfAbility;
    public BuildManager BuildManager => _buildManager;
    public UnityEvent<bool> AbilityCountChanged;
    public float Darkness => _darkness;

    [Header("Optional")]

    public ChessBlueprint building;
    private MeshRenderer rend;
    private Color startColor;

    public ChessBlueprint GetHouse()
    {
        return building;
    }

    private void Start()
    {
        rend = GetComponentInChildren<MeshRenderer>();
        startColor = rend.material.color;
        _buildManager = BuildManager.instance;

        if (building != null)
        {
            building = Instantiate(building, transform.position, Quaternion.identity);
            building.transform.parent = transform;
            building.transform.localPosition = building.Position;
            building.transform.localRotation = Quaternion.Euler(building.Rotation);
        }

        _indexOfNode = GetComponent<Tile>().TileIndex;
    }

    public Vector2Int GetNodeIndex()
    {
        return _indexOfNode;
    }

    public void AddAbility(int count)
    {
        _hasEffect = true;
        _countOfAbility += count;

        if (_countOfAbility > _maximumAbility)
            _countOfAbility = _maximumAbility;

        AbilityCountChanged?.Invoke(_hasEffect);
    }

    public void SetBuffedMaterial()
    {
        rend.material = _buffedMaterial[0];
        startColor = rend.material.color;
    }

    public void SetMillZone()
    {
        rend.material = _millMaterial;
        startColor = rend.material.color;
    }

    public void Infected()
    {
        rend.material = _infectedMaterial;
        startColor = rend.material.color;
    }

    public void DisableEffect()
    {
        _hasEffect = false;
    }

    public void Destroying()
    {
        if (building != null)
        {
            Destroy(building);
            Destroy(GetComponentInChildren<SpriteRenderer>());
        }
    }


    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if (!_buildManager.CanBuild)
            return;
        if (building != null)
        {
            Debug.Log("Can't build there!");

            return;
        }

        _buildManager.BuildBuildingOn(this);
    }

    private void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        rend.material.color = hoverColor;
    }

    private void OnMouseExit()
    {
        rend.material.color = startColor;
    }
}