using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttackPoints : MonoBehaviour
{
	//Config parameters
	[SerializeField] Transform[] attackPoints;
	[SerializeField] float[] attackPointRadius;

	public Transform[] FetchAttackPoints()
	{
		return attackPoints;
	}

	public float[] FetchAttackPointRadius()
	{
		return attackPointRadius;
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
