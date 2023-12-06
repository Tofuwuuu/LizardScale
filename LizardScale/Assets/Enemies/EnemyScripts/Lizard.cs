using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Lizard : Enemy
{
    // Start is called before the first frame update
    void Start()
    {
        states.Add(new Idle());//0
        states.Add(new Pacing());//1
        states.Add(new Chasing());//2
        states.Add(new MeleeAttacking());//3
        CurrentState = states[0];
        SwitchState(states[1]);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CurrentState.StateUpdate(this);
        if(CurrentState == states[1])
        {
            if(CheckAgroDistance())
            {
                SwitchState(states[2]);
            }
        }
        else if(CurrentState == states[2])
        {
            if (Platform == player.GetComponent<PlayerMovement>().platform)
            {
                if (Vector2.Distance(transform.position, player.transform.position) < attacks[0].attackRange)
                {
                    SwitchState(states[3]);
                    StartCoroutine(attackPause());
                }

                if (!CheckAgroDistance())
                {
                    SwitchState(states[1]);
                }
            }
            
        }
    }

    IEnumerator attackPause()
    {
        GetComponent<Animator>().SetBool("walk", false);
        yield return new WaitForSeconds(attacks[0].recoveryTime);
        SwitchState(states[2]);
        GetComponent<Animator>().SetBool("Attacking", false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Platform>() != null)
        {
            Platform = collision.gameObject.GetComponent<Platform>();
        }
    }

    LayerMask mask;
    public void AttackFrame()
    {
        
        mask |= (1 << 10);
        int facedir;
        if(GetComponent<SpriteRenderer>().flipX)
        {
            facedir = 1;
        }
        else
        {
            facedir = 0;
        }
        GetComponent<AudioSource>().Play();

        RaycastHit2D hit;
        Vector2 castDir = facedir == 1 ? Vector2.right : Vector2.left;
        hit = Physics2D.Raycast(gameObject.transform.position, castDir, 1.5f, mask);
        if (hit.collider != null)
        {
            //print(hit.collider.gameObject.name);
            if (hit.collider.gameObject.layer == 10)
            {
                hit.collider.gameObject.GetComponent<PlayerMovement>().Damage(25);
            }
        }
    }
}
