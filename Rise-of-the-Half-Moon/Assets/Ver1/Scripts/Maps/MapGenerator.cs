using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum PlaceType { NonGrid, Grid };
    public PlaceType placeType = PlaceType.Grid;

    public RectTransform boundingBox; // 직사각형 배치 영역
    public GameObject nodePrefab;     // 노드 프리팹

    public int seed;

    private void Start()
    {
        if (placeType.Equals(PlaceType.NonGrid)) 
        {
            NonGridPlacedMap map = gameObject.AddComponent<NonGridPlacedMap>();
            map.CreateMap(seed, nodePrefab, boundingBox);
        }
        else if(placeType.Equals(PlaceType.Grid))
        {
            GridPlacedMap map = gameObject.AddComponent<GridPlacedMap>();
            map.CreateMap(seed, nodePrefab, boundingBox);
        }
    }
}
