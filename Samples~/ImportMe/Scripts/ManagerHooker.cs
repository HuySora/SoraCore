using UnityEngine;
using SoraCore.Manager;

public class ManagerHooker : MonoBehaviour {
    [field: SerializeField] public MixerGroupSO MixerGroup { get; private set; }
    [field: SerializeField] public LoadingUIController LoadingUIController { get; private set; }

    private void OnEnable() {
        LevelManager.LoadStarted += OnLoadStarted;
        LevelManager.LoadProgressChanged += OnLoadProgressChanged;
        LevelManager.LoadFinished += OnLoadFinished;

        SoundManager.VolumeChanged += (grp, value) =>
        {
            PlayerPrefs.SetFloat($"{nameof(SoundManager)}_{grp.VolumeParameter}", value);
            PlayerPrefs.Save();
        };

        Invoke(nameof(ChangeVolume), 3f);
    }

    private void OnDisable() {
        LevelManager.LoadStarted -= OnLoadStarted;
        LevelManager.LoadProgressChanged -= OnLoadProgressChanged;
        LevelManager.LoadFinished -= OnLoadFinished;
    }

    private void ChangeVolume() {
        SoundManager.SetVolume(MixerGroup, Random.Range(0f, 1f));
    }
    private void OnLoadStarted(LoadContext ctx) {
        LoadingUIController.enabled = ctx.ShowLoadingScreen;
    }
    private void OnLoadProgressChanged(LoadContext ctx) {
        LoadingUIController.MainProgress = ctx.MainProgress;
        LoadingUIController.SubProgress = ctx.SubProgress;
    }
    private void OnLoadFinished(LoadContext ctx) {
        LoadingUIController.enabled = false;
    }
}