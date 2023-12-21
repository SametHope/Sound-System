using System.Collections.Generic;

// Original Author Suleyman Yasir Kula
// Modified by Samet Umut Çolak (SametHope)

namespace SametHope.SoundSystem.Pooling
{
    /// <summary>
    /// A static class that provides utilities to manage and interact with object pooling using <see cref="SimplePool{T}"/>.
    /// </summary>
    public static class SimplePoolHelper
    {
        // Pool container for given type
        private static class PoolsOfType<T> where T : class
        {
            // Pool with poolName = null
            private static SimplePool<T> _defaultPool = null;

            // Other pools
            private static Dictionary<string, SimplePool<T>> _namedPools = null;

            /// <summary>
            /// Get a pool of objects of type T with an optional pool name.
            /// If the pool name is null, it returns the default pool for type T.
            /// </summary>
            /// <param name="poolName">Optional name of the pool.</param>
            /// <returns>The pool of objects of type T.</returns>
            public static SimplePool<T> GetPool(string poolName = null)
            {
                if (poolName == null)
                {
                    _defaultPool ??= new SimplePool<T>();
                    return _defaultPool;
                }
                else
                {
                    SimplePool<T> result;

                    if (_namedPools == null)
                    {
                        _namedPools = new Dictionary<string, SimplePool<T>>();
                        result = new SimplePool<T>();
                        _namedPools.Add(poolName, result);
                    }
                    else if (!_namedPools.TryGetValue(poolName, out result))
                    {
                        result = new SimplePool<T>();
                        _namedPools.Add(poolName, result);
                    }

                    return result;
                }
            }
        }

        /// <summary>
        /// Get a pool of objects of type T with an optional pool name.
        /// If the pool name is null, it returns the default pool for type T.
        /// </summary>
        /// <typeparam name="T">The type of objects in the pool.</typeparam>
        /// <param name="poolName">Optional name of the pool.</param>
        /// <returns>The pool of objects of type T.</returns>
        public static SimplePool<T> GetPool<T>(string poolName = null) where T : class
        {
            return PoolsOfType<T>.GetPool(poolName);
        }

        /// <summary>
        /// Push an object back into the pool identified by an optional pool name.
        /// If the pool name is null, it pushes the object to the default pool for type T.
        /// </summary>
        /// <typeparam name="T">The type of objects in the pool.</typeparam>
        /// <param name="obj">The object to be pushed back into the pool.</param>
        /// <param name="poolName">Optional name of the pool.</param>
        public static void Push<T>(T obj, string poolName = null) where T : class
        {
            PoolsOfType<T>.GetPool(poolName).Push(obj);
        }

        /// <summary>
        /// Pop an object from the pool identified by an optional pool name.
        /// If the pool name is null, it pops an object from the default pool for type T.
        /// </summary>
        /// <typeparam name="T">The type of objects in the pool.</typeparam>
        /// <param name="poolName">Optional name of the pool.</param>
        /// <returns>The object popped from the pool.</returns>
        public static T Pop<T>(string poolName = null) where T : class
        {
            return PoolsOfType<T>.GetPool(poolName).Pop();
        }

        /// <summary>
        /// Push an object back into the pool identified by an optional pool name.
        /// If the pool name is null, it pushes the object to the default pool for type T.
        /// This method serves as an extension method for convenience.
        /// </summary>
        /// <typeparam name="T">The type of objects in the pool.</typeparam>
        /// <param name="obj">The object to be pushed back into the pool.</param>
        /// <param name="poolName">Optional name of the pool.</param>
        public static void Pool<T>(this T obj, string poolName = null) where T : class
        {
            PoolsOfType<T>.GetPool(poolName).Push(obj);
        }
    }
}