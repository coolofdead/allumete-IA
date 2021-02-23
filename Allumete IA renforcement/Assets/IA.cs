using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class IA : IPlayer
{
    private const float LEARNING_RATE = 0.001f;
    private const float NUMBER_GAME_REDUCE_EXPLORATION_RATE = 10;

    [SerializeField] private bool isTraining = true;
    [SerializeField] private float epsilon = 0.999f;

    [SerializeField] private int nbGame, nbVictory;
    public float winrate;

    private Dictionary<int, float> winratesByGamestate = new Dictionary<int, float>(Game.DEFAULT_NUMBER_STICK);
    private List<GameState> currentGameHistory = new List<GameState>();

    private Game currentGame;

    public IA(Game game)
    {
        this.currentGame = game;
    }

    public void PrintWinrateByGamestate()
    {
        Debug.Log("--------------------------------------");

        foreach (KeyValuePair<int, float> keyValuePair in winratesByGamestate.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value))
        {
            Debug.Log(keyValuePair.Key + " | " + keyValuePair.Value);
        }

        Debug.Log("--------------------------------------");
    }

    public void StartTraining()
    {
        isTraining = true;
    }

    public void StopTraining()
    {
        isTraining = false;
    }

    public void ResetGameStats()
    {
        nbGame = 0;
        nbVictory = 0;
    }

    public void AddGameStateToGameHistory(GameState gameState, bool wasWinningMove = false)
    {
        if (wasWinningMove)
            nbVictory++;

        if (!isTraining)
            return;

        // If not first move
        if (currentGameHistory.Count > 0)
        {
            var lastGameState = currentGameHistory.Last(); // Pop last game state
            currentGameHistory.Remove(lastGameState); // Remove it from list (can't modify directly bc it's a struct)

            lastGameState.nextState = gameState.state; // Set the nextState value
            if (wasWinningMove)
                lastGameState.reward = gameState.reward; // Set the nextState value

            currentGameHistory.Add(lastGameState); // Then re insert it
        }
    
        // If it's the winning move then we don't add it to the list bc it's already in there
        if (!wasWinningMove)
        {
            currentGameHistory.Add(gameState);
        }
    }

    public void Train()
    {
        nbGame++;
        winrate = (float)nbVictory / (float)nbGame;

        if (isTraining)
        {
            // Flip current game history
            currentGameHistory.Reverse();

            // Loop over each game states and apply formula
            foreach (GameState gameState in currentGameHistory)
            {
                if (winratesByGamestate.ContainsKey(gameState.state) == false)
                    winratesByGamestate.Add(gameState.state, 0);

                if (winratesByGamestate.ContainsKey(gameState.nextState) == false)
                    winratesByGamestate.Add(gameState.nextState, 0);

                // Store in value function the <state, value> foreach game state
                if (gameState.reward == 0) // Meaning it is the last game state (win or loose)
                {
                    winratesByGamestate[gameState.state] = winratesByGamestate[gameState.state] + LEARNING_RATE * (winratesByGamestate[gameState.nextState] - winratesByGamestate[gameState.state]);
                }
                else
                {
                    winratesByGamestate[gameState.state] = winratesByGamestate[gameState.state] + LEARNING_RATE * ((float)gameState.reward - winratesByGamestate[gameState.state]);
                }
            }

            // Reset current game history
            currentGameHistory.Clear();

            if (nbGame % NUMBER_GAME_REDUCE_EXPLORATION_RATE == 0) // Have play 10 games then upgrade exploitation rate
            {
                epsilon *= 0.996f; // Reduce exploration rate by 0.04%
                epsilon = epsilon < 0.05f ? 0.05f : epsilon; // Don't go lower than 0.05% exploration rate
            }
        }
    }

    private int GetStickByExperience()
    {
        var stickActions = new int[] { 1, 2, 3 };
        int bestAction = 1; // Default one
        float bestActionEstimatedWinrate = Mathf.Infinity;

        foreach (int stickNumber in stickActions)
        {
            if (
                currentGame.Stick - stickNumber > 0 &&
                winratesByGamestate.ContainsKey(currentGame.Stick - stickNumber) &&
                (winratesByGamestate[currentGame.Stick - stickNumber] < bestActionEstimatedWinrate)
            )
            {
                bestAction = stickNumber;
                bestActionEstimatedWinrate = winratesByGamestate[currentGame.Stick - stickNumber];
            }
        }

        return bestAction;
    }

    public int GetStickToPick()
    {
        if (Random.Range(0f, 1f) < epsilon) // Play using random
        {
            return Random.Range(1, 4);
        }
        else // Play using experience
        {
            return GetStickByExperience();
        }
    }

    public string GetName()
    {
        return "IA";
    }
}
