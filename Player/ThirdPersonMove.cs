using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ThirdPersonMove : MonoBehaviour
{
    private CharacterController _controller;
    private Camera _mainCamera; // 改为Camera类型
    public Animator _animator;
    private float _targetRot = 0.0f;
    private float RotationSmoothTime = 0.1f;
    private float _rotationVelocity;
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    private float currentSpeed;
    public bool isRunning = false;
    private Vector2 _move;
    public InputSystem_Actions inputs;
    private PlayerAttack playerAttack;

    private void Awake()
    {
        inputs = new InputSystem_Actions();
        inputs.Player.Sprint.started += ctx => isRunning = true;
        inputs.Player.Sprint.canceled += ctx => isRunning = false;
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateCameraReference(); // 场景加载后更新相机引用
    }

    void Start()
    {
        UpdateCameraReference();
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        playerAttack = GetComponent<PlayerAttack>();
    }

    private void UpdateCameraReference()
    {
        _mainCamera = Camera.main; // 使用Camera.main自动查找活动相机
        if (_mainCamera == null)
        {
            Debug.LogWarning("Main camera not found, searching by tag...");
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera")?.GetComponent<Camera>();
        }
    }

    private void OnEnable()
    {
        inputs?.Enable();
        isRunning = inputs?.Player.Sprint.ReadValue<float>() > 0.5f || 
                  Keyboard.current?.leftShiftKey.isPressed == true ||
                  Keyboard.current?.rightShiftKey.isPressed == true;
    }

    private void OnDisable()
    {
        inputs?.Disable();
        inputs.Player.Sprint.started -= ctx => isRunning = true;
        inputs.Player.Sprint.canceled -= ctx => isRunning = false;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        // 确保相机存在
        if (_mainCamera == null)
        {
            UpdateCameraReference();
            if (_mainCamera == null) return;
        }

        bool keyboardSprint = Keyboard.current?.leftShiftKey.isPressed == true || 
                            Keyboard.current?.rightShiftKey.isPressed == true;
        bool inputSystemSprint = inputs?.Player.Sprint.ReadValue<float>() > 0.5f;
        isRunning = keyboardSprint || inputSystemSprint;

        if (GetComponent<PlayerState>()?.isTakingDamage == true || 
           (playerAttack != null && playerAttack.isAttacking))
        {
            _controller?.Move(Vector3.zero);
            return;
        }

        Move();
    }

    void Move()
    {
        if (_mainCamera == null || _controller == null || _animator == null) return;

        Vector3 velocity = new Vector3(0, -1, 0);
        _animator.SetBool("isRunning", isRunning);
        currentSpeed = isRunning ? runSpeed : walkSpeed;

        if (_move != Vector2.zero)
        {
            Vector3 inputDir = new Vector3(_move.x, 0.0f, _move.y).normalized;
            _targetRot = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg +
                         _mainCamera.transform.eulerAngles.y;

            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRot,
                ref _rotationVelocity, RotationSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

            Vector3 targetDir = Quaternion.Euler(0.0f, _targetRot, 0.0f) * Vector3.forward;
            velocity += targetDir.normalized * (currentSpeed * Time.deltaTime);
            float speedValue = _move.magnitude * (isRunning ? runSpeed : walkSpeed) / runSpeed;
            _animator.SetFloat("Speed", speedValue);
        }
        else
        {
            _animator.SetFloat("Speed", 0);
        }

        _controller.Move(velocity);
    }

    void OnMove(InputValue value)
    {
        _move = value?.Get<Vector2>() ?? Vector2.zero;
    }
}