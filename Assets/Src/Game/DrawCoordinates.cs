using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
[ExecuteInEditMode]
#endif

public class DrawCoordinates : MonoBehaviour {

    #if UNITY_EDITOR
    
    public bool showDistanceLabels = true;
    public bool drawRadius = true;
    public float radiusBase = 100f;
    public int radiusRings = 5;

    private void OnDrawGizmos() {
        if (drawRadius) {
            GUIStyle style = new GUIStyle(EditorStyles.label);
            style.fontSize = 8;
            style.normal.textColor = Color.white;

            for (int i = 1; i < radiusRings + 1; i++) {
                if (showDistanceLabels) {
                    Handles.Label(new Vector3(i * radiusBase, 0, 0), (i * radiusBase) + "m", style);
                    Handles.Label(new Vector3(-i * radiusBase, 0, 0), (i * radiusBase) + "m", style);
                    Handles.Label(new Vector3(0, 0, (i * radiusBase)), (i * radiusBase) + "m", style);
                    Handles.Label(new Vector3(0, 0, (-i * radiusBase)), (i * radiusBase) + "m", style);
                }

                Handles.DrawWireDisc(Vector3.zero, Vector3.up, radiusBase * i);
            }

        }
    }
    #endif

}