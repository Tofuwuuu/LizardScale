using UnityEngine;
[CreateAssetMenu(fileName = "EnemySO", menuName = "Scriptable Objects/Enemy")]
public class EnemySO : ScriptableObject//Holds basic stats of the enemy
{
    [SerializeField] public float health = 100; 
    [SerializeField] public float speed = 5f;
    [SerializeField] public float chasingSpeed = 5f;
    [SerializeField] public float agroDistance = 5f;
    [SerializeField] public bool canJump = false; //whether or not the enemy can move between platforms via jumping
    [SerializeField] public EnemyAttackSO[] attacks;
}
