/*********************************************************************************************
* Project: Alarm Im Darm
* File   : BossBulletRage
* Date   : 19.10.2022
* Author : Marcel Klein
*
* Class for the BossBullet behaviour
* 
* History:
*    19.10.2022    MK    Created
*********************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBulletRage : MonoBehaviour, IDamageable
{
    #region Variables

    [SerializeField]
    private int bulletSpeed;

    [SerializeField]
    private float timeUntilDestroy;

    [SerializeField]
    private Rigidbody2D rigidBody;

    [SerializeField]
    private int damage;

    #endregion

    #region Properties
    public int Damage { get => damage; set => damage = value; }
    public int BulletSpeed { get => bulletSpeed; set => bulletSpeed = value; }
    #endregion

    #region Methods
    private void Start()
    {
        Destroy(gameObject, timeUntilDestroy);
    }

    public void ShootBulletInRage(Vector2 shootingDirection)
    {
        rigidBody.AddRelativeForce(shootingDirection * BulletSpeed, ForceMode2D.Impulse);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Player":
                collision.gameObject.GetComponent<IDamageable>().TakeDamage(damage);
                Destroy(gameObject);
                break;
            case "Environment":
                Destroy(gameObject);
                break;
            default:
                break;
        }
    }

    public void TakeDamage(int damage)
    {
        //Nothing happens, just for player bullet collision
    }

    #endregion
}
