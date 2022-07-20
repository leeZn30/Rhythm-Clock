using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundOperation : MonoBehaviour
{
    public float ScrollSpeed = 0.5f;
    float Offset;

    // Start is called before the first frame update
    void Start()
    {
    }

    void Update()
    {

    }

    public void backgroundScroll()
    {

        Offset += Time.deltaTime * ScrollSpeed;
        GetComponent<Renderer>().material.mainTextureOffset = new Vector2(Offset, 0);


    }

}
