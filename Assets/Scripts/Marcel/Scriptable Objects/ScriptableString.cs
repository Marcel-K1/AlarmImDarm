/*********************************************************************************************
* Project: Alarm Im Darm
* File   : ScriptableString.cs
* Date   : 19.10.2022
* Author : Marcel Klein
*
* Setup for creating a SO of type string
* 
* History:
*    19.10.2022    MK    Created
*********************************************************************************************/

using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptables/String"), System.Serializable]
public class ScriptableString : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField, InfoBox("Initial Value", EInfoBoxType.Normal)]
    public string initValue;

    [ReadOnly, InfoBox("Actual Value", EInfoBoxType.Normal)]
    public string value;

    [Button("Reset value")]
    public void OnAfterDeserialize()
    {
        if (value == "")
        {
            value = initValue;
        }
    }

    public void OnBeforeSerialize() { }
}
