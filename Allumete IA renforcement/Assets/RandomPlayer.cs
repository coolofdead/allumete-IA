using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPlayer : IPlayer
{
    public string GetName()
    {
        return "Random Player";
    }

    public int GetStickToPick()
    {
        return Random.Range(1, 4);
    }
}
