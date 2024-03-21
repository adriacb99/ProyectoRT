using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class Construction : MonoBehaviour
{
    public static Construction PlaceConstruction(Planet.Tile tile, Transform parent, ConstructionData obj, Vector3 position, List<Construction> sideObjects, Quaternion q)
    {
        Transform constructionTransform = Instantiate(obj.constructionPrefab, position, q);

        Construction construction = constructionTransform.GetComponent<Construction>();

        construction.transform.parent = parent;
        construction.constructionType = obj;
        construction.tile = tile;

        return construction;
    }

    private ConstructionData constructionType;
    public Planet.Tile tile;

    public abstract ItemData GetItemFromConstruction();
}
