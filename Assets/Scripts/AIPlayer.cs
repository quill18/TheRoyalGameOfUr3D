using UnityEngine;
using System.Collections.Generic;

public class AIPlayer
{
    public AIPlayer()
    {
        stateManager = GameObject.FindObjectOfType<StateManager>();
    }

    StateManager stateManager;

    virtual public void DoAI()
    {

        // Do the thing for the current stage we're in

        if(stateManager.IsDoneRolling == false)
        {
            // We need to roll the dice!
            DoRoll();
            return;
        }

        if(stateManager.IsDoneClicking == false)
        {
            // We have a die roll, but we need to pick a stone to move
            DoClick();
            return;
        }

    }

    virtual protected void DoRoll()
    {
        GameObject.FindObjectOfType<DiceRoller>().RollTheDice();
    }

    virtual protected void DoClick()
    {
        // Pick a stone to move, then "click" it.


        PlayerStone[] legalStones = GetLegalMoves();

        if(legalStones == null || legalStones.Length == 0)
        {
            // We have no legal moves.  How did we get here?
            // We might still be in a delayed coroutine somewhere. Let's not freak out.
            return;
        }

        // BasicAI simply picks a legal move at random

        PlayerStone pickedStone = PickStoneToMove(legalStones);

        pickedStone.MoveMe();
    }

    virtual protected PlayerStone PickStoneToMove( PlayerStone[] legalStones )
    {
        return legalStones[ Random.Range(0, legalStones.Length) ];
    }


    /// <summary>
    /// Returns a list of stones that can be legally moved
    /// </summary>
    protected PlayerStone[] GetLegalMoves()
    {
        List<PlayerStone> legalStones = new List<PlayerStone>();


        // If we rolled a zero, then we clearly have no legal moves.
        if(stateManager.DiceTotal == 0)
        {
            return legalStones.ToArray();
        }

        // Loop through all of a player's stones
        PlayerStone[] pss = GameObject.FindObjectsOfType<PlayerStone>();

        foreach( PlayerStone ps in pss )
        {
            if(ps.PlayerId == stateManager.CurrentPlayerId)
            {
                if( ps.CanLegallyMoveAhead( stateManager.DiceTotal) )
                {
                    legalStones.Add(ps);
                }
            }
        }

        return legalStones.ToArray();
    }

}

