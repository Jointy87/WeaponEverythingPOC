using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.tag == "Player")
        {
            FindObjectOfType<WeaponStashSystem>().AddToStash();
            Destroy(gameObject);
        }
    }
}
