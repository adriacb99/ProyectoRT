using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour
{
    [SerializeField] Canvas canvas;

    public void DragHandler(BaseEventData data) {
        PointerEventData pointerData = (PointerEventData)data;
        Debug.Log(pointerData.position);
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform,
            pointerData.position,
            canvas.worldCamera,
            out pos);
        transform.position = canvas.transform.TransformPoint(pos);
    }
}
