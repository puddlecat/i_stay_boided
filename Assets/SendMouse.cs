using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendMouse : MonoBehaviour
{
    
    Material mat;
    Vector3 worldPosition;
    Ray ray;
    float xscale;
    float yscale;
    float zscale;
    Collider coll;

    void Start(){
        mat = gameObject.GetComponent<MeshRenderer>().material;
        coll = gameObject.GetComponent<Collider>();
        xscale = gameObject.GetComponent<Transform>().localScale.x;
        yscale = gameObject.GetComponent<Transform>().localScale.y;
        zscale = gameObject.GetComponent<Transform>().localScale.z;
    }

    void Update(){
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitData;
            if(coll.Raycast(ray, out hitData, 1000))
            {
                worldPosition = hitData.point;
                //float dividedy = (((worldPosition.y/4)/yscale)+2)/2;
                float dividedx = (((worldPosition.x/4)/xscale)+1)/2;
                float dividedz = ((worldPosition.z/12)/zscale)+0.5f;
                //Debug.Log(dividedz);
                mat.SetFloat("_Mouseposx", dividedx);
                //mat.SetFloat("_Mouseposy", dividedy);
                mat.SetFloat("_Mouseposz", dividedz);
            }
    }
}
