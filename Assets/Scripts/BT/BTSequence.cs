using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTSequence : BTNode
{

    protected List<BTNode> myNodes = new List<BTNode>();

    public BTSequence(List<BTNode> Nodes)
    {
        myNodes = Nodes;
    }


    public override BTNodeStates Eval()
    {
        bool childRunning = false;

        foreach(BTNode node in myNodes)
        {
            switch (node.Eval())
            {
                case BTNodeStates.FAILURE:
                    return currentNodeState;

                case BTNodeStates.SUCCESS:
                    continue;

                case BTNodeStates.RUNNING:
                    childRunning = true;
                    continue;

                default:
                    return currentNodeState;

            }
        }
        currentNodeState = childRunning ? BTNodeStates.RUNNING : BTNodeStates.SUCCESS;

        return currentNodeState;


    }
}
