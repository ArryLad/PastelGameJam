using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    public void brickBreak()
    {
        Destroy(transform.gameObject); //destroy parent
    }
}
