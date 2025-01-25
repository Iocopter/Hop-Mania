using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum State { idle, walking, jumping, falling, knocked, dead, hurt }

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;
    private bool canDoubleJump;

    [Header("Knockback")]
    [SerializeField] private float knockbackDuration = 1f;
    [SerializeField] private float hurtForce = 5f;
    [SerializeField] private Vector2 knockbackPower;
    private bool isKnocked;

    [Header("Collision")]
    [SerializeField] private float groundCheckDistnace;
    [SerializeField] private LayerMask whatIsGround;
    private bool isGrounded;
    private bool isAirborne;
    private float xInput;

    private bool facingRight = true;
    private int facingDir = 1;

    private State state = State.idle;

    [SerializeField] private int Fruit = 0;
    [SerializeField] private Text FruitText;

    // เพิ่ม Health Reference
    [Header("Health System")]
    [SerializeField] public Health healthSystem;

    // เพิ่ม UI สำหรับแสดงผลพลังชีวิต
    [Header("UI Elements")]
    [SerializeField] private Text healthText; // Text UI สำหรับแสดงพลังชีวิต
    [SerializeField] private Image[] hearts; // Array ของ Image สำหรับแสดงหัวใจ
    [SerializeField] private Sprite fullHeart; // Sprite ของหัวใจเต็ม
    [SerializeField] private Sprite emptyHeart; // Sprite ของหัวใจว่าง
    public int health = 3; // พลังชีวิตเริ่มต้น
    public int maxHealth = 3; // พลังชีวิตสูงสุด
    public Vector3 startPosition; // ตำแหน่งเริ่มต้น
    private bool isPaused = false;  // สถานะ Pause ของเกม
    private int enemiesKilled = 0; // ตัวแปรสำหรับนับศัตรูที่ถูกเหยียบ


    // ตัวแปรใหม่เพื่อป้องกันการเสียพลังชีวิตซ้ำ
    private bool isCollected;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        startPosition = transform.position; // บันทึกตำแหน่งเริ่มต้น
    }


    private void Update()
    {
        UpdateAirbornStatus();

        if (isKnocked)
            return;

        HandleCollision();
        HandleInput();
        HandleMovement();
        HandleFlip();
        HandleAnimations();
        UpdateUI(); // เรียกเพื่ออัปเดตพลังชีวิต
        UpdateHearts(); // เรียกเพื่ออัปเดตหัวใจ
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ตรวจสอบว่าเก็บผลไม้ไปแล้วหรือยัง
        if (isCollected) return;
        isCollected = true;  // ป้องกันการทำงานซ้ำ

        // ตรวจสอบการชนกับผลไม้
        if (collision.CompareTag("GameCollectable"))
        {
            // ลบผลไม้
            Destroy(collision.gameObject);

            // เพิ่มจำนวนผลไม้
            Fruit += 1;
            Debug.Log("Fruit Count: " + Fruit);

            // แสดงจำนวนผลไม้ใน UI
            if (FruitText != null)
            {
                FruitText.text = Fruit.ToString();
            }

            // เพิ่มหัวใจให้กับผู้เล่นเมื่อเก็บผลไม้
            if (healthSystem != null)
            {
                // ตรวจสอบว่าผู้เล่นมีเลือดน้อยกว่าหัวใจสูงสุด
                if (healthSystem.health < healthSystem.numOfHearts)
                {
                    healthSystem.Heal(1);  // เพิ่มเลือด 1 หน่วย
                    Debug.Log("Health after healing: " + healthSystem.health);
                }
            }
            else
            {
                Debug.LogWarning("Health system is not assigned!");
            }

            // เรียกใช้ Coroutine สำหรับการลบผลไม้ทีหลัง
            StartCoroutine(DelayedDestroy());
        }
    }

    private IEnumerator DelayedDestroy()
    {
        // รอเวลา 1 วินาทีเพื่อทำลายผลไม้ (คุณสามารถปรับเวลาได้)
        yield return new WaitForSeconds(1f);

        // หลังจากเวลาผ่านไปแล้ว ลบผลไม้
        isCollected = false;
    }

    private void OnCollisionEnter2D(Collision2D other)
{
    if (other.gameObject.CompareTag("Enemy"))
    {
        if (state == State.falling) 
        {
            // ทำลายศัตรูและเพิ่มจำนวนที่เหยียบ
            Destroy(other.gameObject);  
            enemiesKilled++;  // เพิ่มจำนวนศัตรูที่ถูกเหยียบ

            Debug.Log("Enemies killed: " + enemiesKilled);

            // เช็คว่าเหยียบศัตรูครบ 2 ตัวหรือยัง
            if (enemiesKilled >= 2)
            {
                TriggerYouWin(); // เรียกฟังก์ชันแสดงหน้า "You Win"
            }
        }
        else
        {
            state = State.hurt;
            Vector2 knockbackDirection = (transform.position.x < other.transform.position.x) ? Vector2.left : Vector2.right;
            Knockback(knockbackDirection);
            anim.SetTrigger("knockback");
        }
    }
}

