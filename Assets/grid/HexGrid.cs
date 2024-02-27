using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class HexGrid<HGridObject>
{
    private HGridObject[] gridArray;
    int[] trisIndex;
    List<int>[] Grid;
    private int size;

    public HexGrid(int size, Func<HexGrid<HGridObject>, Vector3, List<int>, int, HGridObject> createTile, Vector3[] midFacePoints, int[] trisIndex, List<int>[] Grid, int[] tileTypeArray)
    {
        this.size = size;
        this.trisIndex = trisIndex;
        this.Grid = Grid;

        gridArray = new HGridObject[size];

        for (int i = 0; i < gridArray.Length; i++) {
            gridArray[i] = createTile(this, midFacePoints[i], Grid[i], tileTypeArray[i]);
        }
    }

    public HGridObject GetValue(int triIndex) {       
        return gridArray[trisIndex[triIndex]];
    }

    public void SetValue(int triIndex, HGridObject value)
    {
        gridArray[trisIndex[triIndex]] = value;
    }
}
