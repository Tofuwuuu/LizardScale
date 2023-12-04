using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;


public class elevator : MonoBehaviour
{
    public List<GameObject> requiredEnemies = new List<GameObject>();
    public string nextLevel;
    void Start()
    {

    }

    public bool RoomCompleted()
    {
        bool done = true;
        foreach (GameObject obj in requiredEnemies)
        {
            if (obj.gameObject.activeSelf)
            {
                done = false;
            }
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
