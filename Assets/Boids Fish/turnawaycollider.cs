using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class turnawaycollider : MonoBehaviour
{
    //i'm going to try using closestpoint first
    public List<Collider> allObstacles = new List<Collider>();
    public float avoidanceDistance = 5.0f;
    public float speed = 3.0f;
    public float damping = 0.2f;
    public float cornerRadius = 0.3f;
    public GameObject debugPrefab;
     public GameObject debugPrefab2;

    void Start() 
    {
        var allOtherss = GameObject.FindGameObjectsWithTag("obstacle");
        for(int i=0; i<allOtherss.Length; i++){
            if (allOtherss[i].gameObject != gameObject){
                allObstacles.Add(allOtherss[i].gameObject.GetComponent<Collider>());
            }
        }
        transform.Rotate(0.0f, Random.Range(0.0f, 90f), Random.Range(-30.0f, 0.0f), Space.Self);
    }

    void Update()
    {   
        float closestDist = Mathf.Infinity; //should these not be redeclared?
        float secondClosestDist = Mathf.Infinity+1;
        Vector3 closestPoint = new Vector3(0.0f, 0.0f, 0.0f);
        Vector3 secondClosestPoint = new Vector3(0.0f, 0.0f, 0.0f);
        //find the closest point on an obstacle collider
        for(int i=0; i<allObstacles.Count;i++){
            float disty = Vector3.Distance(transform.position, allObstacles[i].ClosestPoint(transform.position));
            /*if((Mathf.Abs(disty-closestDist)<=cornerRadius) &(disty<=avoidanceDistance) & (closestDist<=avoidanceDistance)){
                //stupid way of detecting corners
                //turn away from the midpoint
                closestDist = Mathf.Min(disty,closestDist);
                closestPoint = (allObstacles[i].ClosestPoint(transform.position) + closestPoint) * 0.5f;
                Debug.Break();
                gameObject.transform.GetChild(1).GetComponent<Renderer>().material.SetColor("_Color",Color.red);
                Instantiate(debugPrefab, closestPoint, Quaternion.identity);
                break;
            }*/
            if(disty < secondClosestDist){
                if(disty > closestDist){
                    secondClosestDist = disty;
                    secondClosestPoint = allObstacles[i].ClosestPoint(transform.position);
                }
                else{
                    secondClosestDist = closestDist;
                    secondClosestPoint = closestPoint;
                    closestDist = disty;
                    closestPoint = allObstacles[i].ClosestPoint(transform.position);

                }
            }
        }
        //add a turn away from obstacle depending on how close it is
        //now, how to combine this with swimming towards a goal?
        /*if(Mathf.Abs(closestDist - secondClosestDist) <= cornerRadius){
            //turn away from the midpoint
            closestPoint = Vector3.Lerp(closestPoint, secondClosestPoint, 0.5f); 
            //i think there's actually just something wrong with this ^
            closestDist = Vector3.Distance(transform.position, closestPoint);
            gameObject.transform.GetChild(1).GetComponent<Renderer>().material.SetColor("_Color",Color.red);
        }
        else{
            gameObject.transform.GetChild(1).GetComponent<Renderer>().material.SetColor("_Color",Color.green);
        }*/

        if(closestDist <= avoidanceDistance){
            if(closestDist < 0.2){
                //DEBUG SECTION
                if(Mathf.Abs(closestDist - secondClosestDist) <= cornerRadius){
                    print("corner distance: "+Mathf.Abs(closestDist - secondClosestDist));
                    Instantiate(debugPrefab, closestPoint, Quaternion.identity);
                    Instantiate(debugPrefab, secondClosestPoint, Quaternion.identity);
                    closestPoint = Vector3.Lerp(closestPoint, secondClosestPoint, 0.5f); 
                    closestDist = Vector3.Distance(transform.position, closestPoint);
                    gameObject.transform.GetChild(1).GetComponent<Renderer>().material.SetColor("_Color",Color.red);
                    Instantiate(debugPrefab2, closestPoint, Quaternion.identity);
                }
                else{
                    print("no corner detected, corner distance: "+Mathf.Abs(closestDist - secondClosestDist));
                    Instantiate(debugPrefab, closestPoint, Quaternion.identity);
                    Instantiate(debugPrefab, secondClosestPoint, Quaternion.identity);
                }
                Debug.Break();
                Selection.activeGameObject = this.gameObject;
            }//END DEBUG SECTION
            else if(Mathf.Abs(closestDist - secondClosestDist) <= cornerRadius){
                closestPoint = Vector3.Lerp(closestPoint, secondClosestPoint, 0.5f); 
                closestDist = Vector3.Distance(transform.position, closestPoint);
            }
            Quaternion rot = Quaternion.LookRotation(transform.position - closestPoint); 

            float xRotation = rot.eulerAngles.x;
            float yRotation = rot.eulerAngles.y;
            float zRotation = rot.eulerAngles.z;
            rot = Quaternion.Euler(xRotation, yRotation, zRotation);

            transform.rotation= Quaternion.Slerp(transform.rotation, rot, ((closestDist*damping) * Time.deltaTime)); 
        }
        transform.Translate(Vector3.forward* (Time.deltaTime*speed));
    }
}
