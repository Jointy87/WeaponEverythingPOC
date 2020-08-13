using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeaponEverything.Core
{
	public class WeaponStashSystem : MonoBehaviour
	{
		//Config parameters
		[SerializeField] GameObject[] swords;

		//States
		int stashSize;
		bool isAlive = true;

		public delegate void PlayerDieDelegate();
		public event PlayerDieDelegate onPlayerDeath;

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
				onPlayerDeath();
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