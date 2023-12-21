using UnityEngine;

namespace SametHope.SoundSystem
{
    /// <summary>
    /// This component functions as a basic <see cref="AudioSource"/> but with <see cref="Audio.Sound"/> asset and <see cref="Audio.SoundSystem"/>.
    /// </summary>
    public class SoundPlayer : MonoBehaviour
    {
        [field: Tooltip("Sound of this sound player.")]
        [field: SerializeField] public Sound Sound { get; private set; }

        [Tooltip("When to automatically play the sound.")]
        [SerializeField] private PlayType _playAt = PlayType.Awake;

        [Tooltip("If enabled, the sound will use the position of this sound player.")]
        [SerializeField] private bool _usePosition = false;

        private void Awake()
        {
            if (_playAt.HasFlag(PlayType.Awake)) Play();
        }
        private void Start()
        {
            if (_playAt.HasFlag(PlayType.Start)) Play();
        }
        private void OnDestroy()
        {
            if (gameObject.scene.isLoaded && _playAt.HasFlag(PlayType.Destroy)) Play();
        }

        private void OnEnable()
        {
            if (_playAt.HasFlag(PlayType.OnEnable)) Play();
        }

        private void OnDisable()
        {
            if (gameObject.scene.isLoaded && _playAt.HasFlag(PlayType.OnDisable)) Play();
        }

        public void Play()
        {
            if (_usePosition) SoundMaster.Play(Sound, transform.position);
            else SoundMaster.Play(Sound);
        }

        [System.Serializable, System.Flags]
        private enum PlayType
        {
            None = 0,
            Awake = 1,
            Start = 2,
            Destroy = 4,
            OnEnable = 8,
            OnDisable = 16,
        }


#if UNITY_EDITOR
        private static System.DateTime _lastMultipleAdditionTime;
        [UnityEditor.MenuItem("GameObject/Audio/Sound Player", false, 10)]
        private static void Create()
        {
            if (UnityEditor.Selection.count <= 1)
            {
                SoundPlayer player;
                if (UnityEditor.Selection.activeGameObject == null)
                {
                    player = new GameObject("Sound Player").AddComponent<SoundPlayer>();
                }
                else
                {
                    player = UnityEditor.Selection.activeGameObject.AddComponent<SoundPlayer>();
                }

                UnityEditor.Selection.activeGameObject = player.gameObject;
            }
            // Do not allow multiple calls in a second for multiple selection
            else if (_lastMultipleAdditionTime.Second != System.DateTime.Now.Second)
            {
                _lastMultipleAdditionTime = System.DateTime.Now;
                var objs = UnityEditor.Selection.GetFiltered<GameObject>(UnityEditor.SelectionMode.Editable);

                foreach (var obj in objs)
                {
                    obj.AddComponent<SoundPlayer>();
                }
            }
        }
#endif

    }
}
