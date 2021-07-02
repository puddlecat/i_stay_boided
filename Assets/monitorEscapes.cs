using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monitorEscapes : MonoBehaviour
{
    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag == "fish"){
            print("FISH ON THE LOOSE");
        }
    }
}
