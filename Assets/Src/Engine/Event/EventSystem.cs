using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceGame {

    using Callback = Action<GameEvent>;

    public class EventSystem {

        private Queue<GameEvent> activeQueue;
        private Queue<GameEvent> inactiveQueue;

        public static readonly EventSystem Instance = new EventSystem();

        private readonly Dictionary<Type, Callback> typeToWrappedActionMap;
        private readonly Dictionary<Delegate, Callback> callbackToWrapperMap;
        private bool isTicking;

        public EventSystem() {
            activeQueue = new Queue<GameEvent>(64);
            inactiveQueue = new Queue<GameEvent>(64);
            typeToWrappedActionMap = new Dictionary<Type, Callback>();
            callbackToWrapperMap = new Dictionary<Delegate, Callback>();
        }

        public void AddListener<T>(Action<T> callback) where T : GameEvent {
            // Early-out if we've already registered this delegate
            if (callbackToWrapperMap.ContainsKey(callback)) {
                return;
            }

            Type type = typeof(T);

            //Need to wrap this in an action to make the type system happy
            Callback wrapper = (evt) => {
                callback.Invoke(evt as T);
            };

            callbackToWrapperMap[callback] = wrapper;

            Callback tempDel;
            if (typeToWrappedActionMap.TryGetValue(type, out tempDel)) {
                tempDel += wrapper;
                typeToWrappedActionMap[type] = tempDel;
            }
            else {
                typeToWrappedActionMap[type] = wrapper;
            }
        }

        public void RemoveListener<T>(Action<T> callback) where T : GameEvent {
            Callback wrapper;
            if (callbackToWrapperMap.TryGetValue(callback, out wrapper)) {
                Callback wrappedAction;
                Type type = typeof(T);
                if (typeToWrappedActionMap.TryGetValue(type, out wrappedAction)) {
                    wrappedAction -= wrapper;
                    if (wrappedAction == null) {
                        typeToWrappedActionMap.Remove(type);
                    }
                    else {
                        typeToWrappedActionMap[type] = wrappedAction;
                    }
                }

                callbackToWrapperMap.Remove(callback);
            }
        }

        public void RemoveAll() {
            typeToWrappedActionMap.Clear();
            callbackToWrapperMap.Clear();
        }

        public bool HasListener<T>(Action<T> callback) where T : GameEvent {
            return callbackToWrapperMap.ContainsKey(callback);
        }

        public void Trigger(GameEvent evt) {
            activeQueue.Enqueue(evt);
        }

        public void TriggerImmediately(GameEvent e) {
            Callback wrappedAction;
            if (typeToWrappedActionMap.TryGetValue(e.GetType(), out wrappedAction)) {
                wrappedAction.Invoke(e);
            }
        }

        public void Tick() {
            if (isTicking) return;
            isTicking = true;
            Queue<GameEvent> swap = inactiveQueue;
            inactiveQueue = activeQueue;
            activeQueue = swap;
            while (inactiveQueue.Count > 0) {
                Callback action;
                GameEvent evt = inactiveQueue.Dequeue();
                if (typeToWrappedActionMap.TryGetValue(evt.GetType(), out action)) {
                    action.Invoke(evt);
                }
            }

            isTicking = false;
        }

        public void Dispose() {
            RemoveAll();
            inactiveQueue.Clear();
            activeQueue.Clear();
        }

    }

}