using System.Collections.Generic;

public class LevelNode
{
    public int LevelID;
    public string LevelName;

    public List<LevelNode> Connections;

    public LevelState State;

    public int NodeFloor;
    public LevelDifficulty Difficulty;


    public LevelNode(int levelID, string levelName, int floor, LevelDifficulty difficulty)
    {
        LevelID = levelID;
        LevelName = levelName;
        NodeFloor = floor;
        Difficulty = difficulty;

        Connections = new List<LevelNode>();
        State = LevelState.Available;
    }
}