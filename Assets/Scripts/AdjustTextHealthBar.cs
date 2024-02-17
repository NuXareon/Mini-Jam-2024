using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustTextHealthBar : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<TextMesh>().GetComponent<MeshRenderer>().sortingOrder = 150;
    }
}
