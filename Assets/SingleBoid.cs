using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleBoid : MonoBehaviour
{
    private List<GameObject> neighbors = new List<GameObject>();
    private List<GameObject> allOthers = new List<GameObject>();
    public float perceptionDistance = 40;
    public float perceptionWidth = 0.5f;
    public float speedFactor = 1.0f;
    [SerializeField] Animator mainAnimator;
    public float cohesionSpeed =0.5f;
    public float alignmentFactor = 1.0f;
    public float avoidanceSpeed = 0.5f;
    public float avoidanceRadius = 7.0f;
    public bool isSpecial = false; //for debugging
    Vector3 myPos;
    public float awarenessDistance = 60.0f; //for now
    public float currentVelocity;
    public float currentAngle;
    public bool isLeader = false;
    public float followingFactor = 0.0f;
    public bool isFish = false;
    //public float speedTest = 1.0f;
   // public GameObject leader;

   // List<GameObject> findNeighbors(){   
    //}

    bool canSee(Vector3 pos){

        //i can't see anything behind me..
        //but if i'm close enough to actually be touching the other guy i should always be aware of it
        float zdistance = Mathf.Abs(pos.z -myPos.z);
        float xdistance = Mathf.Abs(pos.x - myPos.x);

        if((zdistance < 2) & (xdistance < 2)){
            return true;
        }

        else if(pos.z < myPos.z){
            return false;
        }
        else{
            //can only see a certain distance ahead
            if (zdistance < perceptionDistance){
                //can see farther to the sides depending on how far away it is
                if(xdistance<(perceptionWidth/(1.0f/(zdistance+0.2f)))){
                    return true;
                }
            }
            return false;
        }
    }

    Vector3 cohesionPosition(){
        Vector3 avgPosition = new Vector3(0.0f,0.0f,0.0f);
        for(int i=0; i<allOthers.Count; i++){
                avgPosition.x += allOthers[i].gameObject.transform.position.x;
                avgPosition.y += allOthers[i].gameObject.transform.position.y;
                avgPosition.z += allOthers[i].gameObject.transform.position.z;
        }
        avgPosition.x = avgPosition.x / allOthers.Count;
        avgPosition.y = avgPosition.y / allOthers.Count;
        avgPosition.z = avgPosition.z / allOthers.Count;
        return avgPosition;
    }

    void Start()
    {   
        //find the other boids
        myPos = transform.position;
        var allOtherss = GameObject.FindGameObjectsWithTag("fish");
        for(int i=0; i<allOtherss.Length; i++){
            if (allOtherss[i].gameObject != this.gameObject){
            allOthers.Add(allOtherss[i]);
            }
        }
        //leader = GameObject.Find("leader");
        //set initial velocity and rotation
        currentAngle = Random.Range(0.0f, 90f);
        this.gameObject.transform.Rotate(currentAngle, currentAngle, 0.0f, Space.Self);
        currentVelocity = Random.Range(0.0f, 5.0f);
        //currently not being used beyond the first frame
        //to deal with the onesthat are inside each other since i cant find them.. bit stupid
        this.gameObject.transform.Translate(Random.Range(0.0f, 1.0f),Random.Range(0.0f, 1.0f),Random.Range(0.0f, 1.0f));
    }
    
    void Update()
    {
        //get neighbors
        //the name "neighbors" is a bit misleading because it really means the ones it can See
        myPos = transform.position;
        neighbors.Clear();
        /*for (int i=0; i<allOthers.Count; i++){
            if(canSee(allOthers[i].gameObject.transform.position)){
                if(isSpecial){
                allOthers[i].gameObject.GetComponent<Renderer>().material.SetColor("_Color",Color.green);
                }
                neighbors.Add(allOthers[i].gameObject);
            }
            else{
                if(isSpecial){
                allOthers[i].gameObject.GetComponent<Renderer>().material.SetColor("_Color",Color.red);
                }
            }
        }*/
        //try something difference
        for(int i=0; i<allOthers.Count; i++){
            if(Vector3.Distance(allOthers[i].gameObject.transform.position, myPos) < awarenessDistance){
                neighbors.Add(allOthers[i].gameObject);
                //allOthers[i].gameObject.GetComponent<Renderer>().material.SetColor("_Color",Color.green);
            }
           /* else{
                allOthers[i].gameObject.GetComponent<Renderer>().material.SetColor("_Color",Color.red);
            }*/
        }
        //determine where the center of the flock is, and how far away i am from it
        Vector3 goal = cohesionPosition();
        //Vector3 actualGoal = new Vector3(goal.x, 0.0f, goal.y);
        float distanceToGoal = Vector3.Distance(goal, myPos);

        //todo: change the speed with which it turns towards goal
        this.gameObject.transform.LookAt(goal);

        //currentVelocity += (distanceToGoal/2);
        currentVelocity = (Time.deltaTime * (distanceToGoal/2))*cohesionSpeed;
        
        //for each neighbor within avoidance zone,add a turn away depending on how close they are
        //unfortunately it's going inside of ones it cant see. so for now un-implementing the sight thing in favor of a simple radius of awareness
        //also here is where we calculate alignment
       float neighborsAlignmentY = 0.0f;
       float neighborsAlignmentX = 0.0f;
       float neighborsSpeed = 0.0f;
       //todo: fix it when there's no neighbors
        /*
        for(int i=0; i<neighbors.Count; i++){
            neighborsSpeed += neighbors[i].gameObject.GetComponent<SingleBoid>().currentVelocity;
            neighborsAlignmentY += neighbors[i].gameObject.transform.rotation.y;
            neighborsAlignmentX += neighbors[i].gameObject.transform.rotation.x;
            float distanceToNeighborZ = Mathf.Abs(neighbors[i].gameObject.transform.position.z - myPos.z);
            if(distanceToNeighborZ < avoidanceRadius){
               float amountToTurnY = (180.0f/(distanceToNeighborZ-2));
               this.gameObject.transform.Rotate(0.0f, amountToTurnY, 0.0f, Space.Self);
             currentVelocity += (1/distanceToNeighborZ)*avoidanceSpeed;
           }
        }*/
        neighborsSpeed = neighborsSpeed/neighbors.Count;
        neighborsAlignmentY = neighborsAlignmentY/neighbors.Count;
        neighborsAlignmentX = neighborsAlignmentX/neighbors.Count;

        //for now, i'll just average my own speed and angle with that of neighbors
        currentVelocity = ((currentVelocity*(1-alignmentFactor))+(neighborsSpeed*alignmentFactor));
        //todo
        this.gameObject.transform.Rotate(0.0f,(((neighborsAlignmentY*alignmentFactor)+(this.gameObject.transform.rotation.z * (1-alignmentFactor)))),0.0f, Space.Self);

        //follow the leader..
       /*if(!isLeader){
            
        }*/

        //aply the s0eed factor
        currentVelocity = currentVelocity*speedFactor;
        //finally, actually move forward
        this.gameObject.transform.Translate(Vector3.forward * currentVelocity);
       /* if(isFish){
            mainAnimator.SetFloat("speed", speedTest);
        }*/
    }
}
