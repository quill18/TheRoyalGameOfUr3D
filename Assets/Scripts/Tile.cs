using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
		
    }

    public Tile[] NextTiles;
    public PlayerStone PlayerStone;
    public bool IsScoringSpace;
    public bool IsRollAgain;
    public bool IsSideline; // Is part of a player's private/safe area
	
    // Update is called once per frame
    void Update()
    {
		
    }
}
