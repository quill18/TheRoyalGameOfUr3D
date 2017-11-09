using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceTotalDisplay : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        theStateManager = GameObject.FindObjectOfType<StateManager>();
    }

    StateManager theStateManager;
	
    // Update is called once per frame
    void Update()
    {
        if (theStateManager.IsDoneRolling == false)
        {
            GetComponent<Text>().text = "= ?";
        }
        else
        {
            GetComponent<Text>().text = "= " + theStateManager.DiceTotal;
        }

    }
}
