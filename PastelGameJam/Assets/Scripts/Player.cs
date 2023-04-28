using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [Header("GameManager")]
    //public GameManager GM; //GameManager

    [Header("Player Variables")]
    public int maxHealth = 3; //Set maxhealth 
    private int currentHealth;

    [Header("iFrames")]
    [SerializeField] private Color flashColor; //Color for the player to flash 
    private Color regularColor; //regular colour of sprite will be assigned to this
    [SerializeField] SpriteRenderer sprite; //reference to spriteRenderer
    [SerializeField] float flashDuration; //flash duration
    [SerializeField] int numberOfFlashes; //the number of times the player will flicker
    [SerializeField] bool invincible; //determine if player can be damaged

    private void Awake()
    {
        currentHealth = maxHealth;
        regularColor = sprite.color; //get colour of sprite
    }
    public void TakeDamage()
    {
        currentHealth -= 1; //reduce current health
        if (currentHealth <= 0) //if no health
        {
            //GM.GameOver(); //Cause a game over
        }
        else
            StartCoroutine(iFrame()); //otherwise flash to show inviniciblity
    }

    private void Update()
    {
        //Solves build issue which prevented some collisions, likely IgnoreLayerCollisions was remaining true on a GameOver.
        if (invincible)
            Physics2D.IgnoreLayerCollision(6, 10, true);
        else
            Physics2D.IgnoreLayerCollision(6, 10, false);
    }

    private IEnumerator iFrame()
    {
        //Change the sprites colour repeatedily imitating that classic super mario style
        int temp = 0;
        invincible = true; //now player has been damaged they should be invinsible 
        while (temp < numberOfFlashes)
        {
            sprite.color = flashColor; //turn to flash colour
            yield return new WaitForSeconds(flashDuration); //how long to remain that sprite colour
            sprite.color = regularColor; //revert to regular colour
            yield return new WaitForSeconds(flashDuration);
            temp++;
        }
        invincible = false; //when flashing is over they are able to be damaged again
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (!invincible && other.gameObject.CompareTag("Enemy")) //whe not invincible and the object colliding with is an Enemy 
        {
            TakeDamage(); //take damage function
        }
    }
}