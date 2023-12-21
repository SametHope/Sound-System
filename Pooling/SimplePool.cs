using System;
using System.Collections.Generic;
using UObject = UnityEngine.Object;

// Original Author Suleyman Yasir Kula
// Modified by Samet Umut Çolak (SametHope)

namespace SametHope.SoundSystem.Pooling
{
    /// <summary>
    /// A simple generic object pooling class that allows efficient reuse of objects of type T.
    /// </summary>
    /// <typeparam name="T">The type of objects to be pooled.</typeparam>
    public class SimplePool<T> where T : class
    {
        /// <summary>
        /// Objects stored in the pool.
        /// </summary>
        public readonly Stack<T> PoolStack = null;

        /// <summary>
        /// Blueprint (or prefab) to use for object instantiation.
        /// </summary>
        public T Blueprint { get; set; }

        /// <summary>
        /// A function that can be used to override the default object creation logic.
        /// </summary>
        public Func<T, T> CreateFunction;

        /// <summary>
        /// Action that can be used to implement extra logic when objects are pooled.
        /// </summary>
        public Action<T> OnPush;

        /// <summary>
        /// Action that can be used to implement extra logic when objects are fetched from the pool.
        /// </summary>
        public Action<T> OnPop;

        /// <summary>
        /// Creates a new instance of the SimplePool class.
        /// </summary>
        /// <param name="CreateFunction">A function that can be used to override default object creation logic (optional).</param>
        /// <param name="OnPush">An action that can be used to implement extra logic when objects are pooled (optional).</param>
        /// <param name="OnPop">An action that can be used to implement extra logic when objects are fetched from the pool (optional).</param>
        public SimplePool(Func<T, T> CreateFunction = null, Action<T> OnPush = null, Action<T> OnPop = null)
        {
            PoolStack = new Stack<T>();
            this.CreateFunction = CreateFunction;
            this.OnPush = OnPush;
            this.OnPop = OnPop;
        }

        /// <summary>
        /// Creates a new instance of the SimplePool class with a specified blueprint (or prefab).
        /// </summary>
        /// <param name="blueprint">The blueprint to use for object instantiation (optional).</param>
        /// <param name="CreateFunction">A function that can be used to override default object creation logic (optional).</param>
        /// <param name="OnPush">An action that can be used to implement extra logic when objects are pooled (optional).</param>
        /// <param name="OnPop">An action that can be used to implement extra logic when objects are fetched from the pool (optional).</param>
        public SimplePool(T blueprint, Func<T, T> CreateFunction = null, Action<T> OnPush = null, Action<T> OnPop = null) : this(CreateFunction, OnPush, OnPop)
        {
            Blueprint = blueprint;
        }

        /// <summary>
        /// Populate the pool with objects using the default blueprint.
        /// </summary>
        /// <param name="count">The number of instances to populate the pool with.</param>
        /// <returns>True if the population succeeded, false otherwise.</returns>
        public bool Populate(int count)
        {
            return Populate(Blueprint, count);
        }

        /// <summary>
        /// Populate the pool with objects using a specific blueprint.
        /// </summary>
        /// <param name="blueprint">The blueprint to use for object instantiation.</param>
        /// <param name="count">The number of instances to populate the pool with.</param>
        /// <returns>True if the population succeeded, false otherwise.</returns>
        public bool Populate(T blueprint, int count)
        {
            if (count <= 0)
            {
                return true;
            }

            T obj = NewObject(blueprint);
            if (obj == null)
            {
                return false;
            }

            Push(obj);

            for (int i = 1; i < count; i++)
            {
                Push(NewObject(blueprint));
            }

            return true;
        }

        /// <summary>
        /// Fetch an object from the pool.
        /// </summary>
        /// <returns>The fetched object.</returns>
        public T Pop()
        {
            T objToPop;

            if (PoolStack.Count == 0)
            {
                objToPop = NewObject(Blueprint);
            }
            else
            {
                objToPop = PoolStack.Pop();
                while (objToPop == null)
                {
                    if (PoolStack.Count > 0)
                    {
                        objToPop = PoolStack.Pop();
                    }
                    else
                    {
                        objToPop = NewObject(Blueprint);
                        break;
                    }
                }
            }

            OnPop?.Invoke(objToPop);

            return objToPop;
        }

        /// <summary>
        /// Fetch multiple objects from the pool.
        /// </summary>
        /// <param name="count">The number of objects to fetch.</param>
        /// <returns>An array containing the fetched objects.</returns>
        public T[] Pop(int count)
        {
            if (count <= 0)
            {
                return new T[0];
            }

            T[] result = new T[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = Pop();
            }

            return result;
        }

        /// <summary>
        /// Return an object back to the pool.
        /// </summary>
        /// <param name="obj">The object to return to the pool.</param>
        public void Push(T obj)
        {
            if (obj == null)
            {
                return;
            }

            OnPush?.Invoke(obj);

            PoolStack.Push(obj);
        }

        /// <summary>
        /// Return multiple objects back to the pool.
        /// </summary>
        /// <param name="objects">The objects to return to the pool.</param>
        public void Push(IEnumerable<T> objects)
        {
            if (objects == null)
            {
                return;
            }

            foreach (T obj in objects)
            {
                Push(obj);
            }
        }

        /// <summary>
        /// Clear the pool, optionally destroying the pooled objects.
        /// </summary>
        /// <param name="destroyObjects">Whether to destroy the pooled objects or not.</param>
        public void Clear(bool destroyObjects = true)
        {
            if (destroyObjects)
            {
                foreach (T item in PoolStack)
                {
                    UObject.Destroy(item as UObject);
                }
            }

            PoolStack.Clear();
        }

        /// <summary>
        /// Create an instance of the blueprint and return it.
        /// </summary>
        /// <param name="blueprint">The blueprint to use for object instantiation.</param>
        /// <returns>The new instance of the blueprint.</returns>
        private T NewObject(T blueprint)
        {
            if (CreateFunction != null)
            {
                return CreateFunction(blueprint);
            }

            return blueprint is null || !(blueprint is UObject) ? null : UObject.Instantiate(blueprint as UObject) as T;
        }
    }
}