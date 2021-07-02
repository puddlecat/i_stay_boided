using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//move towards a target while avoiding obstancles
public class avoid : MonoBehaviour
{
    public GameObject target;
    public List<GameObject> allObstacles = new List<GameObject>();
    public float damping =1.0f;
    public float damping2 = 0.05f;
    public float awarenessDistance = 10.0f;
    public float avoidanceDistance = 8.0f;
    [SerializeField] Animator mainAnimator;
    public float avoidSpeed = 0.5f;
    public bool isFish = false;
    void Start()
    {
        target = GameObject.Find("target");
        var allOtherss = GameObject.FindGameObjectsWithTag("obstacle");
        for(int i=0; i<allOtherss.Length; i++){
            if (allOtherss[i].gameObject != this.gameObject){
                allObstacles.Add(allOtherss[i]);
            }
        }
    }

    //neighbors are the objects it's aware of
    List<GameObject> getNeighbors (){
        List<GameObject> neighbors = new List<GameObject>();
        for(int i=0; i<allObstacles.Count; i++){
            if(Vector3.Distance(allObstacles[i].gameObject.transform.position, transform.position) < awarenessDistance){
                neighbors.Add(allObstacles[i].gameObject);
            }
        }
        return neighbors;
    }

    /*List<GameObject> getObstacles (List<GameObject> neighbors){
        List <GameObject> obstacles = new List<GameObject>();
        for(int i=0; i<neighbors.Count; i++){
            if(Vector3.Distance(neighbors[i].gameObject.transform.position, transform.position) < avoidanceDistance){
                obstacles.Add(neighbors[i].gameObject);
            }
        }
        return obstacles;
    }*/

    (Transform, float) getClosestObstacle(List<GameObject> neighbors){
        float closestDistance = Mathf.Infinity;
        Transform closestTransform = transform;
        for(int i =0; i<neighbors.Count; i++){
            float dist = Mathf.Abs((neighbors[i].transform.position.x-(neighbors[i].transform.localScale.x/2))- transform.position.x);
            if(dist < avoidanceDistance){
                //wait, i only need to avoid obstacles in front of me
                //cite
                var heading = neighbors[i].transform.position - transform.position;
                var dot = Vector3.Dot(heading, transform.forward);

                if(dot > 0){
                    if(dist < closestDistance){
                        closestDistance = dist;
                        closestTransform = neighbors[i].transform;
                    }
                }
                //the thing is that it should be in reference to a collider, and not the scale of the object
                //also, i don't need to avoid an obstacle that i'm below or above
                //the simpler option is to just use colliders, but i don't know if that will be slow
            }
        }
        return (closestTransform, closestDistance);
    }

/*    //so, the other option is to just not let it near more than one obstacle at at ime
    (Vector3, float) avoidObstacles(List<GameObject> obstacles){
        if(obstacles.Count == 0){
            //todo
            return(new Vector3(0,0,0), 0);
        }
        Vector3 positionToAvoid = obstacles[0].gameObject.transform.position;
        float speed = currentSpeed;
        
        if(obstacles.Count > 1){
            for (int i=1; i<obstacles.Count; i++){
                //add it to position depending on how close it is
                positionToAvoid += obstacles[i].gameObject.transform.position*(1/(Vector3.Distance(obstacles[i].gameObject.transform.position, transform.position)));
                //positionToAvoid.x += obstacles[i].gameObject.transform.position.x*(1/(Mathf.Abs(obstacles[i].gameObject.transform.position.x-transform.position.x)));
                //positionToAvoid.y += obstacles[i].gameObject.transform.position.y*(1/(Mathf.Abs(obstacles[i].gameObject.transform.position.y-transform.position.y)));
                //positionToAvoid.z += obstacles[i].gameObject.transform.position.z*(1/(Mathf.Abs(obstacles[i].gameObject.transform.position.z-transform.position.z)));
            }
            positionToAvoid = positionToAvoid / obstacles.Count;
        }
        return (positionToAvoid, speed);
    }*/

    void Update()
    {
        float distanceToGoal = Vector3.Distance(target.transform.position, transform.position);
        float vel  = Time.deltaTime * (distanceToGoal/2);
        var relativePos = Quaternion.LookRotation(target.transform.position - transform.position);//from docs
        transform.rotation = Quaternion.Slerp(transform.rotation, relativePos, 1/damping * Time.deltaTime);

        var neighbors = getNeighbors();
        var obst = getClosestObstacle(neighbors);

        if(obst.Item2 != Mathf.Infinity){
            print("near obstacle");
            float dir = avoidanceDistance + obst.Item1.localScale.z;
            if((obst.Item1.position.z+(obst.Item1.localScale.z/2))>transform.position.z){
                //object is on my right
                dir = 1.0f-avoidanceDistance;
            }
            Vector3 awayDirection = new Vector3(obst.Item1.position.x, obst.Item1.position.y, obst.Item1.position.z+dir);
            var awayRotation= Quaternion.LookRotation(awayDirection);
            //print(awayRotation);
            //the problem here is that it needs to turn to the left if object is on the right

            transform.rotation = Quaternion.Slerp(transform.rotation, awayRotation, (1/damping2) * Time.deltaTime);
           // transform.Rotate(0.0f, (90.0f/(obst-2)), 0.0f, Space.Self); //the -2 is so we dont risk going insideit
            vel += (1/obst.Item2)*avoidSpeed;
        }

        //var average = Quaternion.Lerp(awayRotation, goalRotation, 0.5f); 

        //add this depending on how close it is
        //transform.rotation = Quaternion.Slerp(transform.rotation, average, Time.deltaTime * vel);
       transform.Translate(Vector3.forward * vel);
       if(isFish){
            mainAnimator.SetFloat("speed", (vel*10)+0.2f);
        }
    }
}
