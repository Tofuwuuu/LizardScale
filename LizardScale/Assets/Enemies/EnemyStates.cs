using System;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public abstract class BaseState
{
    public abstract void StateUpdate(Enemy enemy);
    public abstract void StateBegin(Enemy enemy);
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
    public bool CheckAgroDistance(Vector3 enemyLocation, Vector3 targetLocation, float agroRadius)
    {
        return(Vector3.Distance(enemyLocation, targetLocation) < agroRadius);
    }

    public bool MoveTo(Enemy enemy, Vector2 target, float speed)
    {
        enemy.transform.position = Vector2.MoveTowards(enemy.transform.position, target, speed);
        if(Vector2.Distance(new Vector2(enemy.transform.position.x, 0), new Vector2(target.x, 0)) < .01f)
        {
            return true;
        }
        return false;
    }
}

public class Idle : BaseState //Not Moving
{
    public override void StateUpdate(Enemy enemy)
    {
        if(CheckAgroDistance(enemy.transform.position, GetPlayerLocation(enemy), enemy.agroRange))
        {
            //Debug.Log("Player in Range");
        }
        else
        {
            //Debug.Log("Player out of range");
        }
        
    }
    public override void StateBegin(Enemy enemy)
    {

    }
    public override void StateEnd (Enemy enemy)
    {

    }

}





public class Pacing : BaseState //Choses 2 points on the current platform and walks between them
{
    enum sides { right,left}
    bool status;
    sides currentTarget;
    float progress;
    public override void StateUpdate(Enemy enemy)
    {
        if(enemy.Platform != null)
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
                }
                else
                {
                    currentTarget = sides.left;
                }
                progress = 0;
                status = false;
            }
        }
    }
    public override void StateBegin(Enemy enemy)
    {
        currentTarget = sides.left;
    }
    public override void StateEnd(Enemy enemy)
    {

    }

}
