using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class startingWTurnaway : MonoBehaviour
{
    private List<Collider> allObstacles = new List<Collider>();
    public float obstacleAvoidanceDistance = 5.0f;
    public float damping = 0.2f;
    public float cornerRadius = 0.3f;
    private List<GameObject> allOtherFish = new List<GameObject>();
    private List<startingWTurnaway> allScripts = new List<startingWTurnaway>(); //i think this should be faster than using getcomponent every frame
    [SerializeField] Animator mainAnimator;
    private List<Collider> allFishColliders = new List<Collider>();
    public float awarenessRadius = 30.0f;
    private int neighbors = 0;
    public float attractionFactor = 1.0f;
    public float velocityFactor = 0.0f;
    public float currentVelocity = 0.0f;
    private float distToGoal = 0.0f;
    private Vector3 centerOfMass = new Vector3(0.0f, 0.0f, 0.0f);
    public float averageVelocity = 0.0f;
    private Vector3 averageHeading = new Vector3(0.0f, 0.0f, 0.0f);
    public float alignmentFactor = 0.1f;
    public bool isSpecial = false;
    private GameObject centerOfMassDebug = null;
    private GameObject closestFishDebug = null;
    public float avoidanceFishDistance = 3.0f;
    private GameObject closestFish;
    private float closestFishDist;
    public float avoidFactor = 1.0f;

    float mapp(float inn){ //maps current velocity to an animation speed
        return((inn - 0.01f) * (1.8f - 0.15f) / (0.1f - 0.01f) + 0.15f); //copied from the arduino function. lul
    }

    /*float mappDistance(float inn){ //maps distance from obstacle to a value from 0 to 1- for lerp
        return((inn - 0.01f) * 1.0f / (0.1f - 0.01f));
    }*/ 
    //can be replaced with simply (1/) value / obstacleAvoidanceDistance

    void Start() 
    {   
        //get the other fish and obstacles
        var allOtherss = GameObject.FindGameObjectsWithTag("obstacle");
        for(int i=0; i<allOtherss.Length; i++){
            allObstacles.Add(allOtherss[i].gameObject.GetComponent<Collider>());
        }
        allOtherss = GameObject.FindGameObjectsWithTag("fish");
        for(int i=0; i<allOtherss.Length; i++){
            if (allOtherss[i].gameObject != gameObject){
                allOtherFish.Add(allOtherss[i]);
                allScripts.Add(allOtherss[i].GetComponent<startingWTurnaway>());
                allFishColliders.Add(allOtherss[i].GetComponent<Collider>());
            }
        }
        //assign a random starting rotation
        currentVelocity = velocityFactor;
        transform.Rotate(0.0f, Random.Range(0.0f, 90f), Random.Range(-30.0f, 0.0f), Space.Self);

        if(isSpecial){
            centerOfMassDebug = GameObject.Find("center of mass");
        }
        if(isSpecial){
            closestFishDebug = GameObject.Find("closest fish position");
        }
    }

    void Update()
    {   
        float avoidxRotation = 0.0f; //we dont want to roll
        float avoidyRotation = transform.eulerAngles.y;
        float avoidzRotation = transform.eulerAngles.z;
        Quaternion avoidrot = new Quaternion(0,0,0,0);//doesntmatter
        Vector3 avoidvector = new Vector3(0.0f, 0.0f, 0.0f);

        float closestDist = Mathf.Infinity; //should these not be redeclared?
        float secondClosestDist = Mathf.Infinity+1;
        Vector3 closestPoint = new Vector3(0.0f, 0.0f, 0.0f);
        Vector3 secondClosestPoint = new Vector3(0.0f, 0.0f, 0.0f);

        averageVelocity = 0.0f;
        averageHeading = new Vector3(0.0f, 0.0f, 0.0f);

        closestFishDist = Mathf.Infinity;
        closestFish = null;

        //find the closest point on an obstacle collider
        for(int i=0; i<allObstacles.Count;i++){
            float disty = Vector3.Distance(transform.position, allObstacles[i].ClosestPoint(transform.position));
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

        if(closestDist <= obstacleAvoidanceDistance){
            if(Mathf.Abs(closestDist - secondClosestDist) <= cornerRadius){
                closestPoint = Vector3.Lerp(closestPoint, secondClosestPoint, 0.5f); 
                closestDist = Vector3.Distance(transform.position, closestPoint);
            }
            avoidrot = Quaternion.LookRotation(transform.position - closestPoint); 
            avoidxRotation = avoidrot.eulerAngles.x;
            avoidyRotation = avoidrot.eulerAngles.y;
            avoidzRotation = avoidrot.eulerAngles.z;
            avoidvector = new Vector3(avoidxRotation, avoidyRotation, avoidzRotation);
        }

        //check on the fish close to me, get their average heading/velocity and location
        centerOfMass = new Vector3(0.0f, 0.0f, 0.0f);
        neighbors = 0;
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
        //TODO: this should be wrapped in an if, since otherwise it will divide by 0 when aware of nobody
        centerOfMass.x = centerOfMass.x/neighbors;
        centerOfMass.y = centerOfMass.y/neighbors;
        centerOfMass.z = centerOfMass.z/neighbors;
        averageVelocity = averageVelocity/neighbors;
        averageHeading.y = averageHeading.y/neighbors;
        averageHeading.z = averageHeading.z/neighbors;

        /*if(isSpecial){ //for debug
            centerOfMassDebug.transform.position = centerOfMass;
        }*/

        Quaternion attractRot = Quaternion.LookRotation(centerOfMass - transform.position); 
        float boidyRotation = attractRot.eulerAngles.y*attractionFactor;
        float boidzRotation = attractRot.eulerAngles.z*attractionFactor;
        float boidxRotation = 0.0f;
        Vector3 vectorAttract = new Vector3(boidxRotation,boidyRotation,boidzRotation);
        //add alignment, and fish avoidance
        boidyRotation += averageHeading.y*alignmentFactor;
        boidzRotation += averageHeading.z*alignmentFactor;
        if(closestFishDist <= avoidanceFishDistance){
            Quaternion awayRot = Quaternion.LookRotation(transform.position - closestFish.transform.position); 
            boidyRotation += awayRot.eulerAngles.y*avoidFactor;
            boidzRotation += awayRot.eulerAngles.z*avoidFactor;
            /*if(isSpecial){ //for debug
                closestFishDebug.transform.position = closestFish.transform.position;
            }*/
        }
        //averagevelocity uhh how did i do this in the other filedf

        /*Vector3 boidsvector = new Vector3(boidxRotation, boidyRotation, boidzRotation);
        Vector3 avoidsvector = new Vector3(avoidxRotation, avoidyRotation, avoidzRotation);
        Vector3 vectorfinalrot = Vector3.Lerp(avoidsvector, boidsvector, closestDist/obstacleAvoidanceDistance);
        Quaternion quatfinalrot = Quaternion.Euler(vectorfinalrot.x, vectorfinalrot.y, vectorfinalrot.z);*/

        Quaternion quatfinalrot = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
        if(closestDist <= obstacleAvoidanceDistance){
            //avoid the obstacle, ignore other factors
            //the problem here is that they're just gathering at the wall
            quatfinalrot = Quaternion.Euler(avoidxRotation, avoidyRotation, avoidzRotation);
           /* if(isSpecial){
                print("avoiding");
            }*/
        }

        else{
            //apply the attract, align and avoid
            quatfinalrot = Quaternion.Euler(boidxRotation,boidyRotation,boidzRotation);
            /*if(isSpecial){
                print("not avoiding");
            }*/
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, quatfinalrot, ((closestDist*damping) * Time.deltaTime)); 

        //figure out how fast to go
        //currentVelocity = (Vector3.Distance(centerOfMass, transform.position))/10;//TODO: position minus 6?
        //currentVelocity = currentVelocity * velocityFactor;

        //for debug
        if(closestDist < 0.2){
            Debug.Break();
            Selection.activeGameObject = this.gameObject;
        }

        transform.Translate(Vector3.forward* (Time.deltaTime*currentVelocity));
    }
}
