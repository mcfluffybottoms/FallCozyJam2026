using System;
using System.Collections.Generic;
using Items.Actions;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class WaterCan : Item
{
    public int Radius { get; private set; } = 5;
    public int WaterLevel { get; private set; } = 1;

    [NonSerialized] private WaterAction action;
    private WaterAction Action => action ??= new WaterAction();

    public WaterCan()
    {
        _name = "Water Can";
        _cost = 300;
        _bought = false;
        Radius = 5;
        WaterLevel = 1;
    }

    public WaterCan(int radius, int waterLevel, string name = "", int cost = 300)
    {
        _name = ("Water Can " + name).TrimEnd();
        _cost = cost;
        _bought = false;
        Radius = radius;
        WaterLevel = waterLevel;

        action = new();
    }

    public override void Buy()
    {
        _bought = true;
    }

    public override void Press(Vector3 pos)
    {
        Action.Water(pos, Radius);
    }

    public override void Hold(Vector3 pos)
    {
        Action.Water(pos, Radius);
    }

    public override void Release(Vector3 pos)
    {
        Action.StopWater(pos, Radius);
    }
}