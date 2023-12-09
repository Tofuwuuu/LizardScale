using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerlanding : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            GetComponentInParent<PlayerMovement>().platform = collision.gameObject.GetComponent<Platform>();
            GetComponentInParent<PlayerMovement>().animator.SetBool("Midair", false);
            GetComponentInParent<PlayerMovement>().canJump = true;
        }
    }
}
