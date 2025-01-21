using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using Google.Protobuf;
using UnityEngine;

public class NetManager : UnitySingleton<NetManager> {
  private readonly SocketClient client = new();

  private void Awake() {
    client.handleMsg += HandleMsg;
  }

  public void Connect(string host, int port) {
    client.Connect(host, port);
  }

  public bool Connected() {
    return client.Connected();
  }

  public void BeginRead() {
    client.BeginRead();
  }

  private void HandleMsg(int msgId, byte[] msg) {
    // Debug.Log("SocketClient recerive msgId: " + msgId + ".");
    switch (msgId) {
      case 1:
        var playerId = Pb.SyncPlayerId.Parser.ParseFrom(msg);
        GameManager.Instance.StartGame(playerId.PlayerId);
        break;
      case 200:
        var broadCast = Pb.BroadCast.Parser.ParseFrom(msg);
        HandleBroadCast(broadCast);
        break;
      case 201:
        playerId = Pb.SyncPlayerId.Parser.ParseFrom(msg);
        GameManager.Instance.DestroyPlayer(playerId.PlayerId);
        break;
      case 202:
        var players = Pb.SyncPlayers.Parser.ParseFrom(msg);
        foreach (var player in players.Players) {
          Vector3 position = new(player.Position.X, player.Position.Y, player.Position.Z);
          GameManager.Instance.UpdatePosition(player.PlayerId, position, player.Position.V);
        }
        break;
      default:
        // Debug.LogError("SocketClient recerive invalid msgId: " + msgId + "!");
        break;
    }
  }

  private void HandleBroadCast(Pb.BroadCast msg) {
    switch (msg.Type) {
      case 1:
        if (msg.Content == null) {
          // Debug.LogError("SocketClient recerive BroadCast 1 without Content!");
          return;
        }
        if (msg.Content.StartsWith("/emo ")) {
          GameManager.Instance.ShowEmo(msg.PlayerId, msg.Content[5..]);
        } else {
          UIManager.Instance.ShowChat(msg.PlayerId, msg.Content);
        }
        break;
      case 2:
      case 4:
        if (msg.Position == null) {
          // Debug.LogError("SocketClient recerive BroadCast " + msg.Type + " without Position!");
          return;
        }
        Vector3 position = new(msg.Position.X, msg.Position.Y, msg.Position.Z);
        GameManager.Instance.UpdatePosition(msg.PlayerId, position, msg.Position.V);
        break;
      case 3:
        break;
      default:
        // Debug.LogError("SocketClient recerive invalid BroadCast type: " + msg.Type + "!");
        break;
    }
  }

  public void SendMsg(int msgId, IMessage msg) {
    using var memoryStream = new MemoryStream();
    memoryStream.Write(BitConverter.GetBytes(msg.CalculateSize()));
    memoryStream.Write(BitConverter.GetBytes(msgId));
    memoryStream.Write(msg.ToByteArray());
    byte[] data = memoryStream.ToArray();
    client.Write(data);
  }

  private void OnDestroy() {
    client.Close();
  }
}
