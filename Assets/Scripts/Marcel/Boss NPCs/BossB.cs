/*********************************************************************************************
* Project: Alarm Im Darm
* File   : BossB
* Date   : 19.10.2022
* Author : Marcel Klein
*
* Derived class for making entity to type BossB
* 
* History:
*    19.10.2022    MK    Created
*********************************************************************************************/


using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BossB : Entity
{
    #region Variables
    [Header("Enemy Stats")]
    [SerializeField]
    private int health = 100;
    [SerializeField]
    private int idleSpeed = 10;
    [SerializeField]
    private float chaseSpeed = 12;
    [SerializeField]
    private int damage = 10;
    [SerializeField]
    private int RageDamage = 30;

    [Header("Object References")]
    private Collider2D playerCollider = null;
    private bool collisionHappened = false;
    private Rigidbody2D rb;
    [SerializeField]
    private TextMeshPro enemyHealthText;
    [SerializeField]
    ScriptableInt playerPoints;
    [SerializeField]
    private bool wallTrigger;
    [SerializeField]
    private GameObject deathDrop;

    #endregion

    #region Properties
    public int Health { get => health; set => health = value; }
    public bool CollisionHappened { get => collisionHappened; set => collisionHappened = value; }
    public Collider2D PlayerCollider { get => playerCollider; set => playerCollider = value; }
    public bool WallTrigger { get => wallTrigger; set => wallTrigger = value; }
    public float ChaseSpeed { get => chaseSpeed; set => chaseSpeed = value; }
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
        //Check for player to enter AttentionRadius and setting State accordingly
        playerCollider = Physics2D.OverlapCircle(this.transform.position, AttentionRadius, Mask.value);

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
            case "Player":
                if (State == "RageState")
                {
                    collision.gameObject.GetComponent<IDamageable>().TakeDamage(RageDamage);
                    StartCoroutine(WaitForDeath());
                }
                if(State == "AttackState")
                {
                    collision.gameObject.GetComponent<IDamageable>().TakeDamage(damage);
                    collisionHappened = true;
                }
                break;
            case "Environment":
                if (State != "AttackState" && State != "SetDestinationState")
                {
                    WallTrigger = true;
                }
                break;
            default:
                break;
        }
    }

    public override void Move()
    {
        if (health <= 50 || State == "AttackState")
        {
            if (playerCollider != null)
            {
                var direction = playerCollider.gameObject.transform.position - this.transform.position;
                var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                rb.rotation = angle;
                rb.MovePosition(this.transform.position + (direction.normalized * ChaseSpeed * Time.deltaTime));
            }
            else
            {
                var angle = Mathf.Atan2(MovementDirection.y, MovementDirection.x) * Mathf.Rad2Deg;
                rb.rotation = angle;
                rb.MovePosition(this.transform.position + (MovementDirection * idleSpeed * Time.deltaTime));
            }

        }
        else
        {
            var angle = Mathf.Atan2(MovementDirection.y, MovementDirection.x) * Mathf.Rad2Deg;
            rb.rotation = angle;
            rb.MovePosition(this.transform.position + (MovementDirection * idleSpeed * Time.deltaTime));
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
