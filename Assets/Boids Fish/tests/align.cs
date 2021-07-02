using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class align : MonoBehaviour
{
    public List<GameObject> allOthers = new List<GameObject>();
    public float awarenessDistance = 30.0f;
    public float currentSpeed = 0.0f;
    public bool isSpecial = false;

    //neighbors are the objects it's aware of
    List<GameObject> getNeighbors (){
        List<GameObject> neighbors = new List<GameObject>();
        for(int i=0; i<allOthers.Count; i++){
            if(Vector3.Distance(allOthers[i].gameObject.transform.position, transform.position) < awarenessDistance){
                neighbors.Add(allOthers[i].gameObject);
            }
        }
        return neighbors;
    }

    (Vector3, float) getAverageAlignmentnSpeed(List<GameObject> neighbs){
        float speed = currentSpeed;
        float avgY = 0.0f;
        float avgX = 0.0f;
        float avgZ = 0.0f;
        if(neighbs.Count > 0){
            for(int i=0; i<neighbs.Count; i++){
                speed += neighbs[i].gameObject.GetComponent<align>().currentSpeed;
                avgY += neighbs[i].gameObject.transform.rotation.y;
                avgX += neighbs[i].gameObject.transform.rotation.x;
                avgZ += neighbs[i].gameObject.transform.rotation.z;
            }
        }
        speed = speed/neighbs.Count;
        avgY = avgY/neighbs.Count;
        avgX = avgX/neighbs.Count;
        avgZ = avgZ /neighbs.Count;
        return (new Vector3(avgX, avgY, avgZ), speed);
    }

    void Start()
    {   
        var allOtherss = GameObject.FindGameObjectsWithTag("fish");
        for(int i=0; i<allOtherss.Length; i++){
            if (allOtherss[i].gameObject != this.gameObject){
                allOthers.Add(allOtherss[i]);
            }
        }
    }

    void Update()
    {
        var neighbs = getNeighbors();
        var alignmentS = getAverageAlignmentnSpeed(neighbs);
        this.gameObject.transform.Rotate(alignmentS.Item1, Space.Self);
        //yeah see the problem here is that they just start spinning
       // if(isSpecial){print(alignmentS.Item2);}
    }
}
