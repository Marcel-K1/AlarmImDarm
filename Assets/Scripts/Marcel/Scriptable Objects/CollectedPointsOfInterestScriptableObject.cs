using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CollectedPointsOfInterestScriptableObject", menuName = "ScriptableObject/CollectedPointsOfInterestSO")]
public class CollectedPointsOfInterestScriptableObject : ScriptableObject
{
    public List<string> CollectedPOI;
}
