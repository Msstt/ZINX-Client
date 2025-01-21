using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class EmoPanel : MonoBehaviour {
  public GameObject emoPanel;

  private void Awake() {
    if (emoPanel == null) {
      return;
    }

    Button button = GetComponent<Button>();
    button.onClick.AddListener(ShowPanel);

    Button[] buttons = emoPanel.GetComponentsInChildren<Button>();
    if (buttons.Length < 2) {
      Debug.LogError("UI emo buttons miss!");
      return;
    }
    buttons[0].onClick.AddListener(Hi);
    buttons[1].onClick.AddListener(Angry);
  }

  private void ShowPanel() {
    if (emoPanel.activeSelf) {
      emoPanel.SetActive(false);
    } else {
      emoPanel.SetActive(true);
    }
  }

  private void Hi() {
    GameManager.Instance.ShowEmo("Hi");
  }

  private void Angry() {
    GameManager.Instance.ShowEmo("Angry");
  }
}
