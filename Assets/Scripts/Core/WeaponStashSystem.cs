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
		[SerializeField] float flashTime = .3f;
		[SerializeField] ParticleSystem chargeLossVFX;
		[SerializeField] float slomoTime = .3f;
		[SerializeField] float slomoScale = .3f;

		//States
		int stashSize = 0;
		public bool isAlive {get; set;} = true;
		float fillAmount = 0;
		public int chargesAmount {get; private set;} = 0;
		float flashTimer = Mathf.Infinity;
		int chargeToFlash = 0;
		float weaponBreakTimer = Mathf.Infinity;

		public event Action onPlayerDeath;
		public event Action onRemoveCharge;

		public delegate void SwitchWeaponDelegate(int weaponTypeIndex);
		public event SwitchWeaponDelegate onWeaponSwap;

		private void Update() 
		{
			UpdateFillAmount();
			UpdateChargeAmount();
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
				if (chargeIndex <= chargesAmount - 1) charges[chargeIndex].SetActive(true);
				else charges[chargeIndex].SetActive(false);
			}
		}

		public void AddToFill(float amount)
		{
			fillAmount += amount;

			if(fillAmount >= containerMaxFill && chargesAmount != charges.Length)
			{
				if(chargesAmount == 0) onWeaponSwap(1);
				chargesAmount++;
				fillAmount -= containerMaxFill;
			}
			else if (fillAmount >= containerMaxFill && chargesAmount == charges.Length)
			{
				fillAmount = containerMaxFill;
			}
			
		}

		public void RemoveCharge()
		{
			if (chargesAmount == 0)
			{
				isAlive = false;
				onPlayerDeath();
			}
			else
			{
				chargesAmount--;
				StartCoroutine(WeaponBreakSlomo());
				StartCoroutine(StartFlash());
				onRemoveCharge();
			}

			if (chargesAmount == 0) onWeaponSwap(0);
		}

		private IEnumerator StartFlash()
		{
			flashTimer = 0;
			chargeToFlash = chargesAmount;
			TriggerChargeLossVFX();

			while(flashTimer < flashTime)
			{
				flashTimer += Time.deltaTime;
				Color flashColor = new Color(0, 252, 255);
				flashColor.a = 1;
				charges[chargeToFlash].transform.parent.GetComponent<Image>().color = flashColor;
				yield return null;
			}

			Color originalColor = Color.black;
			originalColor.a = 0.274f;
			charges[chargeToFlash].transform.parent.GetComponent<Image>().color = originalColor;
		}

		public IEnumerator WeaponBreakSlomo()
		{
			weaponBreakTimer = 0;

			while (weaponBreakTimer < slomoTime)
			{
				weaponBreakTimer += Time.deltaTime;
				Time.timeScale = slomoScale;
				yield return null;
			}
			Time.timeScale = 1f;
		}

		private void TriggerChargeLossVFX()
		{
			chargeLossVFX.transform.position = charges[chargeToFlash].transform.position;
			chargeLossVFX.Play();
		}
	}
}