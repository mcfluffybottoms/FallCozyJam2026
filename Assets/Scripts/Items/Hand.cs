using System;
using System.Collections.Generic;
using Items.Actions;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class Hand : Item
{
    const float Radius = 1.0f;
    public Hand()
    {
        _name = "Hand";
        _cost = 0;
        _bought = true;
        _rebuyable = false;
    }

    [NonSerialized] GrabAction grabber = new();
    private GrabAction Grabber => grabber ??= new GrabAction();
    public override void Buy() { }

    public override void Press(Vector3 pos)
    {
        Grabber.Pull(pos, 1.0f);
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