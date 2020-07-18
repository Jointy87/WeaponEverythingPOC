using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponEverything.Core;

namespace WeaponEverything.Combat
{
	public class WeaponHandler : MonoBehaviour
	{
		//Config parameters
		[SerializeField] Transform[] attackPoints;
		[SerializeField] float[] attackPointRadius;
		[SerializeField] LayerMask enemyLayers;

		//Cache
		Animator animator;

		private void Awake()
		{
			animator = GetComponent<Animator>();
		}

        public void SetAnimationTrigger(string triggerString)
        {
            animator.SetTrigger(triggerString);
        }

        public void SetAnimatorFloat(string name, float value)
        {
            animator.SetFloat(name, value);
        }

		// Called from animator
		public void AttackHit()
		{
			bool hasHitEnemy = false;

			for (int pointIndex = 0; pointIndex <= attackPoints.Length - 1; pointIndex++)
			{
				Collider2D[] hitEnemies =
				Physics2D.OverlapCircleAll(attackPoints[pointIndex].position,
					attackPointRadius[pointIndex], enemyLayers);

				foreach (Collider2D enemy in hitEnemies)
				{
					enemy.GetComponent<EnemyFighter>().Die();
					hasHitEnemy = true;
				}
			}

			if (!hasHitEnemy) return;
			FindObjectOfType<WeaponStashSystem>().RemoveFromStash();
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.gray;
			for (int pointIndex = 0; pointIndex <= attackPoints.Length - 1; pointIndex++)
			{
				Gizmos.DrawWireSphere(attackPoints[pointIndex].position, attackPointRadius[pointIndex]);
			}
		}
	}
}
