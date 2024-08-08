using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetFloatBehaviour : StateMachineBehaviour
{
    public string floatName;
    public bool updateOnStateEnter, updateOnStateExit;
    public bool updateOnStateMachineEnter, updateOnStateMachineExit;
    public float valueOnEnter, valueOnExit;

    // OnStateEnter is called before OnstateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(updateOnStateEnter)
        {
            animator.SetFloat(floatName, valueOnEnter);
        }
    }

    // OnStateExit is called on any state inside this state machine
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(updateOnStateExit)
        {
            animator.SetFloat(floatName, valueOnExit);
        }
    }

    // OnStateMachineEnter is called when entering a state machine via its Entry Node
    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        if(updateOnStateMachineEnter)
            animator.SetFloat(floatName, valueOnEnter);
    }

    // OnStateMachineExit is called when exiting a state machine via its Entry Node
    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        if(updateOnStateMachineExit)
            animator.SetFloat(floatName, valueOnExit);
    }
}
