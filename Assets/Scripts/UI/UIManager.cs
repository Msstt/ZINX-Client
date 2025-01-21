using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : UnitySingleton<UIManager> {
  private TextMeshProUGUI showText;
  private TMP_InputField inputText;
  private string chat = string.Empty;
  private bool toShow = false;

  private void Awake() {
    GameObject gameObject = GameObject.Find("ChatText");
    if (gameObject == null) {
      Debug.LogError("UI ChatText not found!");
      return;
    }
    showText = gameObject.GetComponentInChildren<TextMeshProUGUI>();
    if (showText == null) {
      Debug.LogError("UI ChatText not found!");
      return;
    }

    gameObject = GameObject.Find("InputText");
    if (gameObject == null) {
      Debug.LogError("UI InputText not found!");
      return;
    }
    inputText = gameObject.GetComponent<TMP_InputField>();
    if (inputText == null) {
      Debug.LogError("UI InputText not found!");
      return;
    }

    gameObject = GameObject.Find("SendButton");
    if (gameObject == null) {
      Debug.LogError("UI SendButton not found!");
      return;
    }
    if (!gameObject.TryGetComponent<Button>(out var sendButton)) {
      Debug.LogError("UI SendButton not found!");
      return;
    }
    sendButton.onClick.AddListener(SendChat);
  }

  private void Update() {
    if (toShow) {
      showText.text = chat;
      toShow = false;
    }
  }

  private void SendChat() {
    if (inputText == null || inputText.text == string.Empty) {
      return;
    }
    Pb.Talk msg = new() {
      Content = inputText.text
    };
    NetManager.Instance.SendMsg(2, msg);
    inputText.text = "";
  }

  public void ShowChat(int playerId, string content) {
    if (showText == null) {
      return;
    }
    chat += "Player_" + playerId + ": " + content + "\n";
    toShow = true;
  }

  public bool DisableInput() {
    return inputText.isFocused;
  }
}
