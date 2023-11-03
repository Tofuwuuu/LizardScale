using UnityEngine;
[CreateAssetMenu(fileName = "EnemyAttackSO", menuName ="Scriptable Objects/Enemy Attack")]
public class EnemyAttackSO : ScriptableObject
{
    [SerializeField] float attackRange;
    [SerializeField] float attackDamage;
    [SerializeField] string attackName;
}
