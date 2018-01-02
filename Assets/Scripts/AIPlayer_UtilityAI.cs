using UnityEngine;
using System.Collections.Generic;

public class AIPlayer_UtilityAI : AIPlayer
{
    Dictionary<Tile, float> tileDanger;

    override protected PlayerStone PickStoneToMove( PlayerStone[] legalStones )
    {
        Debug.Log("AIPlayer_UtilityAI");


        if(legalStones == null || legalStones.Length == 0)
        {
            Debug.LogError("Why are we being asked to pick from no stones?");
            return null;
        }

        CalcTileDanger( legalStones[0].PlayerId );

        // For each stone, we rank how good it would be to pick it, where 1 is super-awesome and -1 is horrible.
        PlayerStone bestStone = null;
        float goodness = -Mathf.Infinity;

        foreach(PlayerStone ps in legalStones)
        {
            float g = GetStoneGoodness(ps, ps.CurrentTile, ps.GetTileAhead() );
            if(bestStone == null || g > goodness)
            {
                bestStone = ps;
                goodness = g;
            }
        }

        Debug.Log("Choosen Stone Goodness: " + goodness );
        return bestStone;
    }

    virtual protected void CalcTileDanger( int myPlayerId )
    {
        tileDanger = new Dictionary<Tile, float>();

        Tile[] tiles = GameObject.FindObjectsOfType<Tile>();

        foreach(Tile t in tiles)
        {
            tileDanger[t] = 0;
        }


        PlayerStone[] allStones = GameObject.FindObjectsOfType<PlayerStone>();

        foreach(PlayerStone stone in allStones)
        {
            if(stone.PlayerId == myPlayerId)
                continue;

            // This is an enemy stone, add a "danger" value to tiles in front of it (unless safe)

            for (int i = 1; i <= 4; i++)
            {
                Tile t = stone.GetTileAhead(i);

                if( t == null )
                {
                    // This tile (and subsequent tiles) are invalid, so we can just bail
                    break;
                }

                if( t.IsScoringSpace || t.IsSideline || t.IsRollAgain )
                {
                    // This tile is not a danger zone, so we can ignore it.
                    continue;
                }
                    
                // Okay, this tile is within bopping range of an enemy, so it's dangerous.
                if(i == 2)
                {
                    // 2 tiles is most likely, so most dangerous!
                    tileDanger[t] += 0.3f;
                }
                else
                {
                    tileDanger[t] += 0.2f;
                }
            }
        }
    }

    virtual protected float GetStoneGoodness( PlayerStone stone, Tile currentTile, Tile futureTile )
    {
        float goodness = 0;//Random.Range(-0.1f, 0.1f);

        if( currentTile == null )
        {
            // We aren't on the board yet, and it's always nice to add more to the board to open up more options.
            goodness += 0.20f;
        }

        if( currentTile != null && (currentTile.IsRollAgain == true && currentTile.IsSideline == false) )
        {
            // We are sitting on a roll-again space in the middle.  Let's resist moving just because
            // it blocks the space from our opponent
            goodness -= 0.10f;
        }

        if( futureTile.IsRollAgain == true )
        {
            goodness += 0.50f;
        }

        if( futureTile.PlayerStone != null && futureTile.PlayerStone.PlayerId != stone.PlayerId )
        {
            // There's an enemy stone to bop!
            goodness += 0.50f;
        }

        if( futureTile.IsScoringSpace == true )
        {
            goodness += 0.50f;
        }

        float currentDanger = 0;
        if(currentTile != null)
        {
            currentDanger = tileDanger[currentTile];
        }

        goodness += currentDanger - tileDanger[futureTile];

        // TODO:  Add goodness for tiles that are behind enemies, and therefore likely to contribute to future boppage
        // TODO:  Add goodness for moving a stone forward when we might be blocking friendlies

        return goodness;
    }


}

