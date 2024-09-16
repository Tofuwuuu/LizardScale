using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frog : Enemy
{
    Animator animator;
    public AudioClip jumpsound;
    int dir = 1;
    bool hopping = false;
    bool candmg = true;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        states.Add(new Idle());
        CurrentState = states[0];
        StartCoroutine(HopTimer());
    }

    // Update is called once per frame
    

    IEnumerator HopTimer()
    {
        yield return new WaitForSeconds((float)Random.Range(2,4));
        if(transform.position.x < player.transform.position.x)
        {
            dir = 1;
            GetComponent<SpriteRenderer>().flipX = false;
        }
        else
        {
            dir = -1;
            GetComponent<SpriteRenderer>().flipX = true;
        }
        if (!dead)
        {
            GetComponent<AudioSource>().clip = jumpsound;
            GetComponent<AudioSource>().Play();
        }
        
        animator.SetBool("hopping", true);
        hopping = true;
        GetComponent<Rigidbody2D>().AddForce(new Vector2(200 * dir, 400));
        candmg = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject == player && hopping && candmg)
        {
            player.GetComponent<PlayerMovement>().Damage(25);
            candmg = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Ground")
        {
            animator.SetBool("hopping", false);
            hopping = false;
            StopAllCoroutines();
            StartCoroutine(HopTimer());
        }
    }
}
