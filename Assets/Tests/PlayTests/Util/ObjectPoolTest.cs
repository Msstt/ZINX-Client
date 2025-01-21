using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ObjectPoolTest {
  private ObjectPool pool;

  [SetUp]
  public void Init() {
    pool = new GameObject().AddComponent<ObjectPool>();
  }

  [UnityTest]
  public IEnumerator ObjectPoolTestWithEnumeratorPasses() {
    Assert.NotNull(pool);
    GameObject prefabs = Resources.Load<GameObject>("Prefabs/Cat");
    Assert.NotNull(prefabs);
    pool.Register(prefabs, 2);
    List<GameObject> gameObjects = new() {
      pool.Create(prefabs, new Vector3(0, 0, 0), Quaternion.identity),
      pool.Create(prefabs, new Vector3(2, 0, 0), Quaternion.identity)
    };
    Assert.AreEqual(GameObject.FindObjectsOfType<CatControllor>().Length, 2);
    gameObjects.Add(pool.Create(prefabs, new Vector3(2, 0, 0), Quaternion.identity));
    Assert.AreEqual(GameObject.FindObjectsOfType<CatControllor>().Length, 3);
    pool.Destroy(gameObjects[0]);
    Assert.AreEqual(GameObject.FindObjectsOfType<CatControllor>().Length, 2);
    pool.Destroy(gameObjects[2], 1);
    Assert.AreEqual(GameObject.FindObjectsOfType<CatControllor>().Length, 2);
    yield return new WaitForSeconds(1);
    Assert.AreEqual(GameObject.FindObjectsOfType<CatControllor>().Length, 1);
    yield return null;
  }
}
