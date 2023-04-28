using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stomp : MonoBehaviour
{
    public PlayerMovement movement;
    void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();

        if(enemy != null && enemy.canGetStomped)
        {
            //kill enemy
            enemy.Death();
            //apply stompBounce to player
            movement.stompBounce();
        }
    }
}
