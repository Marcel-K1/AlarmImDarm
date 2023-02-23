using System;
using UnityEngine;
using NaughtyAttributes;

/*********************************************************************************************
* Project: Alarm Im Darm
* File   : ScriptableInt
* Date   : 15.11.2022
* Author : Levi
*
* This Scriptable is used to store an Integer value
* 
*********************************************************************************************/

[CreateAssetMenu(menuName = "Scriptables/Int"), Serializable]
public class ScriptableInt : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField, InfoBox("Initial Value", EInfoBoxType.Normal)]
    public int initValue;
    
    [ReadOnly, InfoBox("Actual Value", EInfoBoxType.Normal)]
    public int value;

    //before entering playmode
    [Button("Reset value")]
    public void OnAfterDeserialize()
    {
        value = initValue;
    }

    public void OnBeforeSerialize(){}
}
