using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WeaponEverything.Combat;

namespace WeaponEverything.Core
{
	public class WeaponStashSystem : MonoBehaviour
	{
		//Config parameters
		[SerializeField] GameObject[] swords;

		//States
		int stashSize;
		bool isAlive = true;

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
			if (stashSize == 0)
			{
				isAlive = false;
				FindObjectOfType<PlayerFighter>().Die();
			}
			stashSize--;
		}

		public int FetchStash()
		{
			return stashSize;
		}

		public bool IsAlive()
		{
			return isAlive;
		}
	}
}