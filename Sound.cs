using UnityEngine;
using UnityEngine.Audio;

namespace SametHope.SoundSystem
{
    /// <summary>
    /// Sound class represents an audio sound with various settings.
    /// </summary>
    [CreateAssetMenu(fileName = "New Sound", menuName = "SametHope/Sound System/Sound")]
    public class Sound : ScriptableObject
    {
        internal const float DEFAULT_MIN_PITCH = 1f;
        internal const float DEFAULT_MAX_PITCH = 1f;
        internal const float DEFAULT_VOLUME = 1f;
        internal const float DEFAULT_COOLDOWN = 0.025f;
        internal const float DEFAULT_LAST_TIME_PLAYED = -999f;

        [Header("General")]
        [Tooltip("The audio mixer group that the audio source of the sound will be assigned to. Can be null.")]
        [SerializeField] private AudioMixerGroup _mixerGroup;
        [Tooltip("Amount of audio sources to pool for this sound.\nModifying this in runtime is not recommended.")]
        [SerializeField] private int _poolCount = 10;

        [Header("Specifics")]
        [Tooltip("Clips to select from when the sound will play.\nModifying this in runtime is not recommended.")]
        [SerializeField] private AudioClip[] _clips;
        [Tooltip("Minimum pitch to use when the sound will play.")]
        [SerializeField] private float _minPitch = DEFAULT_MIN_PITCH;
        [Tooltip("Maximum pitch to use when the sound will play.")]
        [SerializeField] private float _maxPitch = DEFAULT_MAX_PITCH;
        [Tooltip("Volume of this sound.")]
        [SerializeField, Range(0f, 1f)] private float _volume = DEFAULT_VOLUME;
        [Tooltip("Cooldown in seconds. When the sound is requested to play too often, it will be ignored.\nModifying this in runtime is not recommended.")]
        [SerializeField, Min(0)] private float _cooldown = DEFAULT_COOLDOWN;

        [Header("Other")]
        [Tooltip("This value is updated each time the sound plays, exposed for debugging etc.")]
        [SerializeField] private float _lastPlayTime = DEFAULT_LAST_TIME_PLAYED;

        /// <summary>
        /// Gets the name of the sound.
        /// </summary>
        public string GetName() => name;

        /// <summary>
        /// Gets the AudioMixerGroup assigned to the sound.
        /// </summary>
        public AudioMixerGroup GetMixerGroup() => _mixerGroup;

        /// <summary>
        /// Gets the pool count for this sound.
        /// </summary>
        public int GetPoolCount() => _poolCount;

        /// <summary>
        /// Gets a random AudioClip from the available clips.
        /// </summary>
        public AudioClip GetClip() => _clips[Random.Range(0, _clips.Length)];

        /// <summary>
        /// Gets a random pitch value within the specified range.
        /// </summary>
        public float GetPitch() => Random.Range(_minPitch, _maxPitch);

        /// <summary>
        /// Gets the volume level of the sound.
        /// </summary>
        public float GetVolume() => _volume;

        /// <summary>
        /// Gets the cooldown time between consecutive plays of the sound.
        /// </summary>
        public float GetCooldown() => _cooldown;

        /// <summary>
        /// Checks if the cooldown time has passed since the last play.
        /// </summary>
        public bool IsCooldownReady() => Time.time - _lastPlayTime >= _cooldown;

        /// <summary>
        /// Updates the last play time to the current time.
        /// </summary>
        public void UpdateLastPlayTime() => _lastPlayTime = Time.time;

        /// <summary>
        /// Performs setup operations for the Sound object. This is significant as the Sound object is a scriptable object.
        /// </summary>
        public void Setup()
        {
            _lastPlayTime = DEFAULT_LAST_TIME_PLAYED;
        }

#if UNITY_EDITOR
        internal void ResetBasicValues()
        {
            _minPitch = DEFAULT_MIN_PITCH;
            _maxPitch = DEFAULT_MAX_PITCH;
            _volume = DEFAULT_VOLUME;
            _cooldown = DEFAULT_COOLDOWN;
        }
#endif
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(Sound))]
    public class TestScriptableEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            UnityEditor.EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset Basic Values"))
            {
                ((Sound)target).ResetBasicValues();
            }
            GUILayout.EndHorizontal();
            UnityEditor.EditorGUILayout.HelpBox("Basic values are: Min and max pitch, volume and cooldown.", UnityEditor.MessageType.Info);
        }
    }
#endif
}