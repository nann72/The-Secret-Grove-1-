using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public Image healthBar;

    private Animator animator;
    private bool isDead = false;

    private Rigidbody2D rb;
    private PlayerScript2D playerMovement;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerScript2D>(); // Получаем ссылку на компонент PlayerScript2D
        UpdateHealthBar();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return; // Если игрок мертв, не получаем урон

        Debug.Log("Метод TakeDamage вызван!");
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHealthBar();
        Debug.Log("Игрок получил урон: " + amount + ", текущее ХП: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;

        if (animator != null)
        {
            animator.SetTrigger("death"); // Запускаем анимацию смерти
        }

        // Останавливаем физику персонажа
        if (rb != null)
        {
            rb.simulated = false; // Отключаем физику
        }

        // Останавливаем движение персонажа
        if (playerMovement != null)
        {
            playerMovement.enabled = false; // Отключаем компонент движения
        }

        // Останавливаем вращение персонажа, если нужно
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll; // Фиксируем все оси (положение и вращение)
        }

        // Останавливаем персонажа на его текущем уровне по Y, чтобы он не "висел"
        Vector2 currentPosition = transform.position;
        transform.position = new Vector2(currentPosition.x, currentPosition.y); // Сохраняем текущее положение по Y

        Debug.Log("Персонаж умер.");
    }

    // Метод для проверки, мертв ли игрок
    public bool IsDead()
    {
        return isDead; // Возвращаем состояние смерти
    }
}
