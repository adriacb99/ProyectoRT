using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;

public class BeltItemsManager : MonoBehaviour
{
    [SerializeField] SplineContainer splineContainer;
    [SerializeField] GameObject defaultItemBoxPrefab;
    [SerializeField] float beltSpeed;

    [SerializeField]
    [Range(0f, 1f)]
    private float time;

    float3 position;
    float3 tangent;
    float3 upVector;

    List<BeltItem> beltItems;

    private float add;

    [Serializable]
    public class BeltItem
    {
        public GameObject ItemBox;
        public float indexSpline;

        public void SetBox(GameObject itemBox, float indexSpline)
        {
            this.ItemBox = itemBox;
            this.indexSpline = indexSpline;
        }
            
        public void SetIndex(float index)
        {
            indexSpline = index;
        }
    }

    private void Awake()
    {
        beltItems = new List<BeltItem>();
    }

    public bool añadir = false;
    private void OnValidate()
    {
        añadir = true;
    }
    private void LateUpdate()
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
        add = beltSpeed / splineContainer.Splines[1].GetLength();
        //splineContainer.Evaluate(1, time, out position, out tangent, out upVector);
        foreach (var item in beltItems)
        {
            splineContainer.Evaluate(1, item.indexSpline, out position, out tangent, out upVector);
            Quaternion rotation = Quaternion.LookRotation(tangent, upVector);
            item.ItemBox.transform.position = position;
            item.ItemBox.transform.rotation = rotation;
            item.indexSpline += add;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(position+upVector/7, 0.075f);
    }

    void AddItemToBelt(float pos)
    {
        splineContainer.Evaluate(1, pos, out position, out tangent, out upVector);

        Quaternion rotation = Quaternion.LookRotation(tangent, upVector);

        BeltItem item = new BeltItem();
        GameObject obj = Instantiate(defaultItemBoxPrefab, position, rotation);
        item.SetBox(obj, pos);

        beltItems.Add(item);
    }
}
