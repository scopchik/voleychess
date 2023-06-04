using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placement : MonoBehaviour
{
    [SerializeField] private Chess _chess;
    [SerializeField] private GameObject _field;

    private void OnMouseDown()
    {
        
    }

    public void Place()
    {
        Instantiate(_chess, _field.transform);
    }
}