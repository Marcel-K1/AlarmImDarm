using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "playerSprite", menuName = "ScriptableObject/playerSpriteSO")]
public class PlayerSpriteScriptableObject : ScriptableObject
{
    [SerializeField]
    private Sprite playerDefaultSprite;
    public Sprite PlayerDefaultSprite { get => playerDefaultSprite;}
}
