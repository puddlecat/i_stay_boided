using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boidsfishnew : MonoBehaviour
{
    //todo: change these to private 
    private List<GameObject> allOthers = new List<GameObject>();
    public float awarenessRadius = 20.0f;
   //actually, i'm going to assume that they're all aware of each other at all times for now, to prevent stragglers from going off into nowhere
    public float currentVelocity = 0.0f;
    private Vector3 averageHeading = new Vector3(0.0f, 0.0f, 0.0f);//average rotation of all the other fish
    private float averageVelocity = 0.0f;
    private Vector3 centerOfMass = new Vector3(0.0f, 0.0f, 0.0f); //average location of all the other fish
    public float insideRadius = 0.8f; //radius in which fish are literally inside of each other. we dont want this ever
    public bool isSpecial = false; //for debugging, so that they're not just all printing at once  
    public float avoidRadius = 5.0f; //give my neighbors some space
    public float damping = 0.2f; //make animation smoother
    public float alignmentFactor = 0.5f; //how much should i turn towards the average heading of my flockmates
    private List<GameObject> allObstacles = new List<GameObject>();
    public float speedFactor = 1.0f;
    public float avoidanceFactor = 0.0f;
    public float attractionFactor = 1.0f;
    public float obstacleAvoidRadius = 7.0f;
    public float obstacleInsideRadius = 2.0f;
    [SerializeField] Animator mainAnimator;

    void Start()
    {   
        //get all other fish
        var allOtherss = GameObject.FindGameObjectsWithTag("fish");
       /* for(int i=0; i<allOtherss.Length; i++){
            if (allOtherss[i].gameObject != gameObject){
            allOthers.Add(allOtherss[i]);
            }
        }*/

         allOtherss = GameObject.FindGameObjectsWithTag("obstacle");
        for(int i=0; i<allOtherss.Length; i++){
            if (allOtherss[i].gameObject != gameObject){
            allObstacles.Add(allOtherss[i]);
            }
        }
        //add a random starting alignment and velocity
        //todo- uncomment
        transform.Rotate(0.0f, Random.Range(0.0f, 90f), 0.0f, Space.Self);
        currentVelocity = Random.Range(0.1f, 0.7f);
    }

    void Update()
    {
        averageHeading = new Vector3(0.0f,0.0f,0.0f);
       // centerOfMass = new Vector3(0.0f, 0.0f, 0.0f);
        averageVelocity = 0.0f;
        float avoidanceAngle = 0.0f;
        float avoidanceSpeed = 0.0f;
        int neighbors = 0;
     /*   //check which of the other fishes i am aware of, and check on them
        for(int i=0; i<allOthers.Count; i++){
            //we never want to be able to go inside another fish
            float dist = Mathf.Abs(Vector3.Distance(allOthers[i].transform.position, transform.position));
            if(dist <= insideRadius){
                transform.Translate(dist, 0.0f, 0.0f);
                //todo: maybe distance times 2?
            }
            else if(dist < avoidRadius){
                avoidanceAngle = 90.0f/(dist-2.0f);
                //if object is to the left of me, turn right
                //cite
                var heading = allOthers[i].transform.position - transform.position;
                var dot = Vector3.Dot(heading, transform.right);

                //todo- randomly decide whether to turn up/down or to the side
                if(dot > 0){
                    //its to the right, so i wanna turn left
                    avoidanceAngle = 1-avoidanceAngle;
                }
                avoidanceSpeed = 1.0f/dist;
               //if(isSpecial){
                    //print(avoidanceSpeed);
                //}
            }
            if(dist <= awarenessRadius){
                neighbors += 1;
                averageHeading.x += allOthers[i].transform.eulerAngles.x;
                averageHeading.y += allOthers[i].transform.eulerAngles.y;
                averageHeading.z += allOthers[i].transform.eulerAngles.z;
                //todo: getcomponent apparently is slow
                averageVelocity += allOthers[i].GetComponent<boidsfishnew>().currentVelocity;

                centerOfMass.x += allOthers[i].transform.position.x;
                centerOfMass.y += allOthers[i].transform.position.y;
                centerOfMass.z += allOthers[i].transform.position.z;
            }
          //  }
        }
        //finish the averages
        averageHeading.x = averageHeading.x/neighbors;
        averageHeading.y = averageHeading.y/neighbors;
        averageHeading.z = averageHeading.z/neighbors;
        averageVelocity = averageVelocity/neighbors;

        centerOfMass.x = centerOfMass.x/neighbors;
        centerOfMass.y = centerOfMass.y/neighbors;
        centerOfMass.z = centerOfMass.z/neighbors;
        //issue: the animation itself includes y rotation

        //turn towards the center of mass
        float distanceToFlock = Vector3.Distance(centerOfMass, transform.position);
        currentVelocity += (Time.deltaTime * ((distanceToFlock-10)/10000)); //this needs to be negative if close enough
        var relativePos = Quaternion.LookRotation(centerOfMass- transform.position);//from docs

        float xRotation = relativePos.eulerAngles.x*attractionFactor;
        float yRotation = relativePos.eulerAngles.y*attractionFactor;
        float zRotation = relativePos.eulerAngles.z*attractionFactor;

        xRotation += averageHeading.x*alignmentFactor;
        yRotation += averageHeading.y*alignmentFactor;
        zRotation += averageHeading.z*alignmentFactor;

        float velDiff = averageVelocity - currentVelocity;
        currentVelocity += (velDiff*alignmentFactor);
        //print(currentVelocity);
        yRotation += avoidanceAngle*avoidanceFactor;
        avoidanceAngle = 0.0f;
        //avoiding obstacles should override other concerns, so do it last*/

        //for debugging obstacle avoidance-
        Vector3 centerOfMass = new Vector3(137.1f,0,97.8f);
        //float distanceToFlock = Vector3.Distance(centerOfMass, transform.position);
        //currentVelocity += (Time.deltaTime * ((distanceToFlock-10)/10000)); //this needs to be negative if close enough
        var relativePos = Quaternion.LookRotation(centerOfMass- transform.position);//from docs

        float xRotation = relativePos.eulerAngles.x;
        float yRotation = relativePos.eulerAngles.y;
        float zRotation = relativePos.eulerAngles.z;

        float obstacleAvoidanceAngle = 0.0f;
        GameObject closestObstacle = null;
        float closestDist = Mathf.Infinity;

      /*  for(int i=0; i<allObstacles.Count; i++){
            //calculate distance based on the outer shell (size) of object, not its center
            //determine which way the obstacle is facing
            //or, what axis we are looking at it on..
            //if we are looking at it on x axis, consider y and z scale.
            //actually, you know what? i'm gonna try colliders and see how it performs
            float dist = Mathf.Min((Mathf.Abs((Mathf.Abs(allObstacles[i].transform.position.z-transform.position.z))-(allObstacles[i].transform.localScale.z/2))),(Mathf.Abs((Mathf.Abs(allObstacles[i].transform.position.x-transform.position.x))-(allObstacles[i].transform.localScale.x/2))));
            //todo- should not be z
            if(dist < closestDist){
                closestDist = dist;
                closestObstacle = allObstacles[i];
            }
        }

        if(closestDist <= obstacleAvoidRadius){
            closestObstacle.GetComponent<Renderer>().material.SetColor("_Color",Color.red);
            //we especially don't want to go inside the obstacle
            /*if(closestDist <= obstacleInsideRadius){
                //print("aah!");
                //improve this- point it away from the wall
                transform.LookAt(closestObstacle.transform.position, Vector3.up);
                transform.Translate(Vector3.back * (Time.deltaTime*10));
            //todo: maybe distance times 2?
            }
            else{
                //obstacleAvoidanceAngle = 180.0f/(closestDist-3);
                //try something difference..
                //still make sure it cant go inside though
                if(closestDist < 6){
                    obstacleAvoidanceAngle = 180.0f;
                }
                else{
                    obstacleAvoidanceAngle = 180.0f/closestDist;
                }
                if(isSpecial){print("avoidance angle: "+obstacleAvoidanceAngle);}
                if(obstacleAvoidanceAngle > 360){
                    print(closestDist);
                    Debug.Break();
                }
                //wait is it because it's just flipping it 180 back and forth??
                // i think actually the best think to do would be to calculate an "away from" position
                //velocity still needs to be slower.
                //if object is to the left of me, turn right
                //cite
                var heading = closestObstacle.transform.position - transform.position;
                var dot = Vector3.Dot(heading, transform.right);

                //todo- randomly decide whether to turn up/down or to the side
                if(dot > 0){
                    //its to the right, so i wanna turn left
                    obstacleAvoidanceAngle = 1-obstacleAvoidanceAngle;
                }
                //avoidanceSpeed = 1.0f/dist;
            //}
        }

        else{
           closestObstacle.GetComponent<Renderer>().material.SetColor("_Color",Color.white);
        }*/

        Quaternion rot =  Quaternion.Euler(xRotation, yRotation, zRotation);
        transform.rotation= Quaternion.Slerp(transform.rotation, rot, (1/damping * Time.deltaTime)); 

        //yRotation += obstacleAvoidanceAngle;
        //i think this is the problem = it shouldn't be added to the rotation
        //print("avoidance anlge:"+avoidanceAngle);

        //Quaternion rot =  Quaternion.Euler(xRotation, yRotation, zRotation);
        //rot =  Quaternion.Euler(xRotation, obstacleAvoidanceAngle, zRotation);
        
        currentVelocity = currentVelocity*speedFactor;
        mainAnimator.SetFloat("speed", (currentVelocity*10)+0.2f);

        //move forward
        //transform.rotation= Quaternion.Slerp(transform.rotation, rot, .01f); 
       // transform.Rotate(xRotation, obstacleAvoidanceAngle, zRotation, Space.Self); 
       // if(isSpecial){print(transform.rotation);}
        //transform.Translate(Vector3.forward * currentVelocity);
    }
}
