using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    [SerializeField] float xOffset = 5.5f;
    [SerializeField] float yOffset = 3.5f;

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = new Vector3(player.position.x + xOffset, player.position.y + yOffset, -100);
    }
}
