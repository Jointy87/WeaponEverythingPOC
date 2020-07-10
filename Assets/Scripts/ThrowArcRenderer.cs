using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowArcRenderer : MonoBehaviour
{   
	//Config parameters
	[SerializeField] float velocity;
	[SerializeField] float angle;
	[SerializeField] int resolution = 10;
	
	//Cache
	LineRenderer lr;

	//States
	float gravity;
	float radianAngle;

	private void Awake()
	{
		lr = GetComponent<LineRenderer>();
		gravity = Mathf.Abs(Physics2D.gravity.y);
	}

	private void OnValidate() 
	{
		if(lr != null && Application.isPlaying)
		{
			RenderArc();
		}
	}

	private void Start() 
	{
		RenderArc();
	}

	private void RenderArc()
	{
        lr.positionCount = resolution + 1;
		lr.SetPositions(CalcArcArray());
	}

	private Vector3[] CalcArcArray()
	{
		Vector3[] arcArray = new Vector3[resolution + 1];

		radianAngle = Mathf.Deg2Rad * angle;
		float maxDistance = 
			(velocity * velocity * Mathf.Sin(2 * radianAngle)) / gravity;

		for(int i = 0; i <= resolution; i++)
		{
			float t = (float)i / (float)resolution;
			arcArray[i] = CalcArcPoint(t, maxDistance);
		}

		return arcArray;
	}

	private Vector3 CalcArcPoint(float t, float maxDistance)
	{
		float x = t * maxDistance;
        float y = x * Mathf.Tan(radianAngle) - ((gravity * x * x) / 
			(2 * velocity * velocity * Mathf.Cos(radianAngle) * Mathf.Cos(radianAngle)));

        return new Vector3(x, y); 
	}
}
