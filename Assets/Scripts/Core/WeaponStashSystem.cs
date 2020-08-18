using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WeaponEverything.Core
{
	public class WeaponStashSystem : MonoBehaviour
	{
		//Config parameters
		[SerializeField] float containerMaxFill = 10;
		[SerializeField] GameObject containerFiller;
		[SerializeField] GameObject[] charges;

		//States
		int stashSize = 0;
		bool isAlive = true;
		float fillAmount = 0;
		int chargesAmount = 0;

		public delegate void PlayerDieDelegate();
		public event PlayerDieDelegate onPlayerDeath;

		void Start()
		{
			fillAmount = 0;
			chargesAmount = 1;
		}

		private void Update() 
		{
			UpdateFillAmount();
			UpdateChargeAmount();
		}

		public void AddToFill(float amount)
		{
			fillAmount += amount;
			if(fillAmount >= containerMaxFill && chargesAmount != charges.Length)
			{
				chargesAmount++;
				fillAmount -= containerMaxFill;
			}
			else if (fillAmount >= containerMaxFill && chargesAmount == charges.Length)
			{
				fillAmount = containerMaxFill;
			}
			
		}

		private void UpdateFillAmount()
		{
			containerFiller.transform.localScale = 
				new Vector3(1, fillAmount / containerMaxFill, 1);
		}

		private void UpdateChargeAmount()
		{
			for (int chargeIndex = 0; chargeIndex < charges.Length; chargeIndex++)
			{
				if(chargeIndex <= chargesAmount - 1) charges[chargeIndex].SetActive(true);
				else charges[chargeIndex].SetActive(false);
			}
		}

		public void RemoveCharge()
		{
			if (chargesAmount == 0)
			{
				isAlive = false;
				onPlayerDeath();
			}
			else chargesAmount--;
		}

		public float FetchChargeAmount()
		{
			return chargesAmount;
		}

		public bool IsAlive()
		{
			return isAlive;
		}
	}
}