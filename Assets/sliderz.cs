using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class sliderz : MonoBehaviour
{
    public Text associatedField;
    private Slider mainSlider;
    private List<startingWTurnaway> fish = new List<startingWTurnaway>();
    //stupid way to do it but i'm running out of time
    public bool attract = false;
    public bool align = false;
    public bool avoid = false;
    public bool distance = false;

    void Start()
    {   
        mainSlider = gameObject.GetComponent<Slider>();
        mainSlider.onValueChanged.AddListener(delegate {ValueChangeCheck(); }); //docs
        var all = GameObject.FindGameObjectsWithTag("fish");
        for(int i=0; i<all.Length; i++){
            fish.Add(all[i].GetComponent<startingWTurnaway>());
        }
    }

    public void ValueChangeCheck()
    {
        associatedField.text = mainSlider.value.ToString();
        for(int i=0; i<fish.Count; i++){
            if(align){
                fish[i].alignmentFactor = mainSlider.value;
            }
            else if(avoid){
                fish[i].avoidFactor = mainSlider.value;
            }
            else if(attract){
                fish[i].attractionFactor = mainSlider.value;
            }
            else if(distance){
                fish[i].awarenessRadius = mainSlider.value;
            }
        }
    }

}
