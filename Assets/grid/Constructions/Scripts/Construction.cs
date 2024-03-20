using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Construction : MonoBehaviour
{
    public static Construction PlaceConstruction(Transform parent, ConstructionData obj, Vector3 position, List<Construction> sideObjects, Quaternion q)
    {
        Transform constructionTransform = Instantiate(obj.constructionPrefab, position, q);

        Construction construction = constructionTransform.GetComponent<Construction>();

        construction.transform.parent = parent;
        construction.constructionType = obj;

        return construction;
    }

    private ConstructionData constructionType;

    public abstract ItemData GetItemFromConstruction();
}
