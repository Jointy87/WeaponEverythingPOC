using System.Collections.Generic;
using UnityEngine;

namespace WeaponEverything.Combat
{
	[CreateAssetMenu(fileName = "WeaponsInfo", menuName = "Weapon Everything POC/New WeaponsInfo file", order = 0)]
	public class WeaponsInfo : ScriptableObject
	{
		//Config parameters
		[SerializeField] WeaponTypeClass[] weapons;

		[System.Serializable]
		class WeaponTypeClass
		{
			public WeaponType weaponType;
			public WeaponStats weaponStats;
		}

		[System.Serializable]
		class WeaponStats
		{
			public float damagePerHit;
			public WeaponAttackPoints attackPoints;
			public LayerMask enemyLayers;
			public AnimatorOverrideController animatorController;
			public Material weaponMaterial;
		}

		Dictionary<WeaponType, WeaponStats> weaponLookUpTable = null;

		public float FetchWeaponDamagePerHit(WeaponType type)
		{
			BuildLookup();
			return weaponLookUpTable[type].damagePerHit;
		}

		public WeaponAttackPoints FetchAttackPoints(WeaponType type)	
		{
			return weaponLookUpTable[type].attackPoints;
		}

		public AnimatorOverrideController FetchWeaponAnimator(WeaponType type)
		{
			BuildLookup();
			return weaponLookUpTable[type].animatorController;
		}

		public LayerMask FetchEnemyLayer(WeaponType type)
		{
			BuildLookup();
			return weaponLookUpTable[type].enemyLayers;
		}

		public Material FetchWeaponMaterial(WeaponType type)
		{
			BuildLookup();
			return weaponLookUpTable[type].weaponMaterial;
		}

		private void BuildLookup()
		{
			if (weaponLookUpTable != null) return;

			weaponLookUpTable = new Dictionary<WeaponType, WeaponStats>();

			foreach (WeaponTypeClass weapon in weapons)
			{
				weaponLookUpTable[weapon.weaponType] = weapon.weaponStats;
			}
		}
	}
}