using System.Collections.Generic;
using UnityEngine;

public static class ObjectPooler {

    static List<List<GameObject>> listOfListOfPooledObjects = new List<List<GameObject>> ();
    public static void PoolObject (GameObject prefab, int amount, Transform parent) {
        List<GameObject> pooledObjects = new List<GameObject> ();
        for (int i = 0; i < amount; i++) {
            GameObject clone = MonoBehaviour.Instantiate (prefab, parent);
            clone.SetActive (false);
            pooledObjects.Add (clone);
            
        }
        listOfListOfPooledObjects.Add (pooledObjects);
    }

    public static T GetPooledObject<T> () where T : Component {
        foreach (List<GameObject> pooledObjects in listOfListOfPooledObjects) {
            if (pooledObjects[0].GetComponent<T> () == null) continue;
            foreach (var item in pooledObjects) {
                if (!item.activeInHierarchy) return item.GetComponent<T>();
            }

            GameObject clone = MonoBehaviour.Instantiate (pooledObjects[0], pooledObjects[0].transform.parent);
            pooledObjects.Add (clone);
            return clone.GetComponent<T>();
        }
        return null;
    }
}