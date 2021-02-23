using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartPlayer : IPlayer
{
    private Game game;

    public SmartPlayer(Game game)
    {
        this.game = game;
    }

    public string GetName()
    {
        return "Smart Player";
    }

    public int GetStickToPick()
    {
        switch (game.Stick)
        {
            case 8:
                return 3;
            case 4:
                return 3;
            case 3:
                return 2;
            case 2:
                return 1;
            default:
                return Random.Range(1, 3);
        }
    }

}
