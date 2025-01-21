using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraController : MonoBehaviour {
  private Transform selfPlayer = null;
  private Quaternion rotation;
  private float angle = 0;

  public float horizontalDistance;
  public float verticalDistance;
  public float viewAngle;
  public float rotateSpeed;

  private void Update() {
    if (selfPlayer == null) {
      selfPlayer = GameManager.Instance.GetSelfPlayerTransform();
      if (selfPlayer != null) {
        rotation = selfPlayer.rotation;
      }
    }
  }

  private void LateUpdate() {
    if (selfPlayer == null) {
      return;
    }
    transform.SetPositionAndRotation(
        selfPlayer.position
        - Quaternion.AngleAxis(angle, Up()) * Forward() * horizontalDistance
        + Up() * verticalDistance,
        Quaternion.AngleAxis(angle, Up())
        * Quaternion.AngleAxis(viewAngle, Right()) * rotation);
  }

  private Vector3 Up() {
    return rotation * Vector3.up;
  }

  private Vector3 Forward() {
    return rotation * Vector3.forward;
  }

  private Vector3 Right() {
    return rotation * Vector3.right;
  }

  public void Rotate(int direct) {
    angle += direct * rotateSpeed;
  }

  public Quaternion GetQuaternion() {
    return Quaternion.AngleAxis(angle, Up()) * rotation;
  }
}
