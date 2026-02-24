using UnityEngine;

public class InputLockBehaviour : StateMachineBehaviour
{
    // Called when the animation state starts
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var controller = animator.GetComponent<ThirdPersonController>();
        if (controller != null)
            controller.LockInputs();
    }

    // Called when the animation state ends
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var controller = animator.GetComponent<ThirdPersonController>();
        if (controller != null)
            controller.UnlockInputs();
    }
}
