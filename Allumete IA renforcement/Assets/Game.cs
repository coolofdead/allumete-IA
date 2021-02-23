[System.Serializable]
public struct GameState
{
    public int state;
    public int action;
    public int reward;
    public int nextState;
}

public class Game
{
    public const int DEFAULT_NUMBER_STICK = 12;

    public int Stick { private set; get; }

    public Player PlayerTurn { private set; get; }
    public enum Player { Player1, Player2 };

    public Game()
    {
        Reset();
    }

    public GameState PickSticks(int nbStick)
    {
        Stick -= nbStick;

        var gameState = new GameState
        {
            state = Stick + nbStick,
            nextState = 0, // Set as 0, it'll be set once the next player play a move or stay as 0 if the game is over
            action = nbStick,
            reward = IsEnded() == false ? 0 : -1
        };

        PlayerTurn = PlayerTurn == Player.Player1 ? Player.Player2 : Player.Player1;

        return gameState;
    }

    public Player GetWinner()
    {
        return PlayerTurn;
    }

    public bool IsEnded()
    {
        return Stick <= 0;
    }

    public void Reset()
    {
        PlayerTurn = Player.Player1;
        Stick = DEFAULT_NUMBER_STICK;
    }
}
