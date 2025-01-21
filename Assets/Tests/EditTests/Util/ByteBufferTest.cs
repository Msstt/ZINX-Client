using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ByteBufferTest {
  [Test]
  public void BasicTest() {
    byte[] array = new byte[] { 8, 0, 0, 0, 3, 0, 0, 0 };
    ByteBuffer buffer = new(1024);
    buffer.Write(array, array.Length);
    Assert.AreEqual(buffer.CanRead(), false);
    buffer.Write(array, array.Length);
    Assert.AreEqual(buffer.CanRead(), true);
    buffer.Read(out int msgId, out byte[] ret);
    Assert.AreEqual(3, msgId);
    CollectionAssert.AreEqual(array, ret);
  }
}
