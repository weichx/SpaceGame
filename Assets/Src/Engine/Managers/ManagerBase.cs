using System;
using SpaceGame;
using UnityEngine;

namespace Src.Engine {

    public class ManagerBase : MonoBehaviour {

        protected void Trigger<T>(T evt) where T : GameEvent {
            EventSystem.Instance.Trigger(evt);    
        }

        protected void AddListener<T>(Action<T> handler) where T : GameEvent {
            EventSystem.Instance.AddListener(handler);
        }

        protected void RemoveListener<T>(Action<T> handler) where T : GameEvent {
            EventSystem.Instance.RemoveListener(handler);
        }

    }

}