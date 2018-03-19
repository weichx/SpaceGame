using System;
using SpaceGame.Engine;
using SpaceGame.Events;
using Weichx.Util;
using UnityEngine;
using Util;

namespace SpaceGame.Missions {

    public abstract class MissionBase : MonoBehaviour {

        protected void Trigger<T>(T evt) where T : GameEvent {
            EventSystem.Instance.Trigger(evt);    
        }
        
        protected void OnGameEvent<T>(Action<T> handler) where T : GameEvent {
            EventSystem.Instance.AddListener(handler);
        }

        protected void OffGameEvent<T>(Action<T> handler) where T : GameEvent {
            EventSystem.Instance.RemoveListener(handler);
        }

        protected int SetTimeout(float time, Action callback) {
            return GameTimer.Instance.SetTimeout(time, callback);
        }

        public void Activate(Entity entity) {
            entity.gameObject.SetActive(true);
        }
//
//        public void Deactivate(EntityGroup entityGroup) {
//            for (int i = 0; i < entityGroup.entities.Count; i++) {
//                Deactivate(entityGroup.entities[i]);
//            }
//        }

        public void Deactivate(Entity entity) {
//            EntityDatabase.ActiveEntites.Remove(entity);
            // trigger event?
            entity.gameObject.SetActive(false);
        }

        public void Depart(Entity entity) {
            Trigger(new Evt_EntityDeparting(entity.id));    
        }
        
        public void Arrive(Entity entity) {
            GameData.Instance.RegisterEntity(entity);
            Trigger(new Evt_EntityArriving(entity.id, ArrivalType.Spawn));
        }

        public void ArriveFromHyperspace(Entity entity) {
            Trigger(new Evt_EntityArriving(entity.id, ArrivalType.Hyperspace));
        }
        
        protected abstract void BuildStateChart(StateChart.StateChartBuilder builder);

    }

}