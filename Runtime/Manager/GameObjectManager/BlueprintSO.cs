namespace SoraCore.Manager {
    using System;
    using UnityEngine;
    using MyBox;
    using UnityEngine.AddressableAssets;

    [CreateAssetMenu(fileName = "BlueprintData", menuName = "SoraCore/GameObject Manager/Blueprint Data")]
    public class BlueprintSO : ScriptableObject {
        [field: SerializeField] public AssetReferenceGameObject AssetRef { get; private set; }
        [field: SerializeField] public bool EnablePooling { get; private set; }
        [ReadOnly("<EnablePooling>k__BackingField", true)] public int preload = 10;
        [ReadOnly("<EnablePooling>k__BackingField", true)] public int capacity = 10;

        // Inherit this class and override these 3 for specific needs
        #region ObjectPool<T> delegate methods
        public virtual void OnGameObjGet(GameObject gObj) => gObj.SetActive(true);
        public virtual void OnGameObjRelease(GameObject gObj) => gObj.SetActive(false);
        public virtual void OnGameObjDestroy(GameObject gObj) => Destroy(gObj);
        #endregion
    }
}
