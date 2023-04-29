using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smash : MonoBehaviour
{
    public PlayerMovement movement;
    private void Awake()
    {
        //PlayerMovement movement = GetComponent<PlayerMovement>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        Brick brick = other.GetComponent<Brick>();

        /*if (brick != null && movement.rb.velocity.x >= movement.thirdSpeed)
        {
            brick.brickBreak();
        }*/
    }
}
