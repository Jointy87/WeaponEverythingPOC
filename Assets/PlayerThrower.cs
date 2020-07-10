using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThrower : MonoBehaviour
{
    //Config parameters
    [SerializeField] GameObject weaponToThrow;
    [SerializeField] float throwSpeed;
    [SerializeField] Transform throwOrigin;
    [SerializeField] float renderLineLength;

    //Cache
    LineRenderer lr;

    //States
    Vector3 originVector;
    Vector2 aimDirection;

    private void Awake() 
    {
        lr = GetComponentInChildren<LineRenderer>();
    }

    public void Aim(Vector2 aimDirection)
    {
        if(GetComponent<PlayerMover>().FetchGrounded())
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            originVector =
                new Vector3(throwOrigin.position.x, throwOrigin.position.y, throwOrigin.position.z);

            lr.enabled = true;
            Vector3 aimVector = new Vector3(aimDirection.x, aimDirection.y, 0);

            lr.SetPosition(0, originVector);
            lr.SetPosition(1, originVector + aimVector * renderLineLength);
        }
    }

    public void Throw(Vector2 aimDirection)
    {
        GameObject projectile = Instantiate(weaponToThrow, originVector, Quaternion.identity);
        projectile.GetComponent<Rigidbody2D>().velocity = aimDirection * throwSpeed * Time.deltaTime;
        // BUG: Consuming weapons as they spawn
    }

}
