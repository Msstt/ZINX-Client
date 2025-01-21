using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmoStatus : StateMachineBehaviour {
  // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
  override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    CatControllor cat = animator.GetComponent<CatControllor>();
    if (cat != null) {
      cat.emoing = true;
    }
  }

  // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
  // override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

  // }

  // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
  override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    CatControllor cat = animator.GetComponent<CatControllor>();
    if (cat == null) {
      return;
    }
    cat.emoing = false;

    Material[] materials = cat.meshRenderer.materials;
    if (materials == null || materials.Length < 2) {
      return;
    }
    materials[1] = cat.idle;
    cat.meshRenderer.materials = materials;
  }

  // OnStateMove is called right after Animator.OnAnimatorMove()
  // override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
  //   // Implement code that processes and affects root motion
  // }

  // OnStateIK is called right after Animator.OnAnimatorIK()
  // override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
  //   // Implement code that sets up animation IK (inverse kinematics)
  // }
}
