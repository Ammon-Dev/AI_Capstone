using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToEnemyTask : BTNode
{
    BTUnitManager btManager;

    public MoveToEnemyTask(BTUnitManager _btManager)
    {
        btManager = _btManager;
    }

    public override BTNodeStates Eval()
    {
        Unit activeUnit = btManager.activeUnit;

        activeUnit.unitState = Unit.state.Moving;

        if (btManager.moveUp())
        {
            activeUnit.unitState = Unit.state.Idle;
            Debug.Log("Success");


            return BTNodeStates.SUCCESS;
        }
        else
        {
            activeUnit.unitState = Unit.state.Idle;
            Debug.Log("Failure");


            return BTNodeStates.FAILURE;
        }
        
    }
}

