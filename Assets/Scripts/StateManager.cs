using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
		
    }

    public int NumberOfPlayers = 2;
    public int CurrentPlayerId = 0;

    public int DiceTotal;

    public bool IsDoneRolling = false;
    public bool IsDoneClicking = false;
    public bool IsDoneAnimating = false;

    public void NewTurn()
    {
        // This is the start of a player's turn.
        // We don't have a roll for them yet.
        IsDoneRolling = false;
        IsDoneClicking = false;
        IsDoneAnimating = false;

        CurrentPlayerId = (CurrentPlayerId + 1) % NumberOfPlayers;
    }

    // Update is called once per frame
    void Update()
    {
		
        // Is the turn done?
        if (IsDoneRolling && IsDoneClicking && IsDoneAnimating)
        {
            Debug.Log("Turn is done!");
            NewTurn();
        }

    }
}
