using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {
  void Start() {
    NetManager net = NetManager.Instance;
    GameManager gameManager = GameManager.Instance;
    InputManager inputManager = InputManager.Instance;
    UIManager uiManager = UIManager.Instance;
    // net.Connect();
  }

  private void Update() {
  }
}
