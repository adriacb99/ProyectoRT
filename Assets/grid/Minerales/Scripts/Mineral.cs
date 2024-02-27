using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mineral : MonoBehaviour
{
    [SerializeField] int quantity = 1;

    public void GetMineral() {
        quantity--;
        if (quantity <= 0) Destroy(gameObject);
    }
}
