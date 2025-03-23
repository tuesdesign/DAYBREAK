// Written by Daniel Fiuk. 💖

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utility.Simple_Scripts
{
    /// <summary>
    /// Create easily managed and sorted object pools.
    /// </summary>
    public static class SsObjectPool
    {
        private static readonly Dictionary<string, Queue<GameObject>> OBJECT_POOLS = new Dictionary<string, Queue<GameObject>>(); // a dictionary to sort and track objects in their respective object pools
        private const string PARENT_POOL_NAME = "SsObjectPools"; // the name of the object pool parent
        private const string POOL_PREFIX = "SsObjectPool_"; // the prefix to sub object pools to be paired with object IDs
        private static Transform _objectPoolParent; // stores the reference to the object pool parent
        
        #region Pool Management

        /// <summary>
        /// Creates a new object pool with the specified id.
        /// </summary>
        /// <param name="id">The ID of the object pool.</param>
        /// <returns>The Transform of the newly created object pool.</returns>
        private static Transform CreatePool(string id)
        {
            // check if pool already exists with the specified id
            if (OBJECT_POOLS.ContainsKey(id)) return GetPool(id);
            
            // ensure object pool parent exists
            if (!_objectPoolParent) _objectPoolParent = new GameObject(PARENT_POOL_NAME).transform;
            
            // create object pool
            var transform = new GameObject(POOL_PREFIX + id).transform;
            transform.SetParent(_objectPoolParent);
            return transform;
        }

        /// <summary>
        /// Retrieves the object pool with the specified id.
        /// </summary>
        /// <param name="id">The ID of the object pool.</param>
        /// <returns>The transform of the retrieved object pool.</returns>
        private static Transform GetPool(string id)
        {
            // ensure object pool parent exists
            if (!_objectPoolParent) _objectPoolParent = new GameObject(PARENT_POOL_NAME).transform;

            // get object pool
            var pool = _objectPoolParent.Find(POOL_PREFIX + id);
            return pool ? pool : CreatePool(id);
        }
        
        /// <summary>
        /// Flushes the objects in the specified object pool queue.
        /// </summary>
        /// <param name="pool">The specified object pool.</param>
        [Obsolete("Use FlushObjectPool(string id) instead. Using the id will allow for pool management.")]
        private static void FlushObjectPool(Queue<GameObject> pool)
        {
            // destroy all pooled objects
            while (pool.Count > 0)
                Object.Destroy(pool.Dequeue());
        }
        
        // Suppresses warning for Obsolete method
        #pragma warning disable 0618
        
        /// <summary>
        /// Flushes the object pool of the specified id.
        /// </summary>
        /// <param name="id">The ID of the object pool.</param>
        /// <returns>whether the operation was a success.</returns>
        public static bool FlushObjectPool(string id)
        {
            // check if object pool exists
            if (!OBJECT_POOLS.TryGetValue(id, out var pool)) return false;
            
            // flush object pool
            FlushObjectPool(pool);
            
            // remove object pool from the dictionary
            OBJECT_POOLS.Remove(id);

            // flush successful
            return true;
        }
        
        /// <summary>
        /// Flushes all object pools.
        /// </summary>
        public static void FlushObjectPools()
        {
            // flush all object pools
            foreach (var pool in OBJECT_POOLS)
                FlushObjectPool(pool.Value);

            // clear object pool dictionary
            OBJECT_POOLS.Clear();
        }

        #pragma warning restore 0618
        
        /// <summary>
        /// Destroys all objects associated with the object pool.
        /// </summary>
        /// <param name="id">The ID of the object pool.</param>
        public static void DestroyPool(string id)
        {
            // flush object pool
            FlushObjectPool(id);
            
            // destroy object pool parent
            Object.Destroy(_objectPoolParent.Find(id).gameObject);
            
        }
        
        [Tooltip("Destroys all objects associated with all object pools.")]
        public static void DestroyObjectPools()
        {
            // flush all object pools
            FlushObjectPools();
            
            // destroy object pool parent
            Object.Destroy(_objectPoolParent);
        }

        #endregion

        #region Object Handeling

        /// <summary>
        /// Pools the specified object by its ID.
        /// </summary>
        /// <param name="id">The string ID of the object you are pooling.</param>
        /// <param name="obj">The object you are trying to pool.</param>
        public static void PoolObject(string id, GameObject obj)
        {
            // enqueue GameObjects to existing Object Pool if it exists
            if (OBJECT_POOLS.TryGetValue(id, out var value))
            {
                // enqueue object to pool
                value.Enqueue(obj);

                // set object parent to object pool parent
                obj.transform.SetParent(GetPool(id));
            }

            // else create new Object Pool
            else obj.transform.SetParent(CreatePool(id));
            
            // deactivate object
            obj.SetActive(false);
        }
        
        /// <summary>
        /// Retrieves an object from the specified object pool.
        /// </summary>
        /// <param name="id">The string ID of the pooled object you are retrieving.</param>
        /// <param name="prefab">A prefab of the object you are trying to retrieve to be instantiated if the pool is empty.</param>
        /// <returns>An object from the specified pool or a new instance.</returns>
        public static GameObject GetObject(string id, GameObject prefab)
        {
            // try dequeuing from prefab object pool
            if (OBJECT_POOLS.TryGetValue(id, out var pool))
            {
                if (!pool.TryDequeue(out var obj)) 
                    return Object.Instantiate(prefab);
                
                obj.SetActive(true);
                return obj;
            }

            // create a new Object Pool and create an object by refference
            else OBJECT_POOLS.Add(id, new Queue<GameObject>());

            // if no objects are pooled, create a new instance.
            return Object.Instantiate(prefab);
        }
        
        /// <summary>
        /// Retrieves an object from the specified object pool, while also allowing you to set its basic transforms.
        /// </summary>
        /// <param name="id">The string ID of the pooled object you are retrieving.</param>
        /// <param name="prefab">A prefab of the object you are trying to retrieve to be instantiated if the pool is empty.</param>
        /// <param name="position">The position to set the object to when it is retrieved or instantiated.</param>
        /// <param name="rotation">The rotation to set the object to when it is retrieved or instantiated.</param>
        /// <returns>An object from the specified pool or a new instance.</returns>
        public static GameObject GetObject(string id, GameObject prefab, Vector3 position, Quaternion rotation)
        {
            var obj = GetObject(id, prefab);
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            return obj;
        }

        /// <summary>
        /// Retrieves an object from the specified object pool, while also allowing you to set its transforms and parent.
        /// </summary>
        /// <param name="id">The string ID of the pooled object you are retrieving.</param>
        /// <param name="prefab">A prefab of the object you are trying to retrieve to be instantiated if the pool is empty.</param>
        /// <param name="position">The position to set the object to when it is retrieved or instantiated.</param>
        /// <param name="rotation">The rotation to set the object to when it is retrieved or instantiated.</param>
        /// <param name="parent">A transform to set the parent of the retrieved or instantiated object.</param>
        /// <returns>An object from the specified pool or a new instance.</returns>
        public static GameObject GetObject(string id, GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            var obj = GetObject(id, prefab, position, rotation);
            obj.transform.SetParent(parent);
            return obj;
        }

        #endregion
    }
}