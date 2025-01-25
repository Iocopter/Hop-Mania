using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Player player;
        [Header("Fruits Management")]
    public bool fruitsAreRandom;
    public int fruitsCollected;
    private void Awake()
    {
        // Singleton pattern
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    // ฟังก์ชันเพิ่มจำนวนผลไม้
    public void AddFruit() => fruitsCollected++;  // เพิ่มจำนวนผลไม้ที่เก็บได้
    public bool FruitsHaveRandomLook() => fruitsAreRandom;
}
