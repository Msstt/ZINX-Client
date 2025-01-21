using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ByteBuffer {
  private int readPos = 0;
  private int writePos = 0;
  private readonly int maxSize;
  private readonly byte[] buffer;

  public ByteBuffer(int bufferSize) {
    maxSize = bufferSize;
    buffer = new byte[bufferSize];
  }

  public void Write(byte[] array, int len) {
    lock (buffer) {
      for (int i = 0; i < len; i++) {
        buffer[(writePos + i) % maxSize] = array[i];
      }
      writePos += len;
    }
  }

  private int RemainCount() {
    return writePos - readPos;
  }

  private int ReadUint32() {
    byte[] temp = new byte[4];
    for (int i = 0; i < 4; i++) {
      temp[i] = buffer[(readPos + i) % maxSize];
    }
    return System.BitConverter.ToInt32(temp, 0);
  }

  private byte[] ReadBytes(int len) {
    byte[] array = new byte[len];
    for (int i = 0; i < len; i++) {
      array[i] = buffer[(readPos + i) % maxSize];
    }
    return array;
  }

  public bool CanRead() {
    lock (buffer) {
      if (RemainCount() < 4) {
        return false;
      }
      int len = ReadUint32();
      return RemainCount() >= len + 8;
    }
  }

  public void Read(out int msgId, out byte[] array) {
    lock (buffer) {
      if (!CanRead()) {
        msgId = -1;
        array = null;
        return;
      }
      int len = ReadUint32();
      readPos += 4;
      msgId = ReadUint32();
      readPos += 4;
      array = ReadBytes(len);
      readPos += len;
    }
  }
}
