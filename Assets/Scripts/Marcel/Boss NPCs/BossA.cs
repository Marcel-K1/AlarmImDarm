/*********************************************************************************************
* Project: Alarm Im Darm
* File   : BossA
* Date   : 19.10.2022
* Author : Marcel Klein
*
* Derived class for making entity to type BossA
* 
* History:
*    19.10.2022    MK    Created
*********************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BossA : Entity
{
    #region Variables
    [Header("Enemy Stats")]
    [SerializeField]
    private int health = 100;
    [SerializeField]
    private int idleSpeed = 9;
    [SerializeField]
    private int attackSpeed = 14;
    [SerializeField]
    private int damageAmount = 5;
    [SerializeField]
    float shootingDistance = 0.8f;
    [SerializeField]
    private float fireRate = 1f;
    [SerializeField]
    private float nextFireTime;

    [Header("Object References")]
    [SerializeField]
    private GameObject bulletInRage;
    [SerializeField]
    private GameObject bulletInIdle;
    private Collider2D playerCollider = null;
    [SerializeField]
    private TextMeshPro enemyHealthText;
    [SerializeField]
    ScriptableInt playerPoints;
    [SerializeField]
    private GameObject deathDrop;

    [SerializeField]
    private bool wallTrigger;

    private Rigidbody2D rb;
    #endregion

    #region Properties
    public int Health { get => health; set => health = value; }
    public bool WallTrigger { get => wallTrigger; set => wallTrigger = value; }
    public float FireRate { get => fireRate; set => fireRate = value; }
    #endregion

    #region Methods

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        BossManager.Instance.RegisterEntity(this);

    }

    private void Update()
    {
        //Setup for shooting mechanism
        if (health <= 50 && nextFireTime < Time.time)
        {
            playerCollider = Physics2D.OverlapCircle(this.transform.position, AttentionRadius, Mask.value);
            if (playerCollider != null)
                {
                    var playerPos = new Vector2(playerCollider.gameObject.transform.position.x, 
                        playerCollider.gameObject.transform.position.y);
                    var shootingVec = playerPos - new Vector2(transform.position.x, transform.position.y);
                    Shoot(shootingVec, playerPos);
                }

        }
        else if (health > 50 && nextFireTime < Time.time)
        {
            Shoot((Vector2)MovementDirection, MovementDirection);
        }

        //Update Health Text
        enemyHealthText.text = $"{health}";
    }

    private IEnumerator WaitForDeath()
    {

        if (enemyList.Boss.Contains(this.gameObject))
        {
            BossManager.Instance.UnregisterEntity(this);
            enemyList.Boss.Remove(this.gameObject);
            GameManager.Instance.ShowAdvice();
            playerPoints.value += 30;
            this.gameObject.GetComponentInChildren<SpriteRenderer>().gameObject.SetActive(false);
            Instantiate(deathDrop, this.transform.position, Quaternion.identity);
        }
        yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Environment":
                if (this.State != "SetDestinationState")
                {
                    WallTrigger = true;
                }
                break;
            default:
                break;
        }
    }

    private void Shoot(Vector2 shootingVec, Vector2 positionToSpawn)
    {
        AudioManager.instance.PlaySound(ESounds.NPCShoot);

        if (this.InRage)
        {
            Vector2 vec = positionToSpawn;
            BossBulletRage tempBullet = Instantiate(bulletInRage, new Vector2(transform.position.x + vec.normalized.x * shootingDistance,
                                            transform.position.y + vec.normalized.y * shootingDistance), Quaternion.identity).GetComponent<BossBulletRage>();
            var rnd = Random.Range(0f, 2f);
            nextFireTime = Time.time + FireRate + rnd;
            tempBullet.Damage = damageAmount + 5;
            tempBullet.ShootBulletInRage(shootingVec);
        }
        else
        {
            Vector2 vec = positionToSpawn;
            BossBulletIdle tempBullet = Instantiate(bulletInIdle, new Vector2(transform.position.x + vec.normalized.x * shootingDistance,
                                               transform.position.y + vec.normalized.y * shootingDistance), Quaternion.identity).GetComponent<BossBulletIdle>();
            var rnd = Random.Range(0f, 2f);
            nextFireTime = Time.time + FireRate + rnd;
            tempBullet.Damage = damageAmount;
        }
    }

    public override void Move()
    {
        if (health <= 50)
        {
            var angle = Mathf.Atan2(MovementDirection.y, MovementDirection.x) * Mathf.Rad2Deg;
            rb.MovePosition(this.transform.position + (MovementDirection * attackSpeed * Time.deltaTime));
            rb.rotation = angle;
        }
        else
        {

            var angle = Mathf.Atan2(MovementDirection.y, MovementDirection.x) * Mathf.Rad2Deg;
            rb.MovePosition(this.transform.position + (MovementDirection * idleSpeed * Time.deltaTime));
            rb.rotation = angle;
        }
    }

    public override void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0f)
        {
            StartCoroutine(WaitForDeath());
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, AttentionRadius);
    }

    #endregion
}
