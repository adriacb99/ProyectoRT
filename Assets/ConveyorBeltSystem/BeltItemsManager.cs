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
    [SerializeField] SplineContainer splineContainer;
    [SerializeField] GameObject defaultItemBoxPrefab;
    [SerializeField] float beltSpeed;

    float3 position;
    float3 tangent;
    float3 upVector;

    List<BeltItem> beltItems;
    int indexMovingBox = 0;

    private float add;
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
    }

    public bool añadir = false;
    private void OnValidate()
    {
        if (añadir == true)
        {
            añadir = false;
            AddItemToBelt(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (beltItems.Count > 0)
        {
            if (indexMovingBox > 0) beltItems[indexMovingBox].distanceFrontBox = (beltItems[indexMovingBox - 1].indexSpline * splineContainer.Splines[1].GetLength()) - (beltItems[indexMovingBox].indexSpline * splineContainer.Splines[1].GetLength());
            else beltItems[0].distanceFrontBox = splineContainer.Splines[1].GetLength() - (beltItems[indexMovingBox].indexSpline * splineContainer.Splines[1].GetLength()) + 0.25f;
            add = beltSpeed / splineContainer.Splines[1].GetLength();

            int i = 0;
            foreach (var item in beltItems)
            {
                if (i >= indexMovingBox)
                {
                    splineContainer.Evaluate(1, item.indexSpline, out position, out tangent, out upVector);
                    Quaternion rotation = Quaternion.LookRotation(tangent, upVector);
                    item.ItemBox.transform.position = position;
                    item.ItemBox.transform.rotation = rotation;
                    item.indexSpline += add * Time.deltaTime;

                    if (item.distanceFrontBox < 0.25f && i == indexMovingBox) indexMovingBox++;
                }
                i++;
            }
        }
    }

    void AddItemToBelt(float pos)
    {
        splineContainer.Evaluate(1, pos, out position, out tangent, out upVector);

        Quaternion rotation = Quaternion.LookRotation(tangent, upVector);

        BeltItem item = new BeltItem();
        GameObject obj = Instantiate(defaultItemBoxPrefab, position, rotation);

        float dist = 1;
        if (beltItems.Count >= 1) dist = beltItems.Last().indexSpline * splineContainer.Splines[1].GetLength();
        Debug.Log(dist);
        item.SetBox(obj, pos, dist);

        beltItems.Add(item);
    }

    public void UpdateLength()
    {
        foreach (var item in beltItems)
        {
            item.indexSpline = (actualLength / splineContainer.Splines[1].GetLength()) * item.indexSpline;
        }
        indexMovingBox = 0;
        actualLength = splineContainer.Splines[1].GetLength();
    }
}
