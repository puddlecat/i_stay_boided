using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class avoidNeighborCOllider : MonoBehaviour
{
    private List<Collider> allFishColliders = new List<Collider>();
    private List<GameObject> allOtherFish = new List<GameObject>();
    public float avoidanceFishDistance = 3.0f;
    private GameObject closestFish;
    private float closestFishDist;

    //well first i implemented it without colliders so lets see what happens./
    //and then i kept it like that. the end

    void Start()
    {
        var allOtherss = GameObject.FindGameObjectsWithTag("fish");
        for(int i=0; i<allOtherss.Length; i++){
            if (allOtherss[i].gameObject != gameObject){
                allOtherFish.Add(allOtherss[i].gameObject);
                allFishColliders.Add(allOtherss[i].gameObject.GetComponent<Collider>());
            }
        }
    }

    void Update()
    {
        closestFishDist = Mathf.Infinity;
        closestFish= null;
        for(int i=0; i<allOtherFish.Count; i++){
            float dist = Vector3.Distance(allOtherFish[i].transform.position, transform.position);
            if(dist <= avoidanceFishDistance){
                if(dist < closestFishDist){
                    closestFishDist = dist;
                    closestFish = allOtherFish[i];
                }
            }
        }
        if(closestFish != null){
            Quaternion rot = Quaternion.LookRotation(transform.position - closestFish.transform.position); 
            //for test - seperating rotations
            float xRotation = rot.eulerAngles.x;
            float yRotation = rot.eulerAngles.y;
            float zRotation = rot.eulerAngles.z;
            rot = Quaternion.Euler(xRotation, yRotation, zRotation);

            transform.rotation= Quaternion.Slerp(transform.rotation, rot, ((closestFishDist) * Time.deltaTime));
        } 
    }
}
