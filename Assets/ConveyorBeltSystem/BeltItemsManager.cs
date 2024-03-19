using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using System.Linq;
using static BeltItemsManager;
using static UnityEditor.Progress;

public class BeltItemsManager : MonoBehaviour
{
    [SerializeField] SplineContainer[] splineContainers;
    [SerializeField] GameObject defaultItemBoxPrefab;
    [SerializeField] float beltSpeed;

    float3 position;
    float3 tangent;
    float3 upVector;

    List<BeltItem> beltItems;
    int indexMovingBox = 0;

    private float actualLength;

    [Serializable]
    public class BeltItem
    {
        public GameObject ItemBox;
        public float indexSpline;
        public float distanceFrontBox;

        public void SetBox(GameObject itemBox, float indexSpline)
        {
            this.ItemBox = itemBox;
            this.indexSpline = indexSpline;
            distanceFrontBox = 0;
        }
        public void SetBox(GameObject itemBox, float indexSpline, float distanceFrontBox)
        {
            this.ItemBox = itemBox;
            this.indexSpline = indexSpline;
            this.distanceFrontBox = distanceFrontBox;
        }

        public void SetIndex(float index)
        {
            indexSpline = index;
        }
    }

    private void Start()
    {
        beltItems = new List<BeltItem>();
        TimeTickSystem.onTick += UpdateBelt;
    }

    public bool addItem = false;
    public bool quitar = false;
    private void OnValidate()
    {
        if (addItem == true)
        {
            addItem = false;
            AddItemToBelt(0);
        }
        if (quitar == true)
        {
            quitar = false;
            takeItemFromBelt(0);
        }
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (beltItems.Count > 0)
    //    {
    //        if (indexMovingBox > 0) beltItems[indexMovingBox].distanceFrontBox = (beltItems[indexMovingBox - 1].indexSpline * splineContainer.Splines[1].GetLength()) - (beltItems[indexMovingBox].indexSpline * splineContainer.Splines[1].GetLength());
    //        else beltItems[0].distanceFrontBox = splineContainer.Splines[1].GetLength() - (beltItems[indexMovingBox].indexSpline * splineContainer.Splines[1].GetLength()) + 0.25f;
    //        add = beltSpeed / splineContainer.Splines[1].GetLength();

    //        int i = 0;
    //        foreach (var item in beltItems)
    //        {
    //            if (i >= indexMovingBox)
    //            {
    //                if (item.distanceFrontBox < 0.25f && i == indexMovingBox)
    //                {
    //                    indexMovingBox++;
    //                }
    //                else
    //                {
    //                    splineContainer.Evaluate(1, item.indexSpline, out position, out tangent, out upVector);
    //                    Quaternion rotation = Quaternion.LookRotation(tangent, upVector);
    //                    item.ItemBox.transform.position = position;
    //                    item.ItemBox.transform.rotation = rotation;
    //                    item.indexSpline += add * Time.deltaTime;
    //                }
    //            }
    //            i++;
    //        }
    //    }
    //}

    private void UpdateBelt()
    {
        if (beltItems.Count > 0 && indexMovingBox < beltItems.Count)
        {
            if (indexMovingBox > 0) beltItems[indexMovingBox].distanceFrontBox = (beltItems[indexMovingBox - 1].indexSpline * splineContainers[0].Splines[1].GetLength()) - (beltItems[indexMovingBox].indexSpline * splineContainers[0].Splines[1].GetLength());
            else beltItems[0].distanceFrontBox = splineContainers[0].Splines[1].GetLength() - (beltItems[indexMovingBox].indexSpline * splineContainers[0].Splines[1].GetLength()) + 0.25f;

            int i = 0;
            foreach (var item in beltItems)
            {
                if (i >= indexMovingBox)
                {
                    if (item.distanceFrontBox <= 0.25f && i == indexMovingBox)
                    {
                        indexMovingBox++;
                    }
                    else
                    {
                        splineContainers[0].Evaluate(1, item.indexSpline, out position, out tangent, out upVector);
                        Quaternion rotation = Quaternion.LookRotation(tangent, upVector);
                        item.ItemBox.transform.position = position;
                        item.ItemBox.transform.rotation = rotation;
                        item.indexSpline += beltSpeed/100/ splineContainers[0].Splines[1].GetLength();
                    }
                }
                i++;
            }
            Debug.Log(indexMovingBox);
        }
    }

    void AddItemToBelt(float pos)
    {
        splineContainers[0].Evaluate(1, pos, out position, out tangent, out upVector);

        Quaternion rotation = Quaternion.LookRotation(tangent, upVector);

        BeltItem item = new BeltItem();
        GameObject obj = Instantiate(defaultItemBoxPrefab, position, rotation);
        obj.transform.parent = this.transform;

        float dist = 1;
        if (beltItems.Count >= 1) dist = beltItems.Last().indexSpline * splineContainers[0].Splines[1].GetLength();
        Debug.Log(dist);
        item.SetBox(obj, pos, dist);

        beltItems.Add(item);
    }

    public void takeItemFromBelt(int i) {
        Destroy(beltItems[i].ItemBox);
        beltItems.RemoveAt(i);
        indexMovingBox = i;
    }

    public void UpdateLength()
    {
        foreach (var item in beltItems)
        {
            item.indexSpline = (actualLength / splineContainers[0].Splines[1].GetLength()) * item.indexSpline;
        }
        indexMovingBox = 0;
        actualLength = splineContainers[0].Splines[1].GetLength();
    }
}
