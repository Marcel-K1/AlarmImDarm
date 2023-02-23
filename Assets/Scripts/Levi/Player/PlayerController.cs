using System;
using System.Collections;
using UnityEngine;
using NaughtyAttributes;

/*********************************************************************************************
* Project: Alarm Im Darm
* File   : PlayerController
* Date   : 15.11.2022
* Author : Levi
*
* This script is used to control the Player and handle saving and loading for the Player
* 
*********************************************************************************************/

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour, IDamageable
{
    #region Declarations
    #region Actions

    //gets called when the player dies
    private static Action eonPlayerDeath;

    //gets called whenever the health changes
    public static Action EOnPlayerHealthChanged;

    #endregion
    #region ScriptableObjects

    [Header("ScriptableObjects"), Header("Float")]
    [SerializeField, Expandable]
    ScriptableFloat speed;
    [SerializeField, Expandable]
    ScriptableFloat shootingCooldown;

    [SerializeField, Expandable, HorizontalLine(1, EColor.Black), Header("Int")]
    ScriptableInt damage;
    [SerializeField, Expandable]
    ScriptableInt health;
    [SerializeField, Expandable]
    ScriptableString playerName;
    [SerializeField, Expandable]
    ScriptableInt playerPoints;

    #endregion

    bool canShoot = true;
    bool canMove = true;

    delegate void shootingTypeDelegate();
    
    shootingTypeDelegate currentShootingType;
    
    [SerializeField]
    float shootingDistance = 2;

    float horizontal;
    float vertical;

    [SerializeField, ShowAssetPreview, HorizontalLine(1, EColor.Black), Header("Prefabs")]
    GameObject bullet;

    [SerializeField, Required]
    Rigidbody2D rb;

    Camera cam;

    public static Action EonPlayerDeath { get => eonPlayerDeath; set => eonPlayerDeath = value; }

    enum EshootingTypes
    {
        Simple,
        Double
    }

    [SerializeField]
    EshootingTypes currentShootingTypeEnum;

    #endregion
    #region Awake

    private void Awake()
    {
        cam = Camera.main;
        switch (currentShootingTypeEnum)
        {
            case EshootingTypes.Simple:
                currentShootingType = SimpleShoot;
                break;
            case EshootingTypes.Double:
                currentShootingType = DoubleShoot;
                break;
            default:
                currentShootingType = SimpleShoot;
                break;
        }
    }

    #endregion
    #region Update/FixedUpdate

    private void Update()
    {
        //for shooting
        if (Input.GetMouseButtonDown(0) && canShoot && !GameManager.Instance.Paused)
        {
            StartCoroutine(ShootCooldown());
            currentShootingType?.Invoke();
        }
    }

    private void FixedUpdate()
    {
        if (!canMove)
            return;

        Movement();
    }

    #endregion
    #region Functions

    //used to move the player
    void Movement()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        rb.velocity = new Vector2(horizontal, vertical).normalized * speed.value;
    }

    //spawns the bullet around the player facing the mouse, then shoots it
    void SimpleShoot()
    {
        AudioManager.instance.PlaySound(ESounds.PlayerShoot);

        Vector2 vec = cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;

        float angle = Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg - 90;

        Bullet tempBullet = Instantiate(bullet,
                                        new Vector2(transform.position.x + vec.normalized.x * shootingDistance,
                                        transform.position.y + vec.normalized.y * shootingDistance),
                                        Quaternion.Euler(0, 0, angle)).GetComponent<Bullet>();
        tempBullet.Damage = damage.value;
        tempBullet.ShootBullet();
    }

    //gets called to take damage
    public void TakeDamage(int damage)
    {
        AudioManager.instance.PlaySound(ESounds.PlayerDamage);
        health.value -= damage;
        EOnPlayerHealthChanged?.Invoke();
        if (health.value <= 0)
        {
            GameManager.Instance.GameOver = true;
            Destroy(gameObject);
        }
    }

    //used to reset the players data to the standardValues
    [Button("Reset all Scriptable Objects")]
    public void ResetScriptables()
    {
        speed.OnAfterDeserialize();
        shootingCooldown.OnAfterDeserialize();
        damage.OnAfterDeserialize();
        health.OnAfterDeserialize();
    }

    //gets called for a doubleShot
    void DoubleShoot()
    {
        StartCoroutine(ShootTwice(.2f));
    }

    //DebugFunction to test taking damage and dying
    [Button("Test getting 5 Damage")]
    void TestDamage()
    {
        TakeDamage(5);
    }

    #endregion
    #region IEnumerators

    //used as cooldown for shooting
    IEnumerator ShootCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(shootingCooldown.value);
        canShoot = true;
    }

    //used for the doubleShot (which didn't end up in the final product)
    IEnumerator ShootTwice(float time)
    {
        SimpleShoot();
        yield return new WaitForSeconds(time);
        SimpleShoot();
    }

    #endregion
    #region OnDrawGizmosSelected

    //used to show the shootingDistance, I put it into the if just to be sure it doesn't compile into the finished product
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(new Vector2(transform.position.x, transform.position.y), shootingDistance);
    }
#endif

    #endregion
    #region Save/Loading

    //used to load the players data
    public void LoadSaveData(string json)
    {
        PlayerSaveData save = JsonUtility.FromJson<PlayerSaveData>(json);
        speed.value = save.speed;
        shootingCooldown.value = save.shootingCooldown;
        damage.value = save.damage;
        health.value = save.health;
        playerName.value = save.name;
        playerPoints.value = save.points;
    }

    //returns the players data to save
    public string GetSaveData()
    {
        PlayerSaveData saveData = new PlayerSaveData(speed.value, shootingCooldown.value, damage.value, health.value, playerName.value, playerPoints.value);
        string save = JsonUtility.ToJson(saveData);
        return save;
    }

    //DebugFunction to test Saving

    [Button]
    void TestSave()
    {
        Debug.Log(GetSaveData());
    }

    #endregion
}

//small class used for saving playerData
class PlayerSaveData
{
    #region Declaration/Properties

    public float speed;
    public float shootingCooldown;
    public int damage;
    public int health;
    public string name;
    public int points;

    #endregion
    #region Constructor

    public PlayerSaveData(float saveSpeed, float saveShootingCooldown, int saveDamage, int saveHealth, string saveName, int savePoints)
    {
        speed = saveSpeed;
        shootingCooldown = saveShootingCooldown;
        damage = saveDamage;
        health = saveHealth;
        name = saveName;
        points = savePoints;
    }

    #endregion
}
