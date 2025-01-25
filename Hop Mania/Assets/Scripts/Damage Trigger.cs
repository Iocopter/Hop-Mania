using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            // คำนวณทิศทาง
            int direction = transform.position.x > player.transform.position.x ? -1 : 1;

            // เรียกใช้ Knockback พร้อมส่งค่าทิศทาง (เปลี่ยนจาก Vector2 เป็น int)
            player.TriggerKnockback(direction);  // ส่ง direction เป็น int
        }
    }


}
