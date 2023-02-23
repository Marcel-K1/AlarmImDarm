using UnityEngine;

/*********************************************************************************************
* Project: Alarm Im Darm
* File   : Singleton
* Date   : 15.11.2022
* Author : Levi
*
* This Script is used to make any script into a Singleton by using Inheritance
* 
*********************************************************************************************/

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T instance;

    protected void Awake()
    {
        if (instance == null)
        {
            instance = (T)this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
