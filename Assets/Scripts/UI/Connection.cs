using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Connection : MonoBehaviour {
  private TMP_InputField inputHost;
  private TMP_InputField inputPort;
  private float timer;
  public float disableTime;

  private void Awake() {
    GameManager gameManager = GameManager.Instance;
    NetManager netManager = NetManager.Instance;
    ObjectPool objectPool = ObjectPool.Instance;

    TMP_InputField[] inputFields = GetComponentsInChildren<TMP_InputField>();
    if (inputFields.Length < 2) {
      Debug.LogError("UI ip input fields miss!");
      return;
    }
    inputHost = inputFields[0];
    inputPort = inputFields[1];

    Button button = GetComponentInChildren<Button>();
    if (button == null) {
      Debug.LogError("UI connect button miss!");
      return;
    }
    button.onClick.AddListener(Connect);

    SceneManager.sceneLoaded += Connected;
  }

  private void Update() {
    if (timer > 0) {
      timer -= Time.deltaTime;
    }
    if (!NetManager.Instance.Connected()) {
      return;
    }
    SceneManager.LoadScene(1);
  }

  private void Connected(UnityEngine.SceneManagement.Scene scene, LoadSceneMode sceneMode) {
    if (scene.name != "Map") {
      return;
    }
    GameManager.Instance.Init();
    NetManager.Instance.BeginRead();
    UIManager uiManager = UIManager.Instance;
    InputManager inputManager = InputManager.Instance;
  }

  public void Connect() {
    if (timer > 0) {
      return;
    }
    timer = disableTime;
    NetManager.Instance.Connect(inputHost.text, int.Parse(inputPort.text));
  }
}
