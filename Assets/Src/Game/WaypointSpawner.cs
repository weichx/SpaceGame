using UnityEngine;

namespace SpaceGame {

    public class WaypointSpawner : MonoBehaviour {

        public int count;
        public float range;
        public Color color;
        
        public GameObject[] spawns;
        
        public void Awake() {
            spawns = new GameObject[count];

            for (int i = 0; i < count; i++) {
                Vector3 position = Random.insideUnitSphere * range;
                spawns[i] = new GameObject();
                spawns[i].name = "Spawn[" + i + "]";
                spawns[i].transform.position = position;
            }
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.magenta;

            if (spawns == null) return;
            for (int i = 0; i < spawns.Length; i++) {
                if (i + 1 < spawns.Length) {
                    Gizmos.DrawLine(spawns[i].transform.position, spawns[i + 1].transform.position);
                }

                Gizmos.DrawCube(spawns[i].transform.position, new Vector3(2f, 2f, 2f));
            }

            if (spawns.Length > 1) {
                Gizmos.DrawLine(spawns[spawns.Length - 1].transform.position, spawns[0].transform.position);
            }
        }

    }

}