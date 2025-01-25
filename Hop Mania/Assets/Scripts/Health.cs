using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int health = 3;
    public int numOfHearts = 3;

    public bool isInvincible { get; private set; }

    private void Start()
    {
        Debug.Log("Health: " + health + " numOfHearts: " + numOfHearts);
    }

    public void TakeDamage(int damage, bool invincible = false)
    {
        if (invincible)
        {
            Debug.Log("Invincible: No damage taken.");
            return;
        }

        health -= damage;
        Debug.Log("Damage taken: " + damage + ", Remaining health: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    public void HealByCondition()
    {
        if (health == 1)
        {
            health = 2; // ถ้าหัวใจเหลือ 1 ให้เพิ่มเป็น 2
        }
        else if (health == 2)
        {
            health = 3; // ถ้าหัวใจเหลือ 2 ให้เพิ่มเป็น 3
        }

        // ตรวจสอบให้แน่ใจว่าไม่เกินจำนวนหัวใจสูงสุด
        if (health > numOfHearts)
        {
            health = numOfHearts;
        }

        Debug.Log("Health updated. Current health: " + health);
    }

    public void Heal(int amount)
    {
        // ตรวจสอบว่า amount ไม่ใช่ค่าลบ
        if (amount <= 0)
        {
            Debug.LogWarning("Heal amount must be positive!");
            return;
        }

        // แสดงค่า health ก่อนเพิ่ม
        Debug.Log("Health before healing: " + health);

        // เพิ่ม health ตามจำนวนที่กำหนด
        health += amount;

        // ตรวจสอบไม่ให้ health เกินจำนวนหัวใจสูงสุด
        if (health > numOfHearts)
        {
            health = numOfHearts; // ปรับ health ให้อยู่ในขอบเขตของหัวใจสูงสุด
        }

        // แสดงผลหลังการเพิ่ม health
        Debug.Log("Health increased by " + amount + ". Current health: " + health);

        // ป้องกันการแสดงผลที่ไม่เหมาะสม
        if (health > numOfHearts)
        {
            Debug.LogError("Health cannot exceed max hearts!");
            health = numOfHearts; // รีเซ็ตกลับหากมีข้อผิดพลาด
        }
    }

    public void ResetHealth()
    {
        health = numOfHearts;  // รีเซ็ตให้พลังชีวิตเป็นค่าพื้นฐาน (หรือค่าเริ่มต้น)
    }


    private void Die()
    {
        Debug.Log("Player died");
        // Add death logic here
    }
}
