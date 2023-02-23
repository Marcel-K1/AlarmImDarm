using UnityEngine;


/*Blueprint for items, also points towards item logic in ItemManager
  Made by Kerstin*/
[CreateAssetMenu(fileName = "New Item", menuName = "Buffs/Item")]
public class Item : ScriptableObject
{
    [SerializeField] new string name = "New Item";                             //If useful, if not will be deleted
    /*[TextArea] [SerializeField] string description = "Enter Item description";*/ //Description for ingame explanations
    [SerializeField] float amount;                                             //By which amount the items effect take place
    [Range(2, 100)] [SerializeField] int duration;                             //Duration of item effect

    [SerializeField] ItemTypes type = ItemTypes.Null;                                                            //Type or functionality of item

    [SerializeField] ScriptableInt playerHealth;
    [SerializeField] ScriptableInt playerDamage;
    [SerializeField] ScriptableFloat playerSpeed;
    [SerializeField] ScriptableFloat playerShootingCooldown;

    public void Use()
    {
        switch (type)
        {
            case ItemTypes.Null:
                Debug.LogError("PLEASE CHOOSE ITEM TYPE OF ITEM " + name);
                break;
            case ItemTypes.FoodHealth:
                ItemManager.instance.FoodStatsInt(amount, duration, playerHealth);
                break;
            case ItemTypes.FoodStrength:
                ItemManager.instance.FoodStatsInt(amount, duration, playerDamage); //FoodStatsInt instead of MutationStats cuz there's no other strength buff
                break;
            case ItemTypes.FoodSpeed:
                ItemManager.instance.FoodStatsFloat(amount, duration, playerSpeed);
                break;
            case ItemTypes.FoodShootingCooldown:
                ItemManager.instance.FoodStatsFloat(-amount, duration, playerShootingCooldown);
                break;
            default:
                Debug.LogWarning("Item Use() default");
                break;
        }
    }
}
