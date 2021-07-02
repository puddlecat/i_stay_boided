using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSee : MonoBehaviour
{
    SingleBoid parentScript;
    void Start()
    {
        parentScript = gameObject.transform.parent.gameObject.GetComponent<SingleBoid>();
    }

    void OnTriggerEnter(Collider other)
    {
        print("e");
    }
}
