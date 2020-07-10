using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponStashSystem : MonoBehaviour
{
    //Config parameters
    [SerializeField] GameObject[] swords;

    //States
    int stashSize;

    void Start()
    {
        stashSize = 1;
    }

    void Update()
    {
        DisplayWeapons();
    }

    private void DisplayWeapons()
    {
        for (int sword = 0; sword <= swords.Length - 1; sword++)
        {
            if (sword < stashSize)
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
        stashSize++;
    }

    public void RemoveFromStash()
    {
        if(stashSize == 0)
        {
            FindObjectOfType<PlayerFighter>().Die();
        }
        stashSize--;
    }

    public int FetchStash()
    {
        return stashSize;
    }
}
