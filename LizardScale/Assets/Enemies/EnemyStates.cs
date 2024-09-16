using System;
using System.Collections;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.SocialPlatforms;
using Unity.VisualScripting;
using static UnityEngine.GraphicsBuffer;

public abstract class BaseState
{
    public bool isCompleted { get; protected set; }//Accessible by enemy scripts running the state, is set to true if a particular state is "done" like moving to a spot or something and the enemy script needs to know
    public abstract void StateUpdate(Enemy enemy);//Run in enemy script fixedupdate
    public abstract void StateBegin(Enemy enemy);//Runs whenever switching state
    public abstract void StateEnd(Enemy enemy);
    public Vector3 GetPlayerLocation(Enemy enemy)
    {
        if(enemy.player == null)
        {
            Debug.Log("Tried to find player location, but enemy could not find player gameobject. Returned Vector3.Zero instead. Player is untagged with \"Player\" or not present.");
            return Vector3.zero;
        }
        else
        {
            return enemy.player.transform.position;
        }
    }
    public bool MoveTo(Enemy enemy, Vector2 target, float speed)
    {
        enemy.transform.position = Vector2.MoveTowards(new Vector2(enemy.transform.position.x, enemy.transform.position.y), new Vector2(target.x, enemy.transform.position.y), speed);
        if(Vector2.Distance(new Vector2(enemy.transform.position.x, 0), new Vector2(target.x, 0)) < .01f)
        {
            return true;
        }
        return false;
    }
}

public class Idle : BaseState 
{
    public override void StateUpdate(Enemy enemy)
    {
        
    }
    public override void StateBegin(Enemy enemy)
    {
        isCompleted = false;
    }
    public override void StateEnd (Enemy enemy)
    {

    }

}//Doing nothing


public class Pacing : BaseState 
{
    enum sides { right,left}
    bool status;
    sides currentTarget;
    float progress;
    public override void StateUpdate(Enemy enemy)
    {
        if (enemy.Platform != null)
        {
            if (!status)
            {
                if (currentTarget == sides.left)
                {
                    progress = Mathf.Clamp01(enemy.speed * Time.deltaTime);
                    status = MoveTo(enemy, enemy.Platform.leftBound, progress);

                }
                else
                {
                    progress = Mathf.Clamp01(enemy.speed * Time.deltaTime);
                    status = MoveTo(enemy, enemy.Platform.rightBound, progress);
                }
            }
            else
            {

                if (currentTarget == sides.left)
                {
                    currentTarget = sides.right;
                    enemy.GetComponent<SpriteRenderer>().flipX = true;
                }
                else
                {
                    enemy.GetComponent<SpriteRenderer>().flipX=false;
                    currentTarget = sides.left;
                }
                progress = 0;
                status = false;
            }
        }
        //else { Debug.Log("hh"); }
    }
    public override void StateBegin(Enemy enemy)
    {
        enemy.GetComponent<SpriteRenderer>().flipX = false;
        currentTarget = sides.left;
        enemy.GetComponent<Animator>().SetBool("walk", true);
    }
    public override void StateEnd(Enemy enemy)
    {
        
    }

}//Chooses 2 points on the current platform and walks between them


public class Hovering : BaseState
{
    Vector2 target = Vector2.zero;
    public override void StateBegin(Enemy enemy)
    {
        target = Vector2.zero;
        isCompleted = false;
        target = enemy.transform.position + new Vector3(UnityEngine.Random.Range(-3, 3), UnityEngine.Random.Range(-3, 3), 0);//picks random direction to float in
        if (enemy.transform.position.x < target.x)
        {
            enemy.GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            enemy.GetComponent<SpriteRenderer>().flipX = false;
        }
    }
    public override void StateEnd(Enemy enemy)
    {
        isCompleted = true;
    }
    public override void StateUpdate(Enemy enemy)
    {
        if (!isCompleted)
        {
            if(FloatTo(enemy, target, enemy.speed))
            {
                isCompleted = true;
            }
        }
    }

    public bool FloatTo(Enemy enemy, Vector2 target, float speed)
    {
        enemy.transform.position = Vector2.MoveTowards(enemy.transform.position, target, speed * Time.deltaTime);
        if (Vector2.Distance(enemy.transform.position, target) < .01f)
        {
            return true;
        }
        return false;
    }

}//Only for flying enemies, hovering in random directions when not idle

public class FlyingChasing : BaseState
{
    Vector2 target;
    public override void StateUpdate(Enemy enemy)
    {
        target = GetPlayerLocation(enemy);
        enemy.transform.position = Vector2.MoveTowards(enemy.transform.position, target, enemy.speed * Time.deltaTime);
        if (enemy.transform.position.x < GetPlayerLocation(enemy).x)
        {
            enemy.GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            enemy.GetComponent<SpriteRenderer>().flipX = false;
        }
    }
    public override void StateBegin(Enemy enemy)
    {
        isCompleted = false;
    }
    public override void StateEnd(Enemy enemy)
    {
        isCompleted = true;
    }
}//Only for flying enemies, when chasing the player


public class Shooting : BaseState
{
    public override void StateUpdate(Enemy enemy)
    {

    }
    public override void StateBegin(Enemy enemy)
    {
        isCompleted = false;
        RangedFlyer e = enemy as RangedFlyer;
        if(enemy.transform.position.x < GetPlayerLocation(enemy).x)
        {
            enemy.GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            enemy.GetComponent<SpriteRenderer>().flipX=false;
        }
        
        e.Shoot();
    }
    public override void StateEnd(Enemy enemy)
    {
        isCompleted = true;
    }
}


public class Chasing : BaseState
{
    public override void StateUpdate(Enemy enemy)
    {
        
        if(enemy.Platform != null)
        {
            if (enemy.transform.position.x > enemy.player.transform.position.x)
            {
                enemy.GetComponent<SpriteRenderer>().flipX = false;
                if (enemy.player.transform.position.x > enemy.Platform.leftBound.x)
                {
                    MoveTo(enemy, GetPlayerLocation(enemy), enemy.speed * Time.deltaTime);
                }
                else
                {
                    MoveTo(enemy, enemy.Platform.leftBound, enemy.speed * Time.deltaTime);
                }

            }
            else
            {
                enemy.GetComponent<SpriteRenderer>().flipX = true;
                if (enemy.player.transform.position.x < enemy.Platform.rightBound.x)
                {
                    MoveTo(enemy, GetPlayerLocation(enemy), enemy.speed * Time.deltaTime);
                }
                else
                {
                    MoveTo(enemy, enemy.Platform.rightBound, enemy.speed * Time.deltaTime);
                }
            }
        }
    }
        
    public override void StateBegin(Enemy enemy)
    {
        isCompleted=false;
    }
    public override void StateEnd(Enemy enemy)
    {
        isCompleted=true;
    }
}

public class MeleeAttacking : BaseState
{
    public override void StateUpdate(Enemy enemy)
    {
        
    }
    public override void StateBegin(Enemy enemy)
    {
        isCompleted=false;
        enemy.GetComponent<Animator>().SetBool("Attacking", true);
    }
    public override void StateEnd(Enemy enemy)
    {
        isCompleted=true;
    }
}
