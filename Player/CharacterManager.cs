using Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager instance;

    [Header("Player Settings")] [SerializeField]
    private GameObject playerObject;

    [Header("Combat Settings")] public WeaponSlot weaponSlot;
    private Transform _player;
    private PlayerState _playerState;
    private CinemachineVirtualCamera _virtualCamera;
    private CharacterController characterController;
    public Transform player;
    public PlayerState playerState => _playerState ?? player.GetComponent<PlayerState>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        player=InitializePlayer();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private Transform InitializePlayer()
    {
        if (playerObject != null)
        {
            _player = playerObject.transform;
            characterController = _player.GetComponent<CharacterController>();
            DontDestroyOnLoad(playerObject);
        }

        return _player;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            return;
        }
        SetupVirtualCamera();
    }
    
    public void UpdatePlayerDamage()
    {
        if (playerState != null)
        {
            playerState.damage = 5 + (weaponSlot?.item?.damage ?? 0);
            GameEvents.Instance.OnPlayerStateChanged?.Invoke();
        }
    }
    
    public void TeleportPlayer(Vector3 position)
    {
        if (characterController == null)
            return;
        characterController.enabled = false;
        player.position = position;
        characterController.enabled = true;
    }

    private void SetupVirtualCamera()
    {
        var vcamObj = GameObject.FindGameObjectWithTag("Vcam");
        if (vcamObj != null && vcamObj.TryGetComponent(out _virtualCamera))
        {
            _virtualCamera.Follow = player.Find("LookAt");
        }
    }
    

}