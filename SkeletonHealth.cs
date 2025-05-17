using UnityEngine;
using UnityEngine.UI;
using System.Collections; 

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100; // Максимальное здоровье
    public int currentHealth;   // Текущее здоровье
    public Image healthBar;     // Полоса здоровья (если нужно отображать)
    
    private Animator animator;
    private bool isDead = false; // Флаг, чтобы отслеживать, умер ли враг

    void Start()
    {
        currentHealth = maxHealth; // Инициализация здоровья
        animator = GetComponent<Animator>(); // Получаем компонент Animator
        UpdateHealthBar();
    }

    public void TakeDamage(int amount)
    {
        // Если враг уже мертв, не принимаем урон
        if (isDead)
            return;

        currentHealth -= amount; // Уменьшаем здоровье на величину урона
        if (currentHealth < 0) currentHealth = 0; // Проверка на минимальное значение здоровья

        UpdateHealthBar();
        Debug.Log("Враг получил урон: " + amount + ", текущее ХП: " + currentHealth);

        if (currentHealth <= 0 && !isDead)
        {
            Die(); // Если здоровье стало 0, враг умирает
        }
        else
        {
            if (animator != null)
            {
                animator.SetTrigger("yron"); // Запускаем анимацию получения урона
            }
        }
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = (float)currentHealth / maxHealth; // Обновление полосы здоровья
        }
    }

    void Die()
    {
        if (animator != null)
        {
            animator.SetTrigger("death"); // Запускаем анимацию смерти
        }

        isDead = true; // Отмечаем, что враг мертв

        // Останавливаем физику врага
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = false; // Отключаем физику
        }

        // Останавливаем движение врага
        EnemyAttack enemyAttack = GetComponent<EnemyAttack>();
        if (enemyAttack != null)
        {
            enemyAttack.enabled = false; // Если есть компонент атаки, отключаем его
        }

        // Останавливаем вращение врага, если нужно
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll; // Фиксируем все оси (положение и вращение)
        }

        // Запускаем Coroutine для задержки перед исчезновением врага
        StartCoroutine(HandleDeath());
    }

    private IEnumerator HandleDeath()
    {
        // Ждем завершения анимации смерти (например, 2 секунды)
        yield return new WaitForSeconds(2f); // Поставьте время, соответствующее длительности анимации смерти

        // Удаляем врага из игры
        Destroy(gameObject);

        Debug.Log("Враг уничтожен.");
    }

    // Проверка на смерть врага, чтобы другие методы не могли воздействовать на него
    public bool IsDead()
    {
        return isDead;
    }
}

public class EnemyAttack : MonoBehaviour
{
    public int damage = 15;
    public float attackCooldown = 1f;
    public float attackRange = 2f;
    public float attackMinDamage = 15f;
    public float attackMaxDamage = 25f;
    public Animator animator;

    private float lastAttackTime;
    private EnemyHealth enemyHealth;

    private void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();

        if (animator != null)
        {
            animator.SetTrigger("spawn"); // Анимация спавна
        }
    }

    private void Update()
    {
        if (enemyHealth != null && enemyHealth.IsDead())
        {
            if (animator != null && !animator.GetCurrentAnimatorStateInfo(0).IsName("death"))
            {
                animator.SetTrigger("death"); // Анимация смерти
            }
            return; // Враг мёртв — выход из метода
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth == null || playerHealth.currentHealth <= 0)
        {
            // Игрок мёртв — не атакуем
            if (animator != null && !animator.GetCurrentAnimatorStateInfo(0).IsName("duhanie"))
            {
                animator.SetTrigger("duhanie"); // Анимация дыхания
            }
            return;
        }

        float distance = Vector2.Distance(transform.position, player.transform.position);

        if (distance <= attackRange)
        {
            // Игрок в радиусе атаки — активируем анимацию бега
            if (animator != null && !animator.GetCurrentAnimatorStateInfo(0).IsName("run"))
            {
                animator.SetTrigger("run"); // Анимация бега
            }

            if (Time.time - lastAttackTime >= attackCooldown)
            {
                if (animator != null)
                {
                    animator.SetTrigger("ydar"); // Анимация удара
                }

                float randomDamage = Random.Range(attackMinDamage, attackMaxDamage);
                playerHealth.TakeDamage((int)randomDamage);
                Debug.Log($"Урон нанесён игроку: {randomDamage}");

                lastAttackTime = Time.time;
            }
        }
        else
        {
            // Игрок далеко — активируем анимацию дыхания
            if (animator != null && !animator.GetCurrentAnimatorStateInfo(0).IsName("duhanie"))
            {
                animator.SetTrigger("duhanie");
            }
        }
    }

    // Метод вызова при получении урона извне
    public void TakeDamage(int amount)
    {
        if (enemyHealth == null || enemyHealth.IsDead()) return;

        enemyHealth.TakeDamage(amount);
        Debug.Log($"Враг получил урон: {amount}");

        if (animator != null)
        {
            animator.SetTrigger("yron"); // Анимация получения урона
        }

        if (enemyHealth.IsDead())
        {
            if (animator != null)
            {
                animator.SetTrigger("death"); // Анимация смерти
            }
        }
    }
}
