using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : UnitySingleton<GameManager> {
  private Dictionary<int, CatControllor> players = new();
  private GameObject cat;
  private int? selfPlayerId = null;

  private Object mutex = new();
  private Dictionary<int, (Vector3, float)> toUpdatePosition = new();
  private HashSet<int> toDestroy = new();

  private void Awake() {
    cat = Resources.Load<GameObject>("Prefabs/Cat");
    if (cat == null) {
      Debug.LogError("\"Prefabs/Cat\" not found!");
    }
    if (cat.GetComponent<CatControllor>() == null) {
      Debug.LogError("\"Prefabs/Cat/CatControllor\" not found!");
    }
  }

  public void Init() {
    ObjectPool.Instance.Register(cat, 10);
  }

  private void Update() {
    lock (mutex) {
      foreach (var item in toUpdatePosition) {
        int playerId = item.Key;
        (Vector3 pos, float heading) = item.Value;
        if (!players.ContainsKey(playerId)) {
          CreatePlayer(playerId, pos, heading);
        } else {
          if (selfPlayerId.HasValue && playerId == selfPlayerId.Value) {
            continue;
          }
          players[playerId].UpdatePosition(pos, heading);
        }
        Debug.Log("Update " + playerId + "'s position : " + pos + "," + heading + ".");
      }
      toUpdatePosition.Clear();

      foreach (var playerId in toDestroy) {
        if (!players.ContainsKey(playerId)) {
          continue;
        }
        Debug.Log("Player " + playerId + " leave.");
        ObjectPool.Instance.Destroy(players[playerId].gameObject);
        players.Remove(playerId);
      }
      toDestroy.Clear();
    }
  }

  private void CreatePlayer(int playerId, Vector3 pos, float heading) {
    CatControllor player = ObjectPool.Instance.Create(cat, pos, Quaternion.Euler(0f, heading, 0f)).GetComponent<CatControllor>();
    players.Add(playerId, player);
  }

  public void StartGame(int playerId) {
    lock (mutex) {
      selfPlayerId = playerId;
    }
    // Debug.Log("Start game with self player id: " + playerId + ".");
  }

  public void UpdatePosition(int playerId, Vector3 pos, float heading) {
    lock (mutex) {
      if (!toUpdatePosition.ContainsKey(playerId)) {
        toUpdatePosition.Add(playerId, (pos, heading));
      } else {
        toUpdatePosition[playerId] = (pos, heading);
      }
    }
  }

  public void MovePlayer(Vector3 direct, Quaternion rotation) {
    lock (mutex) {
      if (!selfPlayerId.HasValue) {
        return;
      }
      players[selfPlayerId.Value].Rotate(rotation);
      players[selfPlayerId.Value].Move(direct);
    }

    Pb.Position msg = new() {
      X = players[selfPlayerId.Value].transform.position.x,
      Y = players[selfPlayerId.Value].transform.position.y,
      Z = players[selfPlayerId.Value].transform.position.z,
      V = players[selfPlayerId.Value].transform.rotation.eulerAngles.y
    };
    NetManager.Instance.SendMsg(3, msg);
  }

  public void DestroyPlayer(int playerId) {
    lock (mutex) {
      if (selfPlayerId.HasValue && playerId == selfPlayerId.Value) {
        return;
      }
      toDestroy.Add(playerId);
    }
  }

  public void ShowEmo(int playerId, string emo) {
    lock (mutex) {
      if (!players.ContainsKey(playerId)) {
        return;
      }
      players[playerId].ShowEmo(emo);
    }
  }

  public void ShowEmo(string emo) {
    Pb.Talk msg = new() {
      Content = "/emo " + emo
    };
    NetManager.Instance.SendMsg(2, msg);
  }

  public Transform GetSelfPlayerTransform() {
    lock (mutex) {
      if (selfPlayerId.HasValue && players.ContainsKey(selfPlayerId.Value)) {
        return players[selfPlayerId.Value].transform;
      }
    }
    return null;
  }
}
