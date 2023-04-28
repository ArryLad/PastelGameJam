using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Variables")]
    public int damage = 1;

    public bool canGetStomped;
    public void Death()
    {
        //Instantiate(deathParticle, transform.position, Quaternion.identity); //instantiate death particle
        //FindObjectOfType<AudioManager>().Play("sEnemyDeath");
        Destroy(transform.parent.gameObject); //destroy parent 
    }


}
