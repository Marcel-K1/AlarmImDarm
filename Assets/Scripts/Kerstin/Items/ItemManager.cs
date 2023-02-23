using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;


/*Handles Item List for further use, handles item logic
  Made by Kerstin*/
public class ItemManager : MonoBehaviour
{
    #region Singleton
    public static ItemManager instance;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogWarning("More than one instance of ItemManager found");
        }

        instance = this;

        DontDestroyOnLoad(this.gameObject);
    }
    #endregion

    public delegate void OnItemTimerDone();
    public OnItemTimerDone onItemTimerDone;
    
    List<Item> items = new List<Item>(); //Used to store items for their duration

    #region List Management
    public bool Add(Item item)
    {
        items.Add(item);
        if (items.Contains(item))
            return true;
        else
            return false;
    }

    public bool Remove (Item item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            return true;
        }
        else
            return false;
    }
    #endregion

    #region Food Stats Increasing
    public void FoodStatsInt(float amount, int duration, ScriptableInt scriptableInt)
    {
        scriptableInt.value += (int)amount; //Replenishing stats
        if(duration > 2)
        {
            StartCoroutine(LifeSpan(amount, duration, scriptableInt, null));
        }
        else if(duration <= 2) //Some items have immediate effect and therefore no lifespan
        {
            onItemTimerDone?.Invoke();
        }
    }

    public void FoodStatsFloat(float amount, int duration, ScriptableFloat scriptableFloat)
    {
        scriptableFloat.value += amount; //Replenishing stats but with float
        if (duration > 2)
        {
            StartCoroutine(LifeSpan(amount, duration, null, scriptableFloat));
        }
        else if (duration <= 2)
        {
            onItemTimerDone?.Invoke();
        }
    }
    #endregion

    #region Lifespan
    IEnumerator LifeSpan(float amount, int duration, ScriptableInt scriptableInt, ScriptableFloat scriptableFloat)
    {
        while (duration > 0)
        {
            duration--;
            yield return new WaitForSeconds(1f);
        }

        if (scriptableInt != null)
        {
            scriptableInt.value -= (int)amount;
        }
        if (scriptableFloat != null)
        {
            scriptableFloat.value -= amount;
        }
        
        onItemTimerDone?.Invoke();
    }
    #endregion
}
