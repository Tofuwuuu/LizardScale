using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEventsPlayer : MonoBehaviour
{
    PlayerMovement player;
    LayerMask mask;

    private void Start()
    {
        player = GetComponent<PlayerMovement>();
        mask |= (1 << 9);
    }
    public void AttackFrame()
    {
        RaycastHit2D hit;
        Vector2 castDir = player.lastmovedir == 1 ? Vector2.right : Vector2.left;
        hit = Physics2D.Raycast(gameObject.transform.position, castDir, 2, mask);
        //print(hit);
        //Debug.DrawLine(gameObject.transform.position, new Vector2(gameObject.transform.position.x + (2 * castDir.x), gameObject.transform.position.y));
        if (hit.collider != null)
        {
            //print(hit.collider.gameObject.name);
            if(hit.collider.gameObject.layer == 9)
            {
                hit.collider.gameObject.GetComponent<Enemy>().Damage(50);
            }
        }
    }

}
