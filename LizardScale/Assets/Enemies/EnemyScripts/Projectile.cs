using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector2 Target;
    Vector2 dir;
    public float damage;
    float speed = 10f;

    private void Start()
    {
        this.gameObject.SetActive(false);
    }
    private void Update()
    {
        if(this.gameObject.activeSelf)
        {

            transform.Translate(dir * speed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            //do damage
        }
        gameObject.transform.position = Vector2.zero;
        this.gameObject.SetActive(false);
 
    }

    public void CalculateDirection()
    {
        dir = (Target - (Vector2)transform.position).normalized;
    }
}

