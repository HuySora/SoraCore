namespace SoraCore.Manager {
    using UnityEngine;
    using UnityEngine.UIElements;
    using System;
    using MyBox;

    public class LoadingUIController : MonoBehaviour, IUIController {
        [field: SerializeField, AutoProperty] public UIDocument Document { get; private set; }
        [Range(0f, 1f)] public float MainProgress;
        [Range(0f, 1f)] public float SubProgress;

        private ProgressBar _mainProgressBar;
        private ProgressBar _subProgressBar;

        private void Awake() {
            _mainProgressBar = Document.rootVisualElement.Q<ProgressBar>("main-progress");
            _subProgressBar = Document.rootVisualElement.Q<ProgressBar>("sub-progress");
        }


        private void Update() {
            // TODO: Make the progress bar smoother
            _mainProgressBar.value = MainProgress;
            _subProgressBar.value = SubProgress;
        }
        public void ShowUI(bool value) {
            Document.rootVisualElement.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
            Document.rootVisualElement.pickingMode = value ? PickingMode.Position : PickingMode.Ignore;
        }
    }
}