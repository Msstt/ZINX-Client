using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test2 : MonoBehaviour {
  public float speed = 3.0F;
  public float rotateSpeed = 3.0F;

  void Update() {
    CharacterController controller = GetComponent<CharacterController>();

    // Rotate around y - axis
    // transform.Rotate(0, Input.GetAxis("Horizontal") * rotateSpeed, 0);

    // Move forward / backward
    // Vector3 forward = transform.TransformDirection(Vector3.forward);
    Vector3 dir = new(Input.GetAxisRaw("Vertical"), 0, Input.GetAxisRaw("Horizontal"));
    float curSpeed = speed;
    controller.SimpleMove(dir * curSpeed);
  }
}
