// Written by Daniel Fiuk. 💖

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
        private static readonly Dictionary<string, ObjectPool> OBJECT_POOLS = new Dictionary<string, ObjectPool>(); // a dictionary to sort and track objects in their respective object pools
        private const string PARENT_POOL_NAME = "SsObjectPools"; // the name of the object pool parent
        private const string POOL_PREFIX = "SsObjectPool_"; // the prefix to sub object pools to be paired with object IDs
        private static Transform _objectPoolParent; // stores the reference to the object pool parent

        private struct ObjectPool
        {
            public readonly Queue<GameObject> Queue;
            public readonly Transform Parent;

            public ObjectPool(Queue<GameObject> queue, Transform parent)
            {
                Queue = queue;
                Parent = parent;
            }
        }

        /// <summary>
        /// Retrieves or creates the Object Pool with the specified id.
        /// </summary>
        /// <param name="id">The ID of the Object Pool.</param>
        /// <returns>The retrieved Object Pool.</returns>
        private static ObjectPool GetPool(string id)
        {
            // check if pool already exists with the specified id
            if (OBJECT_POOLS.TryGetValue(id, value: out var pool)) return pool;

            // ensure object pool parent exists
            if (!_objectPoolParent) _objectPoolParent = new GameObject(PARENT_POOL_NAME).transform;

            // create object pool parent
            var parent = new GameObject(POOL_PREFIX + id).transform;
            parent.SetParent(_objectPoolParent);

            // create object pool
            var newPool = new ObjectPool(new Queue<GameObject>(), parent);
            OBJECT_POOLS.Add(id, newPool);
            return newPool;
        }
        
        /// <summary>
        /// Flushes the objects in the specified object pool queue.
        /// </summary>
        /// <param name="pool">The specified object pool.</param>
        private static void FlushPool(Queue<GameObject> pool)
        {
            // destroy all pooled objects
            while (pool.Count > 0) Object.Destroy(pool.Dequeue());
        }
        
        /// <summary>
        /// Flushes the Object Pool of the specified ID.
        /// </summary>
        /// <param name="id">The ID of the Object Pool.</param>
        public static void FlushPool(string id)
        {
            // check if object pool exists
            if (!OBJECT_POOLS.TryGetValue(id, out var pool)) return;

            // flush object pool
            FlushPool(pool.Queue);
            
            // destroy object pool parent
            Object.Destroy(pool.Parent.gameObject);

            // remove object pool from the dictionary
            OBJECT_POOLS.Remove(id);
        }

        /// <summary>
        /// Flushes all Object Pools.
        /// </summary>
        public static void FlushPools()
        {
            // flush all Object Pools
            foreach (var pool in OBJECT_POOLS) FlushPool(pool.Value.Queue);
            
            // destroy object pool parent
            if (_objectPoolParent) Object.Destroy(_objectPoolParent.gameObject);

            // clear Object Pool dictionary
            OBJECT_POOLS.Clear();
        }

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
                value.Queue.Enqueue(obj);

                // set object parent to object pool parent
                obj.transform.SetParent(GetPool(id).Parent);
            }

            // else create new Object Pool
            else obj.transform.SetParent(GetPool(id).Parent);

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
                // if the pool is empty, create a new instance
                if (!pool.Queue.TryDequeue(out var obj)) return Object.Instantiate(prefab);

                // activate the object and return it
                obj.SetActive(true);
                return obj;
            }

            // create a new Object Pool and create an object by reference
            GetPool(id);
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
            // get the object from the pool
            var obj = GetObject(id, prefab);
            
            // set the object's position and rotation and return it
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
            // get the object from the pool with position and rotation
            var obj = GetObject(id, prefab, position, rotation);
            
            // set the object's parent and return it
            obj.transform.SetParent(parent);
            return obj;
        }
    }
}