using UnityEngine;
using UnityEngine.Events;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;
    public ChessBlueprint house;
    public ChessPlacement Area;
    private ChessBlueprint houseToBuild;
    private PlayerStats _player;

    public UnityEvent<ChessBlueprint> HouseBuilded;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("More than one BM in scene!");
            return;
        }

        instance = this;
    }

    private void Start()
    {
        _player = PlayerStats.Player;
    }

    public bool CanBuild { get { return houseToBuild != null; } }

    private bool CostChecker()
    {
        return _player.Money >= houseToBuild.Cost;
    }

    public void BuildBuildingOn(Node node)
    {
        if (!CostChecker())
        {
            Debug.Log("Not enough money");
            return;
        }

        if (RadiusChecker(node, houseToBuild.RadiusBlocker))
        {
            Debug.Log($"В радиусе {houseToBuild.RadiusBlocker} клеток уже есть такое здание!");
            return;
        }

        _player.AddMoney(-houseToBuild.Cost);

        ChessBlueprint building = Instantiate(houseToBuild, node.transform.position, Quaternion.identity);
        HouseBuilded?.Invoke(houseToBuild);

        node.building = building;
        node.building.transform.parent = node.transform;
        building.transform.localPosition = building.Position;
        building.transform.localRotation = Quaternion.Euler(building.Rotation);
        building.SetNode(node);
        building.MakeEffect();
    }

    private bool RadiusChecker(Node startPoint, int countMultiply)
    {
        for (int i = startPoint.GetNodeIndex().x - countMultiply; i <= startPoint.GetNodeIndex().x + countMultiply; i++)
        {
            if (i < 0 || i > Area.dimensions.x - 1)
                continue;

            for (int j = startPoint.GetNodeIndex().y - countMultiply; j <= startPoint.GetNodeIndex().y + countMultiply; j++)
            {
                if (j < 0 || j > Area.dimensions.y - 1)
                    continue;

                if (Area.m_Tiles[i, j].TakeNode().building == null)
                    continue;

                if (Area.m_Tiles[i, j].TakeNode().building.CompareTag(houseToBuild.tag))
                    return true;
            }
        }

        return false;
    }

    public void SelectHouseToBuild(ChessBlueprint house)
    {
        houseToBuild = house;
    }
}