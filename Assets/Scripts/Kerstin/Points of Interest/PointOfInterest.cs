using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Points of Interest logic
  Made by Kerstin*/
[ RequireComponent(typeof(BoxCollider2D), typeof(CircleCollider2D))]
public class PointOfInterest : MonoBehaviour
{
    BoxCollider2D boxCollider;          //used to simulate object
    CircleCollider2D circleCollider;    //used to trigger point gain
    SpriteRenderer spriteRenderer;

    [SerializeField] float discoveringTime = 0.6f;

    public static Action EOnTriggerPOI; //Event to trigger Dr. Fartalot and to get 10 points

    [SerializeField] ScriptableInt points;

    [SerializeField] int pointAmount = 10;

    [SerializeField] string pOIName;

    [SerializeField] string pOIText;

    public string POIName { get => pOIName; set => pOIName = value; }
    public string POIText { get => pOIText; set => pOIText = value; }

    private static event Action<string> pOITextEvent = (pOIText) => { };

    public event Action<string> POITextEvent { add { pOITextEvent += value; } remove { pOITextEvent -= value; } }

    private void Awake()
    {
        pOIName = this.gameObject.name;
        switch (pOIName) //Made by Marcel
        {
            case ("Fellball(Clone)"):
                pOIText = "Heieiei, sie können wohl auch bei Cats mitspielen?!";
                break;
            case ("Handy(Clone)"):
                pOIText = "Ist das, das neue Samsung Galaxy S22?!";
                break;
            case ("Kaktus(Clone)"):
                pOIText = "Mein kleiner grüner Kaktus...Hollari, Hollari, Hollaro!";
                break;
            case ("Papierball(Clone)"):
                pOIText = "Ich mag meine Steuererklärung ja auch nicht, aber...";
                break;
            case ("Ring(Clone)"):
                pOIText = "Ja, ich will! Nicht!";
                break;
            case ("Spritze(Clone)"):
                pOIText = "Oh wie kommt die denn da rein? Ups.";
                break;
            case ("Tamagotchi(Clone)"):
                pOIText = "Haben Sie eine Batterie dabei? Ihr Tamagotchi braucht Strom.";
                break;
            default:
                break;
        }

        boxCollider = GetComponent<BoxCollider2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        boxCollider.isTrigger = false; //just to be sure

        circleCollider.isTrigger = true;
        circleCollider.radius = 5;

        spriteRenderer.color = Color.black;

        spriteRenderer.color = Color.black;

        EOnTriggerPOI += AddPoints;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (!GameManager.Instance.POIData.CollectedPOI.Contains(this.gameObject.name))
            {
                GameManager.Instance.POIData.CollectedPOI.Add(this.gameObject.name);
            }
            pOITextEvent?.Invoke(pOIText);
            EOnTriggerPOI?.Invoke();
            StartCoroutine(WaitForSeconds());
            StartCoroutine(WaitForAdvice());
            StartCoroutine(Uncover());
            AudioManager.instance.PlaySound(ESounds.PointofInterest);
            circleCollider.enabled = false; //disabled to not trigger twice
        }
    }

    private IEnumerator WaitForSeconds()
    {
        yield return new WaitForSeconds(6);
        GameManager.Instance.CurrentPOICollected = true;
    }

    private IEnumerator WaitForAdvice()
    {
        yield return new WaitForSeconds(7);
        GameManager.Instance.ShowAdvice();
    }

    void AddPoints()
    {
        points.value += pointAmount;
    }

    IEnumerator Uncover()
    {
        float tick = 0f;
        while (spriteRenderer.color != Color.white)
        {
            tick += Time.deltaTime * discoveringTime;
            spriteRenderer.color = Color.Lerp(Color.black, Color.white, tick);
            yield return null;
        }
    }
}
