using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SametHope.SoundSystem.Pooling;

namespace SametHope.SoundSystem
{
    /// <summary>
    /// This class allows easy and customizable audio pooling and playback utilizing <see cref="Sound"/> instances.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class SoundMaster : MonoBehaviour
    {
        /// <summary>
        /// Instance of the <see cref="SoundMaster"/>. Useful for calling <see cref="Object.DontDestroyOnLoad(Object)"/> if needed.
        /// </summary>
        public static SoundMaster Instance { get; private set; }


        [field: Tooltip("AudioSource to take as a prefab when creating new instances. Can be null in which case a new gameobject with the component will be created and used.")]
        [field: SerializeField] public AudioSource SourcePrefab { get; set; }

        private readonly Dictionary<string, Sound> _allSoundsDict = new();

        /// <summary>
        /// Active sources that are created by <see cref="SoundMaster"/> methods and are either playing or paused. Sound is included for optional filtering.
        /// <para>Useful for pausing/unpausing or taking other collective actions.</para>
        /// </summary>
        public List<(AudioSource source, Sound sound)> ActiveSounds { get; private set; } = new();

        #region Initialization
        private void Awake()
        {
            EnsureInstanceAssigned();
            AttemptInitializePrefab();
            SetupSoundAssets();
            InitializeSoundDict();
            InitializeSoundPools();
        }

        // In case the system is a scene object and there is no other instance, set the instance automatically
        private void EnsureInstanceAssigned()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void SetupSoundAssets()
        {
            foreach (Sound sound in Resources.FindObjectsOfTypeAll<Sound>())
            {
                sound.Setup();
            }
        }

        private void InitializeSoundDict()
        {
            foreach (Sound sound in Resources.FindObjectsOfTypeAll<Sound>())
            {
                _allSoundsDict.Add(sound.GetName(), sound);
            }
        }

        private void InitializeSoundPools()
        {
            foreach (Sound sound in Resources.FindObjectsOfTypeAll<Sound>())
            {
                SimplePool<AudioSource> sourcePool = SimplePoolHelper.GetPool<AudioSource>(sound.GetName());
                Transform poolParent = new GameObject($"[Sound Pool] [{sound.GetName()}]").transform;
                poolParent.SetParent(transform);

                sourcePool.Blueprint = SourcePrefab;
                sourcePool.CreateFunction = bp =>
                {
                    AudioSource source = Instantiate(bp, poolParent);
                    source.name = $"[{sound.GetName()}]";
                    return source;
                };

                _ = sourcePool.Populate(sound.GetPoolCount());
            }
        }

        private void AttemptInitializePrefab()
        {
            if (SourcePrefab == null)
            {
                SourcePrefab = new GameObject($"[SoundPrefab]").AddComponent<AudioSource>();
                SourcePrefab.transform.SetParent(transform);
                SourcePrefab.playOnAwake = false;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Check if an instance exists, if not, create and assign a new instance.
        /// <para>This is called each time a sound is played to ensure lazy initialization in case if the system did not already initialize.</para>
        /// </summary>
        public static void ValidateInstance()
        {
            if (Instance == null)
            {
                Instance = new GameObject($"[{nameof(SoundMaster)}]").AddComponent<SoundMaster>();
            }
        }

        /// <summary>
        /// Play a sound by name.
        /// <para>Requires the sound asset to be included in the build and loaded into the memory. It is recommended to use <see cref="Play(Sound, Vector3)"/> instead.</para>
        /// </summary>
        /// <param name="soundName">Name of the sound asset.</param>
        /// <param name="position">Position to play the sound at.</param>
        /// <returns>The AudioSource that plays the sound. Will be null if the method fails.</returns>
        public static AudioSource Play(string soundName, Vector3 position = default)
        {
            return !TryValidateByName(soundName) ? null : Play(Instance._allSoundsDict[soundName], position);
        }

        /// <summary>
        /// Play a sound by sound asset.
        /// </summary>
        /// <param name="sound">The sound asset.</param>
        /// <param name="position">Position to play the sound at.</param>
        /// <returns>The AudioSource that plays the sound. Will be null if the method fails.</returns>
        public static AudioSource Play(Sound sound, Vector3 position = default)
        {
            return AttemptHandlePlay(sound, position);
        }

        /// <summary>
        /// Creates and returns a new AudioSource from the given sound name.
        /// <para>Requires the sound asset to be included in the build and loaded into the memory. It is recommended to use <see cref="GetSpecial(Sound)"/> instead.</para>
        /// </summary>
        /// <param name="soundName">Name of the sound asset.</param>
        /// <returns>The AudioSource that plays the sound. Will be null if the method fails.</returns>
        public static AudioSource GetSpecial(string soundName)
        {
            return !TryValidateByName(soundName) ? null : GetSpecial(Instance._allSoundsDict[soundName]);
        }

        /// <summary>
        /// Creates and returns a new AudioSource from the given sound asset.
        /// </summary>
        /// <returns>The AudioSource that plays the sound. Will be null if the method fails.</returns>
        public static AudioSource GetSpecial(Sound sound)
        {
            ValidateInstance();

            AudioSource source = Instantiate(Instance.SourcePrefab);
            SetupSourceFromSound(source, sound);
            return source;
        }
        #endregion

        private static AudioSource AttemptHandlePlay(Sound sound, Vector3 position)
        {
            ValidateInstance();

            if (!sound.IsCooldownReady())
            {
                return null;
            }

            AudioSource source = SimplePoolHelper.Pop<AudioSource>(sound.GetName());
            source.transform.position = position;
            SetupSourceFromSound(source, sound);
            source.Play();

            sound.UpdateLastPlayTime();
            Instance.ActiveSounds.Add(new(source, sound));
            _ = Instance.StartCoroutine(Co_ScheduleRelease(source, sound));

            return source;
        }

        private static void SetupSourceFromSound(AudioSource source, Sound sound)
        {
            source.clip = sound.GetClip();
            source.pitch = sound.GetPitch();
            source.volume = sound.GetVolume();
            source.outputAudioMixerGroup = sound.GetMixerGroup();
        }

        private static IEnumerator Co_ScheduleRelease(AudioSource source, Sound sound)
        {
            // ~~~ Is playing or paused aka is active
            while (source.isPlaying || source.time != 0f)
            {
                yield return null;
            }
            source.Pool(sound.GetName());
            _ = Instance.ActiveSounds.Remove(new(source, sound));
        }

        private static bool TryValidateByName(string soundName)
        {
            if (!Instance._allSoundsDict.ContainsKey(soundName))
            {
                Debug.LogWarning($"Failed to find the requested sound '{soundName}'. Are you sure it is named right and has been included in the build?");
                return false;
            }
            return true;
        }
    }
}
