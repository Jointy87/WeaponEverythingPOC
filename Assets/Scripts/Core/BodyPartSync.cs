using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeaponEverything.Core
{
	public class BodyPartSync : MonoBehaviour
	{
		//Config parameters
		[SerializeField] SpriteRenderer bodySprite;

		public void SyncBody(float yValue)
		{
			bodySprite.transform.localPosition =
				new Vector3(0, yValue, 0);
		}
	}
}


