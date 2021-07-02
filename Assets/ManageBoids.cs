using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageBoids : MonoBehaviour
{
    public GameObject boidPrefab;
    public int numberOfBoids;
    public float perceptionDistance;
    
    void Start()
    {
        //create the boids
        for(int i=0; i<numberOfBoids; i++){
            GameObject newBoid = Instantiate(boidPrefab, new Vector3(i, 0, 0), Quaternion.identity);
            newBoid.GetComponent<SingleBoid>().perceptionDistance = perceptionDistance;
        }
    }

    
    void Update()
    {
        
    }
}
