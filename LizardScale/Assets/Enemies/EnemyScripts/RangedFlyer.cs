using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RangedFlyer : Enemy
{
    GameObject projectile;

    private void Start()
    {
        projectile = GameObject.Find("GunReference/Projectile");
        projectile.GetComponent<Projectile>().damage = attacks[0].attackDamage;
        Shoot();
        hasAgro = false;
        states.Add(new Idle());
        states.Add(new Hovering());
        states.Add(new FlyingChasing());
        states.Add(new Shooting());
        SwitchState(states[1]);
        //states.Add(new )
    }

    private void FixedUpdate()
    {
        if (!hasAgro)
        {
            CurrentState.StateUpdate(this);
            if (CurrentState == states[1])
            {
                if (CheckAgroDistance())
                {
                    hasAgro = true;
                }
                if (CurrentState.isCompleted)
                {
                    StopCoroutine(Movementimer());
                    SwitchState(states[0]);
                    StartCoroutine(PauseHovering());
                }
            }
        }
        else
        {
            CurrentState.StateUpdate(this);
            if (CurrentState == states[2])
            {
                if (CheckAgroDistance())
                {
                    hasAgro = true;
                }
                if (CurrentState.isCompleted)
                {
                    StopCoroutine(Movementimer());
                    SwitchState(states[3]);
                }
            }
            else if (CurrentState == states[3])
            {
                Shoot();
            }
        }
    }

    protected override void SwitchState(BaseState state)
    {
        if(state == states[1] || state == states[2])//if switching to hovering state, start the timer that prevents getting stuck. Else stop it
        {
            StopCoroutine(Movementimer());
            StartCoroutine(Movementimer());
        }
        else
        {
            StopCoroutine(Movementimer());
        }
        state.StateBegin(this);
        CurrentState = state;
    }

    void Shoot()
    {
        projectile.SetActive(true);
        projectile.transform.position = GetComponentInChildren<Transform>().position;
        projectile.GetComponent<Projectile>().Target = player.transform.position;
    }


    IEnumerator PauseHovering()
    {
        yield return new WaitForSeconds(2f);
        SwitchState(states[1]);
    }

    IEnumerator Movementimer()
    {
        yield return new WaitForSeconds(4);//if it gets stuck on a wall or something while hovering it will automatically give up after this time, also this is how long the enemy chases the player before stopping to shoot
        CurrentState.StateEnd(this);
    }
}
