using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollector : MonoBehaviour
{
    PlayerStats player;
    CircleCollider2D playerCollector;
    public float pullSpeed;

    void Start()
    {
        player = FindObjectOfType<PlayerStats>();
        playerCollector = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        playerCollector.radius = player.currentMagnet;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //check if the other game object has the ICollectible interface
        if (col.gameObject.TryGetComponent(out ICollectible collectible))
        {
            //PULLING// 
            //get the rigidbody2d component on the item
            Rigidbody2D rb = col.gameObject.GetComponent<Rigidbody2D>();
            //vector2 pointing from the item to the player
            Vector2 forceDirection = (transform.position - col.transform.position).normalized;
            //apply force to the item in the forceDirection with pullSpeed
            rb.AddForce(forceDirection * pullSpeed);

            //if it does, call the collect method
            collectible.Collect();
        }
    }
}
