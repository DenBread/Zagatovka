﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CurvyPath
{
    [System.Serializable]
    public class ObjectPoolItem
    {
        public GameObject objectToPool;
        public bool shouldExpand;
        public int amountToPool;
    }
    public class ObjectPooling : MonoBehaviour
    {
        public static ObjectPooling SharedInstance;
        public List<ObjectPoolItem> itemsToPool;
        public List<GameObject> pooledObjects;

        void Awake()
        {
            SharedInstance = this;
        }

        private void Start()
        {
            pooledObjects = new List<GameObject>();
            foreach (ObjectPoolItem item in itemsToPool)
            {
                for (int i = 0; i < item.amountToPool; i++)
                {
                    GameObject obj = (GameObject)Instantiate(item.objectToPool);
                    obj.SetActive(false);
                    pooledObjects.Add(obj);
                }
            }
        }

        public GameObject GetPooledObjectByTag(string tag)
        {
            for (int i = 0; i < pooledObjects.Count; i++)
            {
                if (pooledObjects[i] != null)
                {
                    if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].tag == tag)
                    {
                        return pooledObjects[i];
                    }
                }
            }

            foreach (ObjectPoolItem item in itemsToPool)
            {
                if (item.objectToPool.tag == tag)
                {
                    if (item.shouldExpand)
                    {
                        GameObject obj = (GameObject)Instantiate(item.objectToPool);
                        obj.SetActive(false);
                        pooledObjects.Add(obj);
                        return obj;
                    }
                }
            }
            return null;
        }
    }
}