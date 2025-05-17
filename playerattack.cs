using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int attackDamage = 15;  // Урон при атаке
    public float attackRange = 1.5f;  // Радиус атаки
    public LayerMask enemyLayer;  // Слой для врагов (нужно будет назначить слой для врагов)

    public Animator animator; // Аниматор игрока

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, если объект - враг
        if (other.CompareTag("Enemy"))
        {
            // Получаем компонент здоровья врага
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                // Наносим урон врагу
                enemyHealth.TakeDamage(attackDamage); 
                // Выводим сообщение для отладки
                Debug.Log($"Враг {other.name} получил урон: {attackDamage}");
            }
        }
    }

    // Метод для выполнения атаки
    public void Attack()
    {
        // Запускаем анимацию удара
        if (animator != null)
        {
            animator.SetTrigger("Attack"); // Убедитесь, что у вас есть анимация с именем "Attack"
        }
    }

    // Отображение радиуса атаки в редакторе (для отладки)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
