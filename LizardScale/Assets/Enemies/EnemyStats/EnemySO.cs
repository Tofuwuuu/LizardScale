using UnityEngine;
[CreateAssetMenu(fileName = "EnemySO", menuName = "Scriptable Objects/Enemy")]
public class EnemySO : ScriptableObject//Holds basic stats of the enemy
{
    [SerializeField] float health = 100; 
    [SerializeField] float speed = 5f;
    [SerializeField] bool canJump = false; //whether or not the enemy can move between platforms via jumping
    [SerializeField] EnemyAttackSO[] attacks; 
}
