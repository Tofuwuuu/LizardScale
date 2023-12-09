using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class elevator : MonoBehaviour
{
    public List<GameObject> requiredEnemies = new List<GameObject>();
    public string nextLevel;

    public bool RoomCompleted()
    {
        bool done = true;
        foreach (GameObject obj in requiredEnemies)
        {
            if (obj.gameObject.activeSelf && obj.GetComponent<Enemy>().dead == false)
            {
                done = false;
            }
        }
        if (done)
        {
            GetComponent<Animator>().SetBool("Opening", true);
        }
        return done;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(RoomCompleted() && collision.gameObject.tag == "Player")
        {
            SceneManager.LoadScene(nextLevel);
        }
    }
}
