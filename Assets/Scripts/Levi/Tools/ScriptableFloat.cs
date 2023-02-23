using System;
using UnityEngine;
using NaughtyAttributes;

/*********************************************************************************************
* Project: Alarm Im Darm
* File   : ScriptableFloat
* Date   : 15.11.2022
* Author : Levi
*
* This Scriptable is used to store a Float value
* 
*********************************************************************************************/

[CreateAssetMenu(menuName = "Scriptables/Float"), Serializable]
public class ScriptableFloat : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField, InfoBox("Initial Value", EInfoBoxType.Normal)]
    public float initValue;

    [ReadOnly, InfoBox("Actual Value", EInfoBoxType.Normal)]
    public float value;

    //before entering playmode
    [Button("Reset value")]
    public void OnAfterDeserialize()
    {
        value = initValue;
    }

    public void OnBeforeSerialize(){}
}
