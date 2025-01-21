using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class CatControllor : MonoBehaviour {
  static readonly HashSet<string> emos = new() { "Hi", "Angry" };
  private CharacterController controller;
  private Animator animator;
  private Quaternion rotation;

  [HideInInspector]
  public SkinnedMeshRenderer meshRenderer;
  private Coroutine coroutine = null;
  private bool walking = false;
  private string emo = null;

  public float speed;

  [Header("表情")]
  public Material idle;
  public Material hi;
  public Material angry;

  [HideInInspector]
  public bool emoing = false;

  private void Awake() {
    controller = GetComponent<CharacterController>();
    animator = GetComponent<Animator>();
    meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
  }

  private void Start() {
    rotation = transform.rotation;
  }

  private void Update() {
    if (emo != null) {
      if (!emos.Contains(emo) || emoing) {
        return;
      }
      animator.SetTrigger(emo);
      if (meshRenderer == null) {
        return;
      }

      Material[] materials = meshRenderer.materials;
      if (materials == null || materials.Length < 2) {
        return;
      }
      switch (emo) {
        case "Hi":
          materials[1] = hi;
          break;
        case "Angry":
          materials[1] = angry;
          break;
      }
      meshRenderer.materials = materials;
      emo = null;
    }
  }

  private void LateUpdate() {
    SetAnimator();
    walking = false;
  }

  private void SetAnimator() {
    animator.SetBool("Walk", walking);
  }

  public void UpdatePosition(Vector3 pos, float heading) {
    if (coroutine != null) {
      StopCoroutine(coroutine);
    }
    coroutine = StartCoroutine(MoveCoroutine(pos, heading));
  }

  private IEnumerator MoveCoroutine(Vector3 pos, float heading) {
    while (Mathf.Abs(pos.x - transform.position.x) >= 0.01f ||
            Mathf.Abs(pos.y - transform.position.y) >= 0.1f ||
            Mathf.Abs(pos.z - transform.position.z) >= 0.01f) {
      Vector3 direct = pos - transform.position;
      direct.y = 0;
      direct.Normalize();
      SimpleMove(direct);
      yield return null;
    }
    transform.rotation = Quaternion.Euler(0f, heading, 0f);
  }

  private void SimpleMove(Vector3 direct) {
    if (emoing) {
      return;
    }
    walking = true;
    transform.LookAt(transform.position + direct);
    controller.SimpleMove(speed * direct);
  }

  public void Move(Vector3 direct) {
    SimpleMove(rotation * direct);
  }

  public void Rotate(Quaternion rotation) {
    this.rotation = rotation;
  }

  public void ShowEmo(string emo) {
    this.emo = emo;
  }
}
