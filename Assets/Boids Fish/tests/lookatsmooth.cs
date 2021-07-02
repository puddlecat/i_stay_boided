using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lookatsmooth : MonoBehaviour
{
    public GameObject lookee;
    [Range(0, 2)]
    public float damping = 1.0f;
    void Start()
    {
        lookee = GameObject.Find("lookee");
    }

    void Update()
    {
        var relativePos = Quaternion.LookRotation(lookee.transform.position - transform.position);//from docs
        this.gameObject.transform.rotation = Quaternion.Slerp(transform.rotation, relativePos, 1/damping * Time.deltaTime);
    }
}
