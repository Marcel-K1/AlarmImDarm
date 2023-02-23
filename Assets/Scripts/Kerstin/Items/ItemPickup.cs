using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*Handles interaction with items
  Made by Kerstin*/
[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer), typeof(ParticleSystem))]
public class ItemPickup : MonoBehaviour
{

    [SerializeField] Item item; //Insert corresponding Scriptable Object Item in inspector

    BoxCollider2D collider;
    ParticleSystem particleSystem;
    ParticleSystemRenderer particleSystemRenderer;
    SpriteRenderer spriteRenderer;
    [SerializeField] Material explosionMaterial;

    public Item GetItem {get => item;}

    private void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
        particleSystem = GetComponent<ParticleSystem>();
        particleSystemRenderer = GetComponent<ParticleSystemRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        ParticleSystemConfiguration();
    }

    private void Start()
    {
        ItemManager.instance.onItemTimerDone += RemoveItem; //Gets called from ItemManager Script
        collider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PickUp();
            AudioManager.instance.PlaySound(ESounds.Collectable);
        }
    }

    void PickUp()
    {
        bool wasPickedUp = ItemManager.instance.Add(item);

        if (wasPickedUp)
        {
            particleSystem.Play();
            spriteRenderer.enabled = false; //Item gets destroyed on RemoveItem()
            collider.enabled = false;
            item.Use();
        }
    }

    void RemoveItem()
    {
        bool wasRemoved = ItemManager.instance.Remove(item);

        if (wasRemoved)
        {
            Destroy(this);
        }
    }

    void ParticleSystemConfiguration()
    {
        var main = particleSystem.main;
        main.duration = 2;
        main.loop = false;
        main.startLifetime = 0.3f;
        main.startSpeed = 10;
        main.startSize = new ParticleSystem.MinMaxCurve(0.25f, 0.5f);
        main.startRotation = new ParticleSystem.MinMaxCurve(0, 360);
        main.playOnAwake = false;

        var emission = particleSystem.emission;
        emission.enabled = true;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 50, 1, 0.010f) });

        var shape = particleSystem.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.5f;

        var limitVelocity = particleSystem.limitVelocityOverLifetime;
        limitVelocity.enabled = true;
        limitVelocity.limit = new ParticleSystem.MinMaxCurve { constant = 3 };
        limitVelocity.dampen = 0.5f;

        var sizeLifetime = particleSystem.sizeOverLifetime;
        sizeLifetime.enabled = true;
        sizeLifetime.size = new ParticleSystem.MinMaxCurve(0.3f, 0);

        particleSystemRenderer.material = explosionMaterial;
    }
}
