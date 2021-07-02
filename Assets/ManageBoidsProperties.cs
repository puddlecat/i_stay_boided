using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageBoidsProperties : MonoBehaviour
{
    private List<SingleBoid> boids = new List<SingleBoid>();
    public float perceptionDistance = 40;
    public float perceptionWidth = 0.5f;
    public float speedFactor = 1.0f;
    [Range(0, 2)]
    public float cohesionSpeed =0.5f;
    [Range(0, 1)]
    public float alignmentFactor = 1.0f;
    public float avoidanceRadius = 7.0f;
    [Range(0, 2)]
    public float avoidanceSpeed = 0.5f;
    public float awarenessDistance = 60.0f; //for now
    public float followingFactor = 0.0f;

    void Start()
    {
        var boidss = FindObjectsOfType<SingleBoid>();
        for(int i=0; i<boidss.Length; i++){
            boids.Add(boidss[i]);
        }
    }

    void Update()
    {
        for(int i=0; i<boids.Count; i++){
            boids[i].speedFactor = speedFactor;
            boids[i].cohesionSpeed = cohesionSpeed;
            boids[i].avoidanceSpeed = avoidanceSpeed;
            boids[i].alignmentFactor = alignmentFactor;
            boids[i].avoidanceRadius = avoidanceRadius;
            boids[i].awarenessDistance = awarenessDistance;
            boids[i].followingFactor = followingFactor;
        }
    }
}
