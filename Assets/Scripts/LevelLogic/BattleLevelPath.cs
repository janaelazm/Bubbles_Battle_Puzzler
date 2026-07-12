using System;
using UnityEngine;

[Serializable]
public class BattleLevelPath
{
    [Header("Info angezeigt")]
    public string pathName;

    [Tooltip("Grün, Gelb ou Rot")]
    public PathDifficulty difficulty;

    [Tooltip("Punkte")]
    public int points;

    [Header("Perk des Levels")]
    public LevelModifier levelModifier;

    [Header("Ziel")]
    public BattleLevelNode nextNode;
}

public enum PathDifficulty
{
    Green,
    Yellow,
    Red
}