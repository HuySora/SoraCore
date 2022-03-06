namespace SoraCore.Manager {
    using MyBox;
    using System;
    using UnityEngine;
    using UnityEngine.Audio;

    public partial class SoundManager : SoraManager {
        #region Dispatching Static Event -------------------------------------------------------------------------------------

        /// <summary>
        /// Raised when the mixer group volume changed
        /// </summary>
        public static event Action<MixerGroupSO, float> OnVolumeChanged;

        #endregion
        #region Static -------------------------------------------------------------------------------------------------------

        private static Action<AudioSO, Vector3, AudioConfigurationSO, MixerGroupSO> _playPosRequested;
        private static Action<AudioSO, Transform, AudioConfigurationSO, MixerGroupSO> _playTransformRequested;
        private static Action<AudioSO, AudioConfigurationSO, MixerGroupSO> _playMusicRequested;
        private static Action<MixerGroupSO, float> _setVolumeRequested;

        /// <summary>
        /// Instantiate an <see cref="AudioSource"/> that play an audio clip from <paramref name="ad"/> at <paramref name="pos"/> using embedded configuration
        /// </summary>
        public static void Play(AudioSO ad, Vector3 pos) => Play(ad, pos, ad.AudioConfiguration, ad.MixerGroup);

        /// <summary>
        /// Instantiate an <see cref="AudioSource"/> that play an audio clip from <paramref name="ad"/> at <paramref name="pos"/> using custom configuration
        /// </summary>
        public static void Play(AudioSO ad, Vector3 pos, AudioConfigurationSO config, MixerGroupSO group) {
            if (_playPosRequested != null) {
                _playPosRequested.Invoke(ad, pos, config, group);
                return;
            }

            LogWarningForEvent(nameof(SoundManager));
        }

        /// <summary>
        /// Attach an <see cref="AudioSource"/> to <paramref name="parent"/> that play an audio clip from <paramref name="ad"/> using embedded configuration
        /// </summary>
        public static void Play(AudioSO ad, Transform parent) => Play(ad, parent, ad.AudioConfiguration, ad.MixerGroup);

        /// <summary>
        /// Attach an <see cref="AudioSource"/> to <paramref name="parent"/> that play an audio clip from <paramref name="ad"/> using custom configuration
        /// </summary>
        public static void Play(AudioSO ad, Transform parent, AudioConfigurationSO config, MixerGroupSO group) {
            if (_playTransformRequested != null) {
                _playTransformRequested.Invoke(ad, parent, config, group);
                return;
            }

            LogWarningForEvent(nameof(SoundManager));
        }

        /// <summary>
        /// Request music source to play <paramref name="ad"/> using embedded configuration
        /// </summary>
        public static void PlayMusic(AudioSO ad) => PlayMusic(ad, ad.AudioConfiguration, ad.MixerGroup);

        /// <summary>
        /// Request music source to play <paramref name="ad"/> using custom configuration
        /// </summary>
        public static void PlayMusic(AudioSO ad, AudioConfigurationSO config, MixerGroupSO group) {
            if (_playMusicRequested != null) {
                _playMusicRequested.Invoke(ad, config, group);
                return;
            }

            LogWarningForEvent(nameof(SoundManager));
        }

        /// <summary>
        /// Change dB volume of <paramref name="mixerGroup"/> base on <paramref name="value"/>
        /// </summary>
        public static void SetVolume(MixerGroupSO group, float value) {
            if (_setVolumeRequested != null) {
                _setVolumeRequested.Invoke(group, value);
                return;
            }

            LogWarningForEvent(nameof(SoundManager));
        }

        #endregion

        public const float MinVolume = 0.0001f;
        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private PrefabSO _audioSourcePrefab;
        [SerializeField] private float _volumeMultiplier = 30f;
        [SerializeField] private AudioSource _musicSource;

        private void OnEnable() {
            _playPosRequested += InnerPlayPos;
            _playTransformRequested += InnerPlayTransform;
            _playMusicRequested += InnerPlayMusic;
            _setVolumeRequested += InnerSetVolume;
        }

        private void OnDisable() {
            _playPosRequested -= InnerPlayPos;
            _playTransformRequested -= InnerPlayTransform;
            _playMusicRequested -= InnerPlayMusic;
            _setVolumeRequested -= InnerSetVolume;
        }

        private void InnerPlayPos(AudioSO ad, Vector3 pos, AudioConfigurationSO cfg, MixerGroupSO grp) {
            /// Spawn an audio source at target position
            AudioSource audioSource = GameObjectManager.Instantiate(_audioSourcePrefab).GetComponent<AudioSource>();
            audioSource.transform.position = pos;

            SetUpAndPlay(audioSource, ad, cfg, grp);
        }

        private void InnerPlayTransform(AudioSO ad, Transform parent, AudioConfigurationSO cfg, MixerGroupSO grp) {
            /// Spawn an audio source at target position
            AudioSource audioSource = GameObjectManager.Instantiate(_audioSourcePrefab).GetComponent<AudioSource>();
            audioSource.transform.parent = parent;

            SetUpAndPlay(audioSource, ad, cfg, grp);
        }

        private void InnerPlayMusic(AudioSO ad, AudioConfigurationSO cfg, MixerGroupSO grp) => SetUpAndPlay(_musicSource, ad, cfg, grp);

        private void SetUpAndPlay(AudioSource source, AudioSO ad, AudioConfigurationSO cfg, MixerGroupSO grp) {
            // Apply configuration to the audio source
            source.clip = ad.GetClip;
            source.loop = ad.Loop;
            source.ApplyConfig(cfg);
            source.outputAudioMixerGroup = grp.Group;

            // Play
            source.Play();
        }

        public void InnerSetVolume(MixerGroupSO grp, float value) {
            if (value > 1) {
                string s1 = $"{grp.Group.name} value".Bold();
                SoraCore.LogWarning($"{s1} parameter > 1, it could be too loud.", nameof(SoundManager));
            }

            // Magic number https://www.youtube.com/watch?v=MmWLK9sN3s8&t=374s (6:14)
            float dBValue = Mathf.Log10(Mathf.Max(MinVolume, value)) * _volumeMultiplier;
            _audioMixer.SetFloat(grp.VolumeParameter, dBValue);

            OnVolumeChanged?.Invoke(grp, value);

            return;
        }
    }
}