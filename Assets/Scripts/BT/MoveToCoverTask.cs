using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToCoverTask : BTNode
{
    BTUnitManager btManager;

    public MoveToCoverTask(BTUnitManager _btManager)
    {
        btManager = _btManager;
    }

    public override BTNodeStates Eval()
    {
        Unit activeUnit = btManager.activeUnit;


        activeUnit.unitState = Unit.state.Moving;

        var cover = btManager.findCover();
        activeUnit.TeleportPlayer(cover.goal, cover.energy);

        return BTNodeStates.SUCCESS;
    }
}
