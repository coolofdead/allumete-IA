using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool debug;

    public int playMoreGame = 100;
    public int numberTrainingGame = 10000;

    private Game game;
    [SerializeField] private IA ia1, ia2;

    private void Start()
    {
        game = new Game();
        ia1 = new IA(game);
        ia2 = new IA(game);

        // Train IA
        for (int i = 0; i < numberTrainingGame; i++)
        {
            Play(ia1, ia2);
        }

        ia1.PrintWinrateByGamestate();

        ia1.StopTraining();
        ia1.ResetGameStats();

        PlayMoreGames();
    }

    public void PlayMoreGames()
    {
        for (int i = 0; i < playMoreGame; i++)
        {
            Play(ia1, new RandomPlayer());
        }
    }

    private void Play(IPlayer player1, IPlayer player2)
    {
        game.Reset();

        IPlayer firstPlayer = Random.Range(0f, 1f) > 0.5f ? player1 : player2;
        IPlayer secondPlayer = firstPlayer == player1 ? player2 : player1;

        // Play a game
        while (!game.IsEnded())
        {
            IPlayer player = game.PlayerTurn == Game.Player.Player1 ? firstPlayer : secondPlayer;

            int nbStickPick = player.GetStickToPick();
            var gameState = game.PickSticks(nbStickPick);

            if (debug)
            {
                Debug.Log(player.GetName() + " => pick " + nbStickPick + " | left " + game.Stick);
            }

            if (player is IA)
                (player as IA).AddGameStateToGameHistory(gameState);
        }

        // Get winner
        var winner = game.GetWinner() == Game.Player.Player1 ? firstPlayer : secondPlayer;
        if (winner is IA)
            (winner as IA).AddGameStateToGameHistory(new GameState { state = 0, reward = 1 }, true);

        if (debug)
        {
            Debug.Log(winner.GetName() + " a gagné");
        }

        if (player1 is IA) (player1 as IA).Train();
        if (player2 is IA) (player2 as IA).Train();
    }
}
