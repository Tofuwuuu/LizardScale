using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RangedFlyer : Enemy
{
    GameObject projectile;

    private void Start()
    {
        projectile = transform.Find("GunReference/Projectile").gameObject;
        projectile.GetComponent<Projectile>().damage = attacks[0].attackDamage;
        Shoot();
        hasAgro = false;
        states.Add(new Idle());//0
        states.Add(new Hovering());//1
        states.Add(new FlyingChasing());//2
        states.Add(new Shooting());//3
        CurrentState = states[0];
        SwitchState(states[1]);
    }

    private void FixedUpdate()
    {
        CurrentState.StateUpdate(this);
        if (!hasAgro)
        {
            if(CurrentState == states[0])
            {
                hasAgro = CheckAgroDistance();
                if(hasAgro)
                {
                    SwitchState(states[2]);
                }
            }//Idling, will start chasing the player if in range
            if (CurrentState == states[1])
            {
                if (CurrentState.isCompleted)
                {
                    SwitchState(states[0]);
                    StartCoroutine(PauseHovering(1, 2));
                }
            }//Randomly Hovering
        }
        else
        {
            if (CurrentState == states[2])
            {
                hasAgro = CheckAgroDistance();
                if(!hasAgro)
                {
                    SwitchState(states[1]);
                }
                if (CurrentState.isCompleted)
                {
                    SwitchState(states[3]);
                }
            }//Chasing the player. Will deagro if he gets too far away and shoots occasionally
        }
    }

    protected override void SwitchState(BaseState state)
    {
        StopAllCoroutines();
        CurrentState.StateEnd(this);
        if(state == states[1] || state == states[2])//if switching to hovering state, start the timer that prevents getting stuck. Else stop it
        {
            StartCoroutine(Movementimer());
        }
        else if(state == states[3])
        {
            StartCoroutine(PauseHovering(2, 4));
        }
        CurrentState = state;
        state.StateBegin(this);
    }

    public void Shoot()
    {
        projectile.GetComponent<Projectile>().Target = player.transform.position;
        projectile.transform.position = GetComponentInChildren<Transform>().position;
        projectile.GetComponent<Projectile>().CalculateDirection();
        projectile.SetActive(true);
        GetComponent<AudioSource>().Play();
    }


    IEnumerator PauseHovering(int state ,float time)
    {
        yield return new WaitForSeconds(time);
        SwitchState(states[state]);
    }

    IEnumerator Movementimer()
    {
        yield return new WaitForSeconds(4);//if it gets stuck on a wall or something while hovering it will automatically give up after this time, also this is how long the enemy chases the player before stopping to shoot
        CurrentState.StateEnd(this);
    }
}
