using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hitbox : MonoBehaviour
{
    List<GameObject> enemyList = new List<GameObject>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            enemyList.Add(collision.gameObject);
        }
    }
    public void Damage()
    {
        if(enemyList != null)
        {
            foreach (GameObject enemy in enemyList)
            {
                if (enemy != null)
                {
                    if (enemy.activeSelf)
                    {
                        enemy.GetComponent<Enemy>().Damage(50);
                    }
                }
            }
        }
        
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        for (int i = 0; i < enemyList.Count; i++)
        {
            if (enemyList[i] == collision.gameObject)
            {
                enemyList.Remove(enemyList[i]);
            }
        }
    }
}
