using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : UnitySingleton<InputManager> {
  private CameraController cameraController;

  private void Awake() {
    cameraController = Camera.main.GetComponent<CameraController>();
  }

  private void Update() {
    if (UIManager.Instance.DisableInput()) {
      return;
    }
    Move();
    if (Input.GetKeyDown(KeyCode.F)) {
      GameManager.Instance.ShowEmo("Hi");
    }
    if (Input.GetKeyDown(KeyCode.G)) {
      GameManager.Instance.ShowEmo("Angry");
    }
    View();
  }

  private Vector3 GetAxis() {
    Vector3 ret = Vector3.zero;
    if (Input.GetKey(KeyCode.W)) {
      ret.z += 1;
    }
    if (Input.GetKey(KeyCode.S)) {
      ret.z -= 1;
    }
    if (Input.GetKey(KeyCode.A)) {
      ret.x -= 1;
    }
    if (Input.GetKey(KeyCode.D)) {
      ret.x += 1;
    }
    ret.Normalize();
    return ret;
  }

  private void Move() {
    Vector3 direct = GetAxis();
    if (direct != Vector3.zero) {
      Quaternion rotation = cameraController.GetQuaternion();
      GameManager.Instance.MovePlayer(direct, rotation);
    }
  }

  private void View() {
    if (cameraController == null) {
      return;
    }

    int direct = 0;
    if (Input.GetKey(KeyCode.Q)) {
      direct += 1;
    }
    if (Input.GetKey(KeyCode.E)) {
      direct -= 1;
    }
    cameraController.Rotate(direct);
  }
}
