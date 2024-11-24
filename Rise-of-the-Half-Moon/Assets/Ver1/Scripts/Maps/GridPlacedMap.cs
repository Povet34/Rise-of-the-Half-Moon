using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPlacedMap : MonoBehaviour
{
    private System.Random random;
    private RectTransform boundingBox;
    private GameObject nodePrefab;

    public void CreateMap(int seed, GameObject nodePrefab, RectTransform boundingBox)
    {
        random = new System.Random(seed);
        this.nodePrefab = nodePrefab;
        this.boundingBox = boundingBox;
    }
}
