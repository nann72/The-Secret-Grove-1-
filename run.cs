using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript2D : MonoBehaviour
{
    public int damage = 15; // Урон от удара
    private Rigidbody2D rb;
    private float HorizontalMove = 0f;
    private bool FacingRight = true;

    [Header("Player Movement Settings")]
    [Range(0, 10f)] public float speed = 0.1f;
    [Range(0, 15f)] public float jumpForce = 5f;

    [Header("Player Animation Settings")]
    public Animator animator;

    [Space]
    [Header("Ground Checker Settings")]
    public bool isGrounded = false;
    [Range(-5f, 5f)] public float checkGroundOffsetY = -1.8f;
    [Range(0, 5f)] public float checkGroundRadius = 0.3f;

    [Header("Jump Cooldown Settings")]
    public float jumpCooldown = 10.0f;
    private float lastJumpTime = -Mathf.Infinity;

    [Header("Attack Settings")]
    public float attackRange = 1.5f;  // Радиус атаки
    public int attackDamage = 15;     // Урон при атаке

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Проверка на землю с использованием Raycast
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.1f);

        // Прыжок с кулдауном
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && Time.time - lastJumpTime >= jumpCooldown)
        {
            animator.SetTrigger("Jumping");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            lastJumpTime = Time.time;
        }

        // Удар по клику мыши
        if (Input.GetMouseButtonDown(0)) // Атака по клику мыши
        {
            animator.SetTrigger("Attack"); // Запускаем анимацию атаки
            Attack(); // Вызываем метод атаки
        }

        // Горизонтальное движение
        HorizontalMove = Input.GetAxisRaw("Horizontal") * speed;
        animator.SetFloat("HorizontalMove", Mathf.Abs(HorizontalMove));

        // Поворот персонажа
        if (HorizontalMove < 0 && FacingRight)
            Flip();
        else if (HorizontalMove > 0 && !FacingRight)
            Flip();
    }

    private void FixedUpdate()
    {
        // Применение горизонтальной скорости
        Vector2 targetVelocity = new Vector2(HorizontalMove * 10f, rb.linearVelocity.y);
        rb.linearVelocity = targetVelocity;

        CheckGround();
    }

    private void Flip()
    {
        FacingRight = !FacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void CheckGround()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            new Vector2(transform.position.x, transform.position.y + checkGroundOffsetY), checkGroundRadius);

        isGrounded = false;

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Ground"))
            {
                isGrounded = true;
                break;
            }
        }
    }

    // Метод для атаки
  // Метод для атаки игрока
private void Attack()
{
    // Проверяем все коллайдеры в зоне атаки
    Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange); // radius = attackRange
    foreach (var enemy in hitEnemies)
    {
        // Проверка, является ли объект врагом
        if (enemy.CompareTag("Enemy"))
        {
            // Наносим урон врагу
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage); // Наносим урон
                Debug.Log("Враг получил урон: " + attackDamage);
            }
        }
    }
}

    // Отображение радиуса атаки в редакторе (для отладки)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); // Отображаем радиус атаки
    }
}
