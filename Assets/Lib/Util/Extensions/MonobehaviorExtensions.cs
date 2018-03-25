using UnityEngine;

namespace Weichx.Util {

    public static class MonobehaviorExtensions {

        public static T GetOrCreateComponent<T>(this MonoBehaviour behavior) where T : MonoBehaviour {
            T retn = behavior.GetComponent<T>();
            if (retn == null) retn = behavior.gameObject.AddComponent<T>();
            return retn;
        }

    }

}