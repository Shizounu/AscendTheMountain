using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Commands.Pool
{
    public abstract class Poolable<T> where T : ICommand, new() {
        public static T GetAvailable() {
            return CommandPool<T>.Instance.GetElement();
        }
        public void ReturnToPool(T elem) {
            CommandPool<T>.Instance.ReturnElement(elem);
        }

    }

    public class CommandPool<T> where T : ICommand, new() {
        const int baseCapacity = 16;
        public CommandPool() {
            pool = new();
            AddElements(baseCapacity);
            capacity = baseCapacity;
            
        }

        private static CommandPool<T> instance;
        public static CommandPool<T> Instance { 
            get {
                if (instance == null) {
                    instance = new CommandPool<T>();
                }
                return instance;
            }
        }

        private int capacity;
        private Queue<T> pool;

        public void AddElements(int count) {
            for (int i = 0; i < count; i++) {
                pool.Enqueue(new());
            }
        }
        public T GetElement() {
            return new();
            if (pool.Count <= 0) { 
                AddElements(capacity);
                capacity *= 2;
            }
            return pool.Dequeue();
        }
        public void ReturnElement(T element) {
            //pool.Enqueue(element);
        }

    }

}
