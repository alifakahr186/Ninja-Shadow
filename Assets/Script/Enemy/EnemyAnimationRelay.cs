using UnityEngine;

public class EnemyAnimationRelay : MonoBehaviour
{
    private BasicEnemyController enemy;

    [SerializeField]

    private void Awake()
    {
        enemy = GetComponentInParent<BasicEnemyController>();

    }

    public void TriggerAttack()
    {
        if (enemy != null)
            enemy.AttackTrigger();
    }

    public void FinishAttack()
    {
        if (enemy != null)
            enemy.SetIsAttacking(false);
    }


}
