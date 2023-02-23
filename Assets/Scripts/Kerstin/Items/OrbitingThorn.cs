using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class OrbitingThorn : MonoBehaviour
{
    float damageAmount, lifeDuration;
    float radius, radiusSpeed;
    Vector3 relativeDistance = Vector3.zero;
    [SerializeField] int rotationSpeed;

    GameObject player;

    CircleCollider2D collider;

    ItemPickup itemPickup;

    Vector3 desiredPosition;
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        itemPickup = GetComponent<ItemPickup>();
        //damageAmount = itemPickup.GetItem.amount;
        //lifeDuration = itemPickup.GetItem.duration;

        collider = GetComponent<CircleCollider2D>();
        collider.radius = 0.38f;
        collider.isTrigger = true;
        rotationSpeed = 80;
        radiusSpeed = 0.5f;
    }

    private void OnEnable()
    {
        StartCoroutine(LifeSpan());
        //transform.position = new Vector3(player.transform.position.x + 5, player.transform.position.y + 5, player.transform.position.z);
        //transform.position = (transform.position - player.transform.position).normalized * radius + player.transform.position;
        //transform.SetParent(player.transform);
        if(player == null)
            player = GameObject.FindWithTag("Player");
        transform.position = (transform.position - player.transform.position).normalized * radius + player.transform.position;
    }

    IEnumerator LifeSpan()
    {
        lifeDuration--;
        Debug.Log("Lifespan decreased");
        yield return new WaitForSeconds(1f);
        if (lifeDuration < 1)
            enabled = false;
    }


    private void LateUpdate()
    {
        OrbitPlayer();
    }

    void OrbitPlayer()
    {
        transform.RotateAround(player.transform.position, Vector3.forward, rotationSpeed * Time.deltaTime);
        desiredPosition = (transform.position - player.transform.position).normalized * radius + player.transform.position;
        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, radiusSpeed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<IDamageable>().TakeDamage((int)damageAmount);
        }
    }

    private void OnDisable()
    {
        ItemManager.instance.onItemTimerDone?.Invoke();
    }
}
