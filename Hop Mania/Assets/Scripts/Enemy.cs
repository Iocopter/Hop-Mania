using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float leftCap;
    [SerializeField] private float rightCap;
    [SerializeField] private float jumpForceX = 5f;
    [SerializeField] private float jumpForceY = 8f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Attack Settings")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackCooldown = 1f;

    private Rigidbody2D rb;
    private Collider2D coll;
    private bool facingLeft = true;
    private bool canAttack = true;
    private bool isDestroyed = false; // ป้องกันการประมวลผลซ้ำ
    

void Start()
{
    rb = GetComponent<Rigidbody2D>();
    coll = GetComponent<Collider2D>();
}


    void Update()
    {
        Patrol();
    }
                
    private void Patrol()
    {
        if (isDestroyed) return; // หยุดการทำงานถ้าศัตรูถูกทำลายแล้ว

        if (facingLeft) // ศัตรูกำลังมุ่งหน้าซ้าย
        {
            if (transform.position.x > leftCap) // ตรวจสอบว่าศัตรูยังไม่เกินขอบซ้าย
            {
                // เปลี่ยนทิศทางการมอง
                if (transform.localScale.x != 1)
                    transform.localScale = new Vector3(1, 1, 1);

                // กระโดดไปทางซ้ายเมื่อแตะพื้น
                if (IsGrounded())
                    rb.velocity = new Vector2(-jumpForceX, jumpForceY);
            }
            else
            {
                facingLeft = false; // เปลี่ยนทิศทางเป็นขวา
            }
        }
        else // ศัตรูกำลังมุ่งหน้าขวา
        {
            if (transform.position.x < rightCap) // ตรวจสอบว่าศัตรูยังไม่เกินขอบขวา
            {
                // เปลี่ยนทิศทางการมอง
                if (transform.localScale.x != -1)
                    transform.localScale = new Vector3(-1, 1, 1);

                // กระโดดไปทางขวาเมื่อแตะพื้น
                if (IsGrounded())
                    rb.velocity = new Vector2(jumpForceX, jumpForceY);
            }
            else
            {
                facingLeft = true; // เปลี่ยนทิศทางเป็นซ้าย
            }
        }
    }

    private bool IsGrounded()
    {
        bool grounded = Physics2D.OverlapBox(coll.bounds.center, coll.bounds.size, 0f, groundLayer);
        Debug.Log("IsGrounded: " + grounded);
        return grounded;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDestroyed) return; // หยุดการทำงานถ้าถูกทำลายแล้ว

        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

            if (playerRb != null && playerRb.velocity.y < 0) // ผู้เล่นกระโดดลงมา
            {
                float playerBottom = collision.collider.bounds.min.y;
                float enemyTop = coll.bounds.max.y - 0.2f; // ลดความสูงเล็กน้อยเพื่อเพิ่มพื้นที่ตรวจสอบ

                if (playerBottom > enemyTop) // ผู้เล่นอยู่เหนือศัตรู
                {
                    isDestroyed = true; // ป้องกันการทำงานซ้ำ
                    Destroy(gameObject); // ทำลายศัตรู
                    Debug.Log("Player stomped on Enemy. Enemy destroyed.");

                    // เพิ่มแรงเด้งให้ผู้เล่น
                    playerRb.velocity = new Vector2(playerRb.velocity.x, 10f);
                    return;
                }
            }

            // ถ้าผู้เล่นไม่ได้เหยียบศัตรู Enemy จะโจมตี
            if (canAttack)
            {
                Health playerHealth = collision.gameObject.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                    Debug.Log("Player hit by Enemy. Health reduced by " + damage);
                }

                StartCoroutine(AttackCooldown());
            }
        }
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(leftCap, transform.position.y, 0), 
                        new Vector3(rightCap, transform.position.y, 0));
    }
}
