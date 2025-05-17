using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    public float range = 5f;
    public float attackRange = 1f;
    public float moveSpeed = 2f;
    public int attackDamage = 15;

    private bool isPlayerInRange = false;
    private bool hasAttacked = false;
    private bool isDead = false;

    public Transform player;
    private Animator animator;
    private EnemyHealth enemyHealth;

    void Start()
    {
        animator = GetComponent<Animator>();
        enemyHealth = GetComponent<EnemyHealth>();

        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null)
                player = foundPlayer.transform;
        }
    }

    void Update()
    {
        // Исправлено условие проверки
        if (isDead || player == null || animator == null || enemyHealth == null)
            return;

        if (!enemyHealth.enabled)
        {
            isDead = true;
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= range)
        {
            isPlayerInRange = true;
            animator.SetBool("InRage", true);

            // Выводим в консоль, когда игрок в пределах диапазона
            Debug.Log("Игрок в пределах диапазона");

            // Поворот
            if (player.position.x > transform.position.x)
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            else
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

            // Атака
            if (distanceToPlayer <= attackRange)
            {
                if (!hasAttacked)
                {
                    animator.SetTrigger("ydar");

                    PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                    if (playerHealth != null)
                        playerHealth.TakeDamage(attackDamage);

                    hasAttacked = true;
                    Invoke(nameof(ResetAttack), 1f);
                }
            }
            else
            {
                // Движение
                transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
            }
        }
        else
        {
            isPlayerInRange = false;
            animator.SetBool("InRage", false);
        }
    }

    void ResetAttack()
    {
        hasAttacked = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
