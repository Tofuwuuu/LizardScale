using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lizard : Enemy
{
    PathingManager pathingManager;
    List<ConnectionPlatform> platforms = new List<ConnectionPlatform>();
    // Start is called before the first frame update
    void Start()
    {
        pathingManager = GameObject.FindGameObjectWithTag("PathingManager").GetComponent<PathingManager>();
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
        yield return new WaitForSeconds(attacks[0].recoveryTime);
        SwitchState(states[2]);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Platform>() != null)
        {
            Platform = collision.gameObject.GetComponent<Platform>();
        }
    }

    private void UpdatePath()
    {
        platforms =  pathingManager.FindPath(Platform, player.GetComponent<PlayerMovement>().platform);
    }

    private void OnEnable()
    {
        PathingManager.updated += UpdatePath;
    }
    private void OnDisable()
    {
        PathingManager.updated -= UpdatePath;
    }
}
