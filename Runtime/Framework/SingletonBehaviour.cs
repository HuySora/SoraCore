using UnityEngine;

namespace SoraCore.Core {
    public abstract class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour {
        protected static T m_Instance;
        private static bool s_IsApplicationQuitting = false;
        
        private void OnApplicationQuit() {
            s_IsApplicationQuitting = true;
        }
        
        /// <summary>
        /// Get the current instance of this singleton (or create a new one)
        /// </summary>
        public static bool TryGetInstance(out T ins) {
            ins = null;
            if (m_Instance != null) {
                ins = m_Instance;
                return true;
            }
            
            if (s_IsApplicationQuitting) {
                return false;
            }
            
            T[] instances = FindObjectsByType<T>(FindObjectsSortMode.InstanceID);
            
            // Only found 1
            if (instances.Length == 1) {
                m_Instance = instances[0];
                ins =  m_Instance;
                return true;
            }
            
            string typeName = $"[{typeof(T)}]";
            
            // Found nothing
            if (instances.Length == 0) {
                Debug.LogError($"No singleton instance of type {typeName} was found!");
                return false;
            }
            
            // Found more than one
            m_Instance = instances[0];
            ins =  m_Instance;
            Debug.LogWarning($"More than 1 singleton instance of type {typeName} was found. Will use the first one in the list", instances[0].gameObject);
            return true;
        }
    }
}