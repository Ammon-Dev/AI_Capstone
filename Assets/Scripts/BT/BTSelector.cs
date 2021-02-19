using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTSelector : BTNode
{

    protected List<BTNode> myNodes = new List<BTNode>();

    public BTSelector(List<BTNode> Nodes)
    {
        myNodes = Nodes;
    }

    public override BTNodeStates Eval()
    {

        foreach (BTNode node in myNodes)
        {
            switch (node.Eval())
            {
                case BTNodeStates.FAILURE:
                    continue;

                case BTNodeStates.SUCCESS:
                    currentNodeState = BTNodeStates.SUCCESS;
                    return currentNodeState;

                default:
                    continue;
            }
        }
        currentNodeState = BTNodeStates.FAILURE;
        
        return currentNodeState;


    }
}
