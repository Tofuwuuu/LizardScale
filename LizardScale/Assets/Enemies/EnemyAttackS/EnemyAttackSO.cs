using UnityEngine;
[CreateAssetMenu(fileName = "EnemyAttackSO", menuName ="Scriptable Objects/Enemy Attack")]
public class EnemyAttackSO : ScriptableObject
{
    [SerializeField] public float attackRange;
    [SerializeField] public float attackDamage;
    [SerializeField] public float recoveryTime;
    [SerializeField] public string attackName;
    [SerializeField] public bool needsLineOfSight;
}
