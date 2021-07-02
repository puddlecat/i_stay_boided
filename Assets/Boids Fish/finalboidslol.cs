using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class finalboidslol : MonoBehaviour
{
    public bool isSpecial = false;
    public float velocityFactor = 0.0f;
    public float currentVelocity = 0.0f;
    private float distToGoal = 0.0f;
    private Vector3 centerOfMass = new Vector3(0.0f, 0.0f, 0.0f);
    private List<Collider> allObstacles = new List<Collider>();
    private List<GameObject> allOtherFish = new List<GameObject>();
    private List<finalboidslol> allScripts = new List<finalboidslol>(); //i think this should be faster than using getcomponent every frame
    [SerializeField] Animator mainAnimator;
    public float awarenessRadius = 30.0f;
    private int neighbors = 0;
    public float attractionFactor = 1.0f;
    public float averageVelocity = 0.0f;
    private Vector3 averageHeading = new Vector3(0.0f, 0.0f, 0.0f);
    public float alignmentFactor = 0.1f;
    public float damping = 0.1f;
    private List<Collider> allFishColliders = new List<Collider>();
    public float avoidanceFishDistance = 3.0f;
    private GameObject closestFish;
    private float closestFishDist;
    public float avoidanceFactor = 1.0f;
    public float obstacleAvoidanceDistance = 8.0f;

    float mapp(float inn){ //maps current velocity to an animation speed
        return((inn - 0.01f) * (1.8f - 0.15f) / (0.1f - 0.01f) + 0.15f); //copied from the arduino function. lul
    }

    void Start()
    {
        //get all other fish
        var allOtherss = GameObject.FindGameObjectsWithTag("fish");
        for(int i=0; i<allOtherss.Length; i++){
            if (allOtherss[i].gameObject != gameObject){
                allOtherFish.Add(allOtherss[i]);
                allScripts.Add(allOtherss[i].GetComponent<finalboidslol>());
                allFishColliders.Add(allOtherss[i].GetComponent<Collider>());
            }
        }

        //get all obstacles
        allOtherss = GameObject.FindGameObjectsWithTag("obstacle");
        for(int i=0; i<allOtherss.Length; i++){
            if (allOtherss[i].gameObject != gameObject){
                allObstacles.Add(allOtherss[i].gameObject.GetComponent<Collider>());
            }
        }

        //assign a random starting rotation
        transform.Rotate(0.0f, Random.Range(0.0f, 60f), Random.Range(-30.0f, 0.0f), Space.Self);
        currentVelocity = 0.1f;
    }

    void Update()
    {
        //check on the fish close to me, get their average heading/velocity and location
        centerOfMass = new Vector3(0.0f, 0.0f, 0.0f);
        neighbors = 0;
        averageVelocity = 0.0f;
        averageHeading = new Vector3(0.0f, 0.0f, 0.0f);
        closestFishDist = Mathf.Infinity;
        closestFish = null;

        for(int i = 0; i<allOtherFish.Count; i++){
            float distanceToFish = Vector3.Distance(allOtherFish[i].transform.position, transform.position);
            if(distanceToFish <= awarenessRadius){
                neighbors += 1;
                centerOfMass.x += allOtherFish[i].transform.position.x;
                centerOfMass.y += allOtherFish[i].transform.position.y;
                centerOfMass.z += allOtherFish[i].transform.position.z;
                averageVelocity += allScripts[i].currentVelocity;
                averageHeading.y += allOtherFish[i].transform.eulerAngles.y;
                averageHeading.z += allOtherFish[i].transform.eulerAngles.z;

                if(distanceToFish <= avoidanceFishDistance){
                    if(distanceToFish < closestFishDist){
                        closestFishDist = distanceToFish;
                        closestFish = allOtherFish[i];
                    }
                }
            }
        }
            //finish the averages
            //if(neighbors>0){

           // }
            centerOfMass.x = centerOfMass.x/neighbors;
            centerOfMass.y = centerOfMass.y/neighbors;
            centerOfMass.z = centerOfMass.z/neighbors;
            averageVelocity = averageVelocity/neighbors;
            averageHeading.y = averageHeading.y/neighbors;
            averageHeading.z = averageHeading.z/neighbors;
            //TODO: this should be wrapped in an if, since otherwise it will divide by 0 when aware of nobody

            //create rotation turned towards center of mass
            Quaternion attractRot = Quaternion.LookRotation(centerOfMass - transform.position); 
            float yRotation = attractRot.eulerAngles.y*attractionFactor;
            float zRotation = attractRot.eulerAngles.z*attractionFactor;
            float xRotation = 0.0f;

            //figure out how fast to go
            currentVelocity = (Vector3.Distance(centerOfMass, transform.position))/100;//TODO: position minus 6?

            //add alignment with neighbors
            yRotation += averageHeading.y*alignmentFactor;
            zRotation += averageHeading.z*alignmentFactor;
            currentVelocity += (averageVelocity-currentVelocity)*alignmentFactor;   

            //add avoidance of closest neighbor
            if(closestFish != null){
                Quaternion rotAvoid = Quaternion.LookRotation(transform.position - closestFish.transform.position); 
                yRotation += rotAvoid.eulerAngles.y*avoidanceFactor;
                zRotation += rotAvoid.eulerAngles.z*avoidanceFactor;

                //transform.rotation= Quaternion.Slerp(transform.rotation, rotAvoid, (1/damping)*Time.deltaTime);

                //issue here- if other fish is below it, it's pointing down
                //subtracting z rotation seems to work?
            } 

        attractRot = Quaternion.Euler(xRotation, yRotation, zRotation);
        transform.rotation= Quaternion.Slerp(transform.rotation, attractRot, (1/damping)*Time.deltaTime);

        //add avoidance of closest obstacle, this should basically override everything else since we don't ever want it to be able to swim through walls
        float closestObstacleDist = Mathf.Infinity; //should these not be redeclared?
        Vector3 closestObstaclePoint = new Vector3(0.0f, 0.0f, 0.0f);
        //find the closest point on an obstacle collider
        for(int i=0; i<allObstacles.Count;i++){
            float disty = Vector3.Distance(transform.position, allObstacles[i].ClosestPoint(transform.position));
            if(disty < closestObstacleDist){
                //detect corner well whatever work on this later
               /* if((closestDist != Mathf.Infinity)){// &(Mathf.Abs(disty - closestDist) <= cornerRadius)){
                    print(Mathf.Abs(disty-closestDist));
                }*/
                closestObstacleDist = disty;
                closestObstaclePoint = allObstacles[i].ClosestPoint(transform.position);
            }
        }
        if(closestObstacleDist <= obstacleAvoidanceDistance){
            //add a turn away from obstacle depending on how close it is
           // float turnAwayAngle = (180.0f/obstacleAvoidanceDistance)/closestObstacleDist;
            //if(isSpecial){print(turnAwayAngle);}
            Quaternion awayObstacleRot = Quaternion.LookRotation(transform.position - closestObstaclePoint); 
            
            //is this correct to add them? no it is not.
            /*xRotation += awayObstacleRot.eulerAngles.x;
            yRotation += awayObstacleRot.eulerAngles.y;
            zRotation += awayObstacleRot.eulerAngles.z;*/
            //uhm but heres the thing. its rapidly rotating it 180 back and forth
            //(180*avoidRadius)/distance?
            transform.rotation= Quaternion.Slerp(transform.rotation, awayObstacleRot, (1/damping)*Time.deltaTime);
        }

            //apply rotation and velocity with factors and move forwards
            currentVelocity = currentVelocity*velocityFactor;
            attractRot = Quaternion.Euler(xRotation, yRotation, zRotation);
            transform.rotation= Quaternion.Slerp(transform.rotation, attractRot, (1/damping)*Time.deltaTime);
            //transform.Translate(Vector3.forward * currentVelocity);
            //change the animation speed
            mainAnimator.SetFloat("speed", mapp(currentVelocity));
    }
}
