/*********************************************************************************************
* Project: Alarm Im Darm
* File   : BossBulletIdle
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

public class BossBulletIdle : MonoBehaviour
{
    #region Variables
    public float speed;

    private Transform player;

    private Vector2 target;

    [SerializeField]
    public int Damage = 5;
    [SerializeField]
    private float timeUntilDestroy;

    #endregion

    #region Methods
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Destroy(gameObject, timeUntilDestroy);
    }

    void Update()
    {
        target = new Vector2(player.position.x, player.position.y);

        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (transform.position.x == target.x && transform.position.y == target.y)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<IDamageable>().TakeDamage(Damage);
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Environment"))
        {
            Destroy(gameObject);
        }
    }

    #endregion
}
