using System.Collections;
using UnityEngine;

public enum FruitType { Apple, Banana, Cherry, Kiwi, Melon, Orange, Pineapple, Strawberry }

public class Fruits : MonoBehaviour
{
    [SerializeField] private FruitType fruitType;
    [SerializeField] private GameObject pickupVfx;

    private GameManager gameManager;
    private bool isCollected = false; // Flag to prevent multiple triggers

    private void Start()
    {
        gameManager = GameManager.instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
{
    if (isCollected) return; // ป้องกันการทำงานซ้ำ
    isCollected = true; // กำหนดว่าเก็บผลไม้แล้ว

    Debug.Log("ผลไม้ถูกเก็บโดย: " + collision.name); // ตรวจสอบว่าใครเป็นคนเก็บผลไม้

    Player player = collision.GetComponent<Player>();

    if (player != null)
    {
        // แสดงค่า Health ก่อนเพิ่ม
        Debug.Log("Health ของผู้เล่นก่อนเพิ่ม: " + player.healthSystem.health);

        gameManager.AddFruit();

        // เพิ่มพลังชีวิตให้ผู้เล่น
        if (player.healthSystem != null)
        {
            // ตรวจสอบก่อนว่า Health น้อยกว่า 3 และเพิ่มเพียง 1 หน่วย
            if (player.healthSystem.health < player.healthSystem.numOfHearts)
            {
                player.healthSystem.Heal(1);  // เพิ่มพลังชีวิต 1 หน่วย
            }
        }

        // แสดงค่า Health หลังเพิ่ม
        Debug.Log("Health ของผู้เล่นหลังเพิ่ม: " + player.healthSystem.health);

        // สร้างเอฟเฟกต์ VFX
        if (pickupVfx != null)
        {
            GameObject newFx = Instantiate(pickupVfx, transform.position, Quaternion.identity);
            Destroy(newFx, 1f); // ลบ VFX หลังจาก 1 วินาที
        }

        // ใช้ฟังก์ชัน DelayedDestroy เพื่อลบผลไม้
        StartCoroutine(DelayedDestroy());
    }
}


    private IEnumerator DelayedDestroy()
    {
        yield return null; // Wait for next frame
        Destroy(gameObject);
    }

}
