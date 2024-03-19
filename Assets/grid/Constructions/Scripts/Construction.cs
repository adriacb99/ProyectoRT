using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Construction : MonoBehaviour
{
    public static Construction PlaceConstruction(Transform parent, ConstructionTypeSO obj, Vector3 position, List<GameObject> sideObjects, Quaternion q)
    {
        Transform constructionTransform = Instantiate(obj.constructionPrefab, position, q);

        Construction construction = constructionTransform.GetComponent<Construction>();

        construction.transform.parent = parent;
        construction.constructionType = obj;

        return construction;
    }

    private ConstructionTypeSO constructionType;
}
