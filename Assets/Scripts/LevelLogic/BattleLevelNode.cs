using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewBattleLevelNode",
    menuName = "Battle Puzzle/Level Node"
)]
public class BattleLevelNode : ScriptableObject
{
    [Header("Identification")]
    public string nodeId;
    public string displayName;

    [Header("Stand")]
    [Min(0)]
    public int stageIndex;

    [Header("Ende des Pfads")]
    public bool isFinishNode;

    [Header("Verfügbarer Weg")]
    public List<BattleLevelPath> paths = new List<BattleLevelPath>();
}