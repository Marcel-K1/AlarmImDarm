using UnityEngine;

/*********************************************************************************************
* Project: Alarm Im Darm
* File   : Bullet
* Date   : 15.11.2022
* Author : Levi
*
* This is the Script to handle the interactions from playerBullets
* 
*********************************************************************************************/

public class Bullet : MonoBehaviour
{
    [SerializeField]
    int bulletSpeed;

    [SerializeField]
    int timeUntilDestroy;

    [SerializeField]
    Rigidbody2D rb;

    //stores the damage of the bullet
    int damage;
    public int Damage
    {
        get => damage;
        set => damage = value;
    }

    private void Start()
    {
        //starts the timer to destroy the bullet
        Destroy(gameObject, timeUntilDestroy);
    }

    //shoots the bullet
    public void ShootBullet()
    {
        rb.AddRelativeForce(Vector2.up * bulletSpeed, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //destroy the bullet on collision and does damage
        switch (collision.gameObject.tag)
        {
            case "Enemy":
                collision.gameObject.GetComponent<IDamageable>().TakeDamage(damage);
                break;
            case "Minion":
                collision.gameObject.GetComponent<IDamageable>().TakeDamage(damage);
                break;
            default:
                break;
        }

        Destroy(gameObject);
    }
}
