using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceTotalDisplay : MonoBehaviour {

	// Use this for initialization
	void Start () {
        theDiceRoller = GameObject.FindObjectOfType<DiceRoller>();
	}

    DiceRoller theDiceRoller;
	
	// Update is called once per frame
	void Update () {
        if(theDiceRoller.IsDoneRolling == false)
        {
            GetComponent<Text>().text = "= ?";
        }
        else
        {
            GetComponent<Text>().text = "= " + theDiceRoller.DiceTotal;
        }

	}
}
