using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public Vector2 leftBound {get; private set;}
    public Vector2 rightBound { get; private set; }
    public GameObject player { get; private set; }
    private float enemyWidthOffset = 0;
    [HideInInspector] public float EnemyWidthOffset { get { return enemyWidthOffset; } set { enemyWidthOffset = value + edgeBuffer; } }//Enemy will define when it lands on a platform
    public ConnectionPlatform[] surroundingPlatformsData;
    BoxCollider2D boxCollider;
    LayerMask ignore;
    PathingManager pathingManager;

    public float horizontalDropDistance = 1; //The amount that the enemy moves sideways while dropping down onto a lower platform. Will depend on how big the enemy sprites are. Can edit in inspector if its a close call whether the enemy will land or not
    float edgeBuffer = .1f; //Distance enemies will stand from the edge, this gets added to the width offset
    float jumpClearanceModifier = .2f;












    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        ignore |= (1 << LayerMask.NameToLayer("Environment"));//prevents accidental detection of enemies or other objects while defining platform boundaries. Walls and Platforms must be in the "Environment" layer
        SetBounds();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Start()
    {
        GenerateNearbyPlatformLinks();
        pathingManager = GameObject.FindGameObjectWithTag("PathingController").GetComponent<PathingManager>();
        if(pathingManager != null && surroundingPlatformsData.Length != 0)
        {
            AddDataToManager();
        }
    }



    private void SetBounds()
    {
        leftBound = new Vector2((transform.position.x + boxCollider.bounds.extents.x * -1) + enemyWidthOffset , transform.position.y + boxCollider.bounds.extents.y); //Left-most point of the platform
        rightBound = new Vector2((transform.position.x + boxCollider.bounds.extents.x) - enemyWidthOffset, transform.position.y + boxCollider.bounds.extents.y);//Right-most point of the platform

        RaycastHit2D hit;
        hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + (boxCollider.bounds.extents.y + .25f)), Vector2.left, boxCollider.bounds.extents.x,ignore);//Checks for Walls on the left
        if(hit.collider != null)
        {
            leftBound = new Vector2(hit.point.x + enemyWidthOffset, leftBound.y);
            //print("Collision on Left");
        }

        hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + (boxCollider.bounds.extents.y + .25f)), Vector2.right, boxCollider.bounds.extents.x, ignore); //Checks for Walls on the Right
        if (hit.collider != null)
        {
            rightBound = new Vector2(hit.point.x - enemyWidthOffset, rightBound.y);
            //print("Collision on Right");
        }
        Debug.DrawLine(leftBound, rightBound, Color.green, 999f);
        //print(leftBound+  "," + rightBound);
    }

    private void GenerateNearbyPlatformLinks()
    {
        if(surroundingPlatformsData.Length > 0)
        {
            ConnectionPlatform plat;
            for(int i = 0; i < surroundingPlatformsData.Length; i++)
            {
                plat = surroundingPlatformsData[i];
                if(plat.connectionEnabled) 
                {
                    if (plat.overlapping)//Lower platforms
                    {
                        switch (plat.side)
                        {
                            case ConnectionPlatform.sides.left:
                                if(leftBound.y < plat.otherPlatform.GetComponent<Platform>().leftBound.y)//If the connected platform is above the current platform
                                {
                                    plat.currentPlatformJumpPoint = new Vector2(plat.otherPlatform.GetComponent<Platform>().rightBound.x + horizontalDropDistance + jumpClearanceModifier, leftBound.y);
                                    plat.connectedPlatformJumpPoint = plat.otherPlatform.GetComponent<Platform>().rightBound;
                                    //Debug.DrawLine(plat.currentPlatformJumpPoint, plat.connectedPlatformJumpPoint, Color.magenta, 999);
                                }
                                else
                                {
                                    plat.connectedPlatformJumpPoint = new Vector2(leftBound.x - horizontalDropDistance, plat.otherPlatform.GetComponent<Platform>().rightBound.y);
                                    plat.currentPlatformJumpPoint = leftBound;
                                    //Debug.DrawLine(plat.currentPlatformJumpPoint, plat.connectedPlatformJumpPoint, Color.yellow, 999);
                                }
                                break;
                            case ConnectionPlatform.sides.right:
                                if (rightBound.y < plat.otherPlatform.GetComponent<Platform>().rightBound.y)//If the connected platform is above the current platform
                                {
                                    plat.currentPlatformJumpPoint = new Vector2(plat.otherPlatform.GetComponent<Platform>().leftBound.x - horizontalDropDistance - jumpClearanceModifier, rightBound.y);
                                    plat.connectedPlatformJumpPoint = plat.otherPlatform.GetComponent<Platform>().leftBound;
                                    //Debug.DrawLine(plat.currentPlatformJumpPoint, plat.connectedPlatformJumpPoint, Color.magenta, 999);
                                }
                                else
                                {
                                    plat.connectedPlatformJumpPoint = new Vector2(rightBound.x + horizontalDropDistance, plat.otherPlatform.GetComponent<Platform>().leftBound.y);
                                    plat.currentPlatformJumpPoint = rightBound;
                                    //Debug.DrawLine(plat.currentPlatformJumpPoint, plat.connectedPlatformJumpPoint, Color.yellow, 999);
                                } 
                                break;
                            default:
                                print("Invalid platform side setting");
                                break;
                        }
                    }
                    else
                    {
                        switch (plat.side)//Platforms that arent directly below/above the current platform
                        {
                            case ConnectionPlatform.sides.left:
                                plat.currentPlatformJumpPoint = leftBound;
                                plat.connectedPlatformJumpPoint = plat.otherPlatform.GetComponent<Platform>().rightBound;
                                //Debug.DrawLine(plat.currentPlatformJumpPoint, plat.connectedPlatformJumpPoint, Color.red, 999);
                                break;
                            case ConnectionPlatform.sides.right:
                                plat.currentPlatformJumpPoint = rightBound;
                                plat.connectedPlatformJumpPoint = plat.otherPlatform.GetComponent<Platform>().leftBound;
                                //Debug.DrawLine(plat.currentPlatformJumpPoint, plat.connectedPlatformJumpPoint, Color.red, 999);
                                break;
                            default:
                                print("Invalid platform side setting");
                                break;
                        }
                    }
                    //plat.cost = Vector2.Distance(plat.currentPlatformJumpPoint, plat.connectedPlatformJumpPoint);
                
                }
            }
        }
    }

    private void AddDataToManager()
    {
        pathingManager.AddConnections(this, surroundingPlatformsData);
    }
}



[Serializable]
public class ConnectionPlatform : IComparable<ConnectionPlatform>//Assign these in the inspector
{
    public enum sides { right, left }
    public bool connectionEnabled = true;
    public GameObject otherPlatform;
    public sides side;//0 if can be jumped to from either side of current platform, 1 if only on the left bound, 2 if only on the right bound
    public bool overlapping;//Whether or not the player jumps down to this platform or across to it (See pathfinding documentation in the discord for visuals)
    public float cost;

    [HideInInspector] public Vector2 connectedPlatformJumpPoint;
    [HideInInspector] public Vector2 currentPlatformJumpPoint;

    public int CompareTo(ConnectionPlatform other)
    {
        return cost.CompareTo(other.cost);
    }
}