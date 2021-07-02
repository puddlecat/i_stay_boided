using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swimToGoalSimple : MonoBehaviour
{
    //intended to test difference velocities and animation speeds to see which looks most natural for swimming
    private GameObject goal;
    public float velocityFactor = 0.0f;
    public float currentVelocity = 0.0f;
    private float distToGoal = 0.0f;
    [SerializeField] Animator mainAnimator;

    //this might be sooo inefficient! but i have to seee
    float mapp(float inn){
        return((inn - 0.01f) * (1.8f - 0.15f) / (0.1f - 0.01f) + 0.15f); //copied from the arduino function. lul
    }


    void Start(){
        goal = GameObject.Find("goal");
    }

    void Update(){
        distToGoal = Vector3.Distance(goal.transform.position, transform.position);
        currentVelocity = (distToGoal-6)/1000;
        currentVelocity = currentVelocity*velocityFactor;
        transform.LookAt(goal.transform.position);
        transform.Translate(Vector3.forward * currentVelocity);
        mainAnimator.SetFloat("speed", mapp(currentVelocity));
    }
}