private void TriggerYouWin()
{
    Debug.Log("You win!");  // ปรากฏใน Console

    // เรียกการแสดงผลหน้า You Win
    GameWinController gameWinController = FindObjectOfType<GameWinController>();
    if (gameWinController != null)
    {
        gameWinController.ShowYouWin(); // เรียกฟังก์ชันแสดงหน้าจอ "You Win"
    }
    else
    {
        Debug.LogError("GameWinController not found!");  // แจ้งข้อผิดพลาดถ้าไม่พบ GameWinController
    }
}



    public void Knockback(Vector2 direction)
    {
        if (isKnocked)
            return;

        // เริ่มต้นการทำ Knockback
        StartCoroutine(KnockbackRoutine());
        anim.SetTrigger("knockback");  // ส่งคำสั่งให้อนิเมชั่นเล่น

        // ตั้งค่าให้ความเร็วในแนวนอนให้เหมาะสมกับทิศทางการเด้ง
        rb.velocity = new Vector2(knockbackPower.x * direction.x, knockbackPower.y);
    }

    private IEnumerator KnockbackRoutine()
    {
        isKnocked = true;

        yield return new WaitForSeconds(knockbackDuration);  // ใช้ knockbackDuration แทนที่ 0.3f

        isKnocked = false;
        state = State.idle;
    }
    private void UpdateAirbornStatus()
    {
        if (isGrounded && isAirborne)
            HandleLanding();

        if (!isGrounded && !isAirborne)
            BecomeAirborne();
    }

    private void BecomeAirborne()
    {
        isAirborne = true;
        state = State.falling;
    }

    private void HandleLanding()
    {
        isAirborne = false;
        canDoubleJump = true;
        state = State.idle;
    }

    private void HandleInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.W))
        {
            JumpButton();
        }
    }

    private void JumpButton()
    {
        if (isGrounded)
        {
            Jump();
            state = State.jumping;
        }
        else if (canDoubleJump)
        {
            DoubleJump();
            state = State.jumping;
        }
    }

    public void TriggerKnockback(int direction)
    {
        // สร้าง Vector2 โดยใช้ทิศทางที่เป็น int มาแปลงเป็น Vector2
        Vector2 knockbackDirection = direction == 1 ? Vector2.right : Vector2.left;
        
        // เรียกใช้ Knockback โดยส่ง Vector2
        Knockback(knockbackDirection);
    }


    private void Jump() => rb.velocity = new Vector2(rb.velocity.x, jumpForce);

    private void DoubleJump()
    {
        canDoubleJump = false;
        rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);
    }

    private void HandleCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistnace, whatIsGround);
    }

    private void HandleAnimations()
    {
        if (state == State.dead)
            return;

        anim.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isGrounded", isGrounded);
    }

    private void HandleMovement()
    {
        // ตรวจสอบว่าอยู่ในสถานะ Knockback หรือไม่ ถ้าอยู่จะไม่ให้เคลื่อนไหว
        if (state == State.knocked)
        {
            rb.velocity = new Vector2(0, rb.velocity.y); // หยุดการเคลื่อนที่ในแนวนอน
            return; // ไม่ทำการเคลื่อนที่ต่อ
        }

        // หากไม่ได้อยู่ในสถานะ Knockback ให้เคลื่อนที่ตาม input
        rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);

        // เปลี่ยนสถานะตามทิศทางการเคลื่อนไหว
        if (Mathf.Abs(xInput) > 0.1f && isGrounded)
        {
            state = State.walking;
        }
        else if (isGrounded)
        {
            state = State.idle;
        }
    }

    private void HandleFlip()
    {
        if (xInput < 0 && facingRight || xInput > 0 && !facingRight)
            Flip();
    }

    private void Flip()
    {
        facingDir = facingDir * -1;
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistnace));
    }

// ฟังก์ชันสำหรับอัปเดตข้อความพลังชีวิต
    private void UpdateUI()
    {
        if (healthText != null && healthSystem != null)
        {
            healthText.text = "Health: " + healthSystem.health.ToString();
        }
    }
    private void TriggerGameOver()
    {
        Debug.Log("Player is dead! Game Over triggered.");
        GameOverController gameOverController = FindObjectOfType<GameOverController>();
        if (gameOverController != null)
        {
            gameOverController.ShowGameOver(); // เรียกฟังก์ชัน ShowGameOver
        }
        else
        {
            Debug.LogError("GameOverController not found!");
        }
    }
    
    public Health GetHealthSystem() 
    {
        return healthSystem;
    }
    

    // ฟังก์ชันสำหรับอัปเดตการแสดงหัวใจ
    private void UpdateHearts()
    {
        if (healthSystem == null)
        {
            Debug.LogError("Health System is not assigned!");
            return;
        }

        if (hearts == null || hearts.Length == 0)
        {
            Debug.LogError("Hearts array is not assigned or empty!");
            return;
        }

        if (healthSystem.numOfHearts <= 0)
        {
            Debug.LogError("numOfHearts is not assigned or invalid!");
            return;
        }

        // อัปเดตหัวใจ
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < healthSystem.health)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            hearts[i].enabled = i < healthSystem.numOfHearts; // ซ่อนหัวใจที่เกินจำนวน
        }

        // ตรวจสอบว่าพลังชีวิตเหลือ 0 หรือไม่
        if (healthSystem.health <= 0)
        {
            TriggerGameOver(); // เรียก Game Over
        }
    }


    private void Start()
    {
        ResetPlayerState();
    }

    public void ResetPlayerState()
    {
        // รีเซ็ตตำแหน่ง
        transform.position = startPosition;

        // รีเซ็ตพลังชีวิต
        health = maxHealth;
        healthSystem.health = maxHealth; // รีเซ็ตค่าใน Health System ด้วย

        // รีเซ็ตสถานะการเคลื่อนไหว
        rb.velocity = Vector2.zero;
        state = State.idle; // ตั้งค่ากลับไปเป็น idle

        // อัปเดต UI และหัวใจ
        UpdateUI();
        UpdateHearts();

        Debug.Log("Player state has been reset.");
    }

    // ฟังก์ชันสำหรับ Pause / Unpause เกม
    public void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            Time.timeScale = 0;  // หยุดเกม
            Debug.Log("Game Paused");
        }
        else
        {
            Time.timeScale = 1;  // เล่นเกมต่อ
            Debug.Log("Game Resumed");
        }
    }

    
}