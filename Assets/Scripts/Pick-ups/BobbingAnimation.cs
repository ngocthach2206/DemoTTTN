using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobbingAnimation : MonoBehaviour
{
    public float frequency; //speed of movement
    public float magnitude; //range of movement
    public Vector3 direction;
    Vector3 initialPosition;
    Pickup pickup;

    void Start()
    {
        pickup = GetComponent<Pickup>();
        //save the starting position of game object
        initialPosition = transform.position;
    }

    void Update()
    {
        if(pickup && !pickup.hasBeenCollected)
        {
            //sine the function for smooth bobbing effect
            transform.position = initialPosition + direction * Mathf.Sin(Time.time * frequency) * magnitude;
        }
    }
}
