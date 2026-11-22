using UnityEngine;

public static class LevelGlobalConfig
{
    public static int WeedLayer = 1 << 7;
    public static int FlowerLayer = 1 << 6;
    public static int WeedFlowerLayer = WeedLayer | FlowerLayer;
}