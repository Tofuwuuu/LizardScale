using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePlayer : MonoBehaviour
{
    public int dir;
    float damage = 30;
    float speed = 20f;
    public void orient()
    {
        GetComponent<Animator>().SetBool("isworking", true);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf)
        {

            transform.Translate(new Vector2(dir, 0) * speed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            collision.gameObject.GetComponent<Enemy>().Damage(damage);
        }
        GetComponent<Animator>().SetBool("isworking", false);
        gameObject.transform.position = Vector2.zero;
        this.gameObject.SetActive(false);

    }
    IEnumerator despawntimer()
    {
        yield return new WaitForSeconds(1);
        GetComponent<Animator>().SetBool("isworking", false);
        gameObject.transform.position = Vector2.zero;
        this.gameObject.SetActive(false);
    }
}
