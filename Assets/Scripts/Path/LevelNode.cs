using System.Collections.Generic;

public class LevelNode
{
    public int LevelID;
    public string LevelName;

    public List<LevelNode> Connections;

    public LevelState State;

    public int NodeFloor;
    public LevelDifficulty Difficulty;

    public LevelModifier Modifier;

    public bool IsEndNode;


    public LevelNode(int levelID, string levelName, int floor, LevelDifficulty difficulty, LevelModifier modifier = null, bool isEndNode = false)
    {
        LevelID = levelID;
        LevelName = levelName;
        NodeFloor = floor;
        Difficulty = difficulty;
        Modifier = modifier;
        IsEndNode = isEndNode;

        Connections = new List<LevelNode>();
        State = LevelState.Available;
    }
}