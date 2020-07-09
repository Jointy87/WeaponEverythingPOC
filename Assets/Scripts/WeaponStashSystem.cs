using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponStashSystem : MonoBehaviour
{
    //Config parameters
    [SerializeField] GameObject[] swords;

    //States
    int swordAmount;

    void Start()
    {
        swordAmount = 1;
    }

    void Update()
    {
        for(int sword = 0; sword <= swords.Length - 1; sword++)
        {
            if(sword < swordAmount)
            {
                swords[sword].SetActive(true);
            }
            else
            {
                swords[sword].SetActive(false);
            }
        }
    }

    public void AddToStash()
    {
        swordAmount++;
    }

    public void RemoveFromStash()
    {
        swordAmount--;
    }
}
