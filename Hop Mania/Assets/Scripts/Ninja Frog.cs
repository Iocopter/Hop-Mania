using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaFrog : MonoBehaviour
{
    [SerializeField] private float leftCap;
    [SerializeField] private float rightCap;

    [SerializeField] private float jumpLength = 2;
    [SerializeField] private float jumpHeight = 2;
    [SerializeField] private LayerMask ground;

    [SerializeField] private int damage = 1;
    [SerializeField] private float attackCooldown = 1f;

    private Collider2D coll;
    private Rigidbody2D rb;
    private bool facingLeft = true;
    private bool canAttack = true;

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Patrol();
    }

    private void Patrol()
    {
        if (facingLeft)
        {
            if (transform.position.x > leftCap)
            {
                if (transform.localScale.x != 1)
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }

                if (coll.IsTouchingLayers(ground))
                {
                    rb.velocity = new Vector2(-jumpLength, jumpHeight);
                }
            }
            else
            {
                facingLeft = false;
            }
        }
        else
        {
            if (transform.position.x < rightCap)
            {
                if (transform.localScale.x != -1)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                }

                if (coll.IsTouchingLayers(ground))
                {
                    rb.velocity = new Vector2(jumpLength, jumpHeight);
                }
            }
            else
            {
                facingLeft = true;
            }
        }
    }

private void OnCollisionEnter2D(Collision2D collision)
{
    if (collision.gameObject.CompareTag("Player"))
    {
        Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

        if (playerRb != null && playerRb.velocity.y <= 0)  // ตรวจสอบว่าผู้เล่นตกลงมาหรือไม่
        {
            float playerBottom = collision.collider.bounds.min.y;
            float enemyTop = coll.bounds.max.y;

            if (playerBottom > enemyTop)  // ผู้เล่นเหยียบศัตรูจากด้านบน
            {
                Destroy(gameObject); // ทำลาย NinjaFrog
                Debug.Log("Player stomped on NinjaFrog. NinjaFrog destroyed.");

                // เพิ่มแรงเด้งให้ผู้เล่น
                playerRb.velocity = new Vector2(playerRb.velocity.x, 10f); // ปรับพลังเด้งในแกน Y

                return; // หยุดฟังก์ชันเมื่อผู้เล่นเหยียบ
            }
        }

        // ถ้าผู้เล่นไม่ได้เหยียบศัตรู NinjaFrog จะโจมตี
        if (canAttack)
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage, playerHealth.isInvincible);
                Debug.Log("Player hit by NinjaFrog. Health reduced by " + damage);
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
}
