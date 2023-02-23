using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class Enemy : MonoBehaviour, IDamageable
{
    public float moveSpeed;
    public float stoppingDistance;
    public float goBack;
    public int health = 100;

    public GameObject Bullet;
    public GameObject Deathdrop;
    public float shootingTime;
    public float timebtwnShooting;

    public Transform player;
    public Transform otherEnemy;
    private Rigidbody2D rb;
    private Vector2 movement;

    [SerializeField]
    private TextMeshPro enemyHealthText;

    [SerializeField] 
    private EnemyListScriptableObject enemyList;


    // Start is called before the first frame update
    void Start()
    {
    
        otherEnemy = GameObject.FindWithTag("Minion").transform;
        
        rb = GetComponent<Rigidbody2D>();
        
        player = GameObject.FindWithTag("Player").transform;
        timebtwnShooting = shootingTime; 
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = player.position - transform.position;
        //berechnet die Aussrichtung des Gegners
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rb.rotation = angle;
        direction.Normalize();
        movement = direction;    
        MoveEnemy();
        Shooting();
        
        enemyHealthText.text = $"{health}";      

        if (health <= 0)            
        {
            AudioManager.instance.PlaySound(ESounds.NPCDeath);

            for (int i = 0; i < enemyList.Minion.Count; i++)
            {
                if (this.gameObject!= null)
                {
                    if (enemyList.Minion[i].transform.position == this.transform.position)
                    {
                        enemyList.Minion.RemoveAt(i);
                        Instantiate(Deathdrop, this.transform.position, Quaternion.identity);
                        Destroy(this.gameObject);
                    }

                }
            }
        }

        AvoidotherEnemy();
    }

    private void MoveEnemy()
    {
        if (Vector2.Distance(transform.position, player.position) > stoppingDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }
        else if (Vector2.Distance(transform.position, player.position) < stoppingDistance && Vector2.Distance(transform.position, player.position) > goBack)
        {
            transform.position = this.transform.position;
        }
        else if (Vector2.Distance(transform.position, player.position) < goBack)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, -moveSpeed * Time.deltaTime);
        }
    }

    private void AvoidotherEnemy()
    {
        if (otherEnemy != null)
        {
            if (Vector2.Distance(transform.position, otherEnemy.position) < goBack)
            {
                transform.position = Vector2.MoveTowards(transform.position, otherEnemy.position, -moveSpeed * Time.deltaTime);
            }
        }
    }

    private void Shooting()
    {
        if (timebtwnShooting <= 0)
        {
            Instantiate(Bullet, transform.position, Quaternion.identity);
            var rnd = Random.Range(0f, 3f);
            timebtwnShooting = shootingTime + rnd;
            AudioManager.instance.PlaySound(ESounds.NPCShoot);
        }
        else
        {
            timebtwnShooting -= Time.deltaTime;
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
    }

}
