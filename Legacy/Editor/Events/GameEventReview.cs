using UnityEditor;
using UnityEngine;

namespace SoraCore
{
    [CustomEditor(typeof(GameEvent), editorForChildClasses: true)]
    public class GameEventReviewEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = Application.isPlaying;

            GameEvent e = target as GameEvent;
            if (GUILayout.Button("Invoke"))
                e?.Invoke();
        }
    }

}
