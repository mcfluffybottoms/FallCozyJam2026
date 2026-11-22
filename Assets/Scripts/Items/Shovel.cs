using System;
using Items.Actions;
using UnityEngine;

[Serializable]
public class Shovel : Item
{
    public float Radius { get; private set; } = 1.0f;
    public Shovel(float radius, int cost = 100)
    {
        _name = "Shovel";
        _cost = cost;
        _bought = false;
        _rebuyable = false;
        Radius = radius;
    }
    public override void Buy()
    {
        _bought = true;
    }

    [NonSerialized] GrabAction grabber = new();
    private GrabAction Grabber => grabber ??= new GrabAction();
    public override void Press(Vector3 pos)
    {
        Grabber.Pull(pos, Radius);
    }

    public override void Hold(Vector3 pos)
    {
        Grabber.Move(pos, Radius);
    }

    public override void Release(Vector3 pos)
    {
        Grabber.Release(pos);
    }
}
