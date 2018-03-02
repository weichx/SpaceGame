using System;
using System.Collections.Generic;
using SpaceGame.Events;
using SpaceGame.Util;
using SpaceGame.Weapons;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SpaceGame {

    public partial class LinearProjectileHandler : WeaponHandler {

        private int[] ownerIds;
        private float[] lifeTimes;
        private Transform[] transforms;
        private PositionDirection[] orientations;

        private int capacity;
        private int activeCount;
        private readonly List<WeaponDespawn> despawnList;
        private readonly List<MuzzleFlash> flashes;

        private readonly GameObjectPool projectilePool;
        private readonly GameObjectPool muzzleFlashPool;

        private readonly Transform parent;
        private readonly LinearProjectileDefinition projectileDefinition;

        public LinearProjectileHandler(Transform parent, LinearProjectileDefinition projectileDefinition) {
            this.parent = parent;
            this.projectileDefinition = projectileDefinition;
            capacity = Mathf.Max(100, capacity);
            flashes = new List<MuzzleFlash>();
            despawnList = new List<WeaponDespawn>(capacity);
            transforms = new Transform[capacity];
            orientations = new PositionDirection[capacity];
            lifeTimes = new float[capacity];
            ownerIds = new int[capacity];
            projectilePool = new GameObjectPool(projectileDefinition.projectile, 100, 25);
            muzzleFlashPool = new GameObjectPool(projectileDefinition.muzzleFlash, 20, 5);
        }

        public override void Spawn(List<FiringParameters> shots) {
            EnsureCapacity(shots.Count);
            for (int i = 0; i < shots.Count; i++) {
                FiringParameters parameters = shots[i];
                Quaternion rotation = Quaternion.LookRotation(parameters.direction);

                GameObject flash = muzzleFlashPool.Spawn(); // todo -- build a pool that can SpawnRandom()
                Transform flashTransform = flash.transform;
                flashTransform.position = parameters.position;
                flashTransform.rotation = rotation * Quaternion.Euler(0, 0, Random.Range(-360f, 360f));
                flashTransform.localScale *= Random.Range(1, 2);
                flashTransform.parent = parent;

                Transform spawned = projectilePool.Spawn().transform;
                spawned.position = parameters.position;
                spawned.rotation = rotation;
                spawned.parent = parent;

                PositionDirection orientation;
                lifeTimes[activeCount] = 0;
                transforms[activeCount] = spawned;
                orientation.direction = spawned.forward;
                orientation.position = spawned.position;
                orientations[activeCount] = orientation;
                ownerIds[activeCount] = parameters.ownerId;
                flashes.Add(new MuzzleFlash(flash));
                activeCount++;
            }
        }

        public override void Tick() {
            float delta = Time.deltaTime;
            float projectileSpeed = projectileDefinition.projectileSpeed;
            float raycastAdvance = projectileDefinition.raycastAdvance;
            float muzzleFlashDuration = projectileDefinition.muzzleFlashDuration;
            float maxLifeTime = projectileDefinition.maxLifeTime;

            float castDistance = projectileSpeed * raycastAdvance * delta;
            float projectileStep = projectileSpeed * delta;
            int layer = LayerMask.GetMask("Entity");

            for (int i = 0; i < flashes.Count; i++) {
                MuzzleFlash flash = flashes[i];
                flash.lifeTime += delta;
                flashes[i] = flash; // flash is a struct so we have to write back into it
                if (flash.lifeTime >= muzzleFlashDuration) {
                    muzzleFlashPool.Despawn(flash.gameObject);
                    flash.gameObject.SetActive(false);
                    flash.gameObject = null;
                    flashes.RemoveAt(i);
                    i--;
                }
            }

            if (activeCount == 0) return;

            for (int i = 0; i < activeCount; i++) {
                RaycastHit hit;
                PositionDirection orientaiton = orientations[i];
                if (Physics.Raycast(orientaiton.position, orientaiton.direction, out hit, castDistance, layer)) {
                    Entity impactedEntity = hit.transform.GetComponent<Entity>();

                    if (impactedEntity == null) {
                        DebugConsole.Log("Impacted a collider without an Entity attached: ", hit.transform.name);
                    }
                    else if (impactedEntity.id != ownerIds[i]) {
                        // todo -- do a deeper ray cast agains geometry here before despawning
                        Despawn(i, WeaponDespawnType.Collision, hit);
                        i--;
                    }
                    else {
                        Debug.Log("Hit " + hit.transform.name);
                    }
                }
            }

            for (int i = 0; i < activeCount; i++) {
                orientations[i].position += orientations[i].direction * projectileStep;
                transforms[i].position = orientations[i].position;
            }

            for (int i = 0; i < activeCount; i++) {
                lifeTimes[i] += delta;
                if (lifeTimes[i] >= maxLifeTime) {
                    Despawn(i, WeaponDespawnType.MaxLifeTimeExceeded);
                    i--;
                }
            }

            for (int i = 0; i < despawnList.Count; i++) {
                WeaponDespawn despawn = despawnList[i];
                if (despawn.weaponDespawnType == WeaponDespawnType.Collision) {
                    Entity entity = despawn.raycastHit.transform.GetComponent<Entity>();
                    if (entity != null) {
                        EventSystem.Instance.Trigger(
                            new Evt_WeaponImpact(entity.id, despawn.raycastHit)
                        );
                    }
                }

                projectilePool.Despawn(despawn.transform.gameObject);
                despawn.transform = null;
            }

            despawnList.Clear();
        }

        private void EnsureCapacity(int additional) {
            while (activeCount + additional > capacity) {
                capacity *= 2;
            }

            Array.Resize(ref ownerIds, capacity);
            Array.Resize(ref lifeTimes, capacity);
            Array.Resize(ref transforms, capacity);
            Array.Resize(ref orientations, capacity);
        }

        private void Despawn(int index, WeaponDespawnType weaponDespawnType, RaycastHit hit = default(RaycastHit)) {
            activeCount--;

            WeaponDespawn despawn;
            despawn.raycastHit = hit;
            despawn.ownerId = ownerIds[index];
            despawn.transform = transforms[index];
            despawn.weaponDespawnType = weaponDespawnType;
            despawnList.Add(despawn);

            transforms[index] = transforms[activeCount];
            lifeTimes[index] = lifeTimes[activeCount];
            ownerIds[index] = ownerIds[activeCount];
            orientations[index] = orientations[activeCount];
            transforms[activeCount] = null;
        }

    }

}