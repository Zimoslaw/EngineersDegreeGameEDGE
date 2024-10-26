using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;
using UnityEngine.Scripting.APIUpdating;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControl : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 60;
    public float RotationSpeed = 4; // AKA mouse sensitivity
    [SerializeField] private float _cameraSmoothness = 10f;
    public bool InvertYAxis = false;

    [SerializeField] private Vector3 _cameraPositionOffset = new(0, 1.7f, 0);

    // Angle constraints for X axis rotation
    [SerializeField] private float[] _cameraRotationVerticalConstraints = { 70f, 270f };

    [SerializeField] private Camera _playerCamera;
    [SerializeField] private Transform _playerLamp;

    // Lerping targets
    private Quaternion _cameraTarget;
    [SerializeField] private Transform _lampTarget;

    private Rigidbody _rigidBody;

    // Head animator
    [SerializeField] private Animator _animator;

    private float _sprintTimer = 0;

    // Menus UI
    [SerializeField] private GameObject[] menus;

    [SerializeField] private PlayableDirector _gameOverTimeline;

    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = gameObject.GetComponent<Rigidbody>();

        if (_playerCamera.IsUnityNull())
            _playerCamera = Camera.main;

        _cameraTarget = _playerCamera.transform.rotation;

        // Set user settings
        InvertYAxis = PlayerPrefs.GetInt("InvertYAxis") != 0;
        RotationSpeed = PlayerPrefs.GetInt("MouseSensitivity");

        // Locking cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Pausing the game
        if (Input.GetButtonDown("Menu"))
        {
            Pause();
        }

        // Head animation
        if (_animator.IsInTransition(0))
            return;
        float _speed = Vector3.Distance(Vector3.zero, _rigidBody.velocity);
        if (_speed > 0.1f)
        {
            _animator.SetFloat("WalkingSpeed", _speed / 2.24f);
            if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("walking"))
                _animator.SetTrigger("TriggerWalk");
        }
        else
        {
            if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("idle"))
                _animator.SetTrigger("TriggerIdle");
        }
    }

    void FixedUpdate()
    {
        // Cannot move if player is reading a note or has open inventory
        if (!_playerCamera.gameObject.GetComponent<InteractionController>().IsReadingNote &&
            !_playerCamera.gameObject.GetComponent<InteractionController>().IsInventoryOpen)
        {
            CameraMove();

            Move();

            LampMove();
        }
    }

    // Camera rotation and movement
    private void CameraMove()
    {
        // Mouse Input
        float deltaXRotation = Input.GetAxis("Camera Y") * RotationSpeed * (InvertYAxis ? -1 : 1);
        float deltaYRotation = Input.GetAxis("Camera X") * RotationSpeed;

        // Calculating target rotation
        Vector3 newCameraRotation = _cameraTarget.eulerAngles + new Vector3(deltaXRotation, deltaYRotation, 0);

        // Checking rotation angles constraints
        if (newCameraRotation.x > _cameraRotationVerticalConstraints[0] && newCameraRotation.x < _cameraRotationVerticalConstraints[1])
            newCameraRotation.x = _cameraTarget.eulerAngles.x;

        // Rotating cameraObject (X axis)
        _cameraTarget.eulerAngles = newCameraRotation;
        _playerCamera.transform.localRotation = Quaternion.Slerp(_playerCamera.transform.localRotation, _cameraTarget, 1 / _cameraSmoothness * 100 * Time.deltaTime);

        // Rotating playerObkects (Y axis)
        Quaternion newPlayerRotation = _playerCamera.transform.rotation;
        newPlayerRotation.x = 0;
        newPlayerRotation.z = 0;
        transform.rotation = newPlayerRotation;
    }

    // Player movement
    private void Move()
    {
        float verticalAxis = Input.GetAxis("Vertical");
        float horizontalAxis = Input.GetAxis("Horizontal");

        // Slowing down backward movement
        float backwardsMultiplier = verticalAxis < 0 ? 0.5f : 1f;
        // Slowing down forward movement (in case of side movement)
        float forwardMultipllier = horizontalAxis == 0 ? 1f : 0.7f;

        // Slowing down side movement
        float sideMultiplier = verticalAxis == 0 ? 0.8f : 0.4f;

        // For constraining max movement speed
        float currentSpeed = Vector3.Distance(Vector3.zero, _rigidBody.velocity);

        // Sprinting (only for forward movement, for max 5 seconds)
        float sprintMultiplier = 1;
        if (verticalAxis > 0 && horizontalAxis == 0 && Input.GetButton("Run"))
        {
            _sprintTimer += Time.deltaTime;
            if (_sprintTimer < 5)
                sprintMultiplier = 2;
        }
        else
        {
            if (_sprintTimer > 0)
                _sprintTimer -= Time.deltaTime;
            else
                _sprintTimer = 0;
        }


        _rigidBody.AddForce(backwardsMultiplier * forwardMultipllier * _movementSpeed * sprintMultiplier * (2 * _movementSpeed * sprintMultiplier - (currentSpeed * 50)) * verticalAxis * transform.forward);

        _rigidBody.AddForce(sideMultiplier * _movementSpeed * (2 * _movementSpeed - (currentSpeed * 50)) * horizontalAxis * transform.right);

        // Move cameraObject
        _playerCamera.transform.position = transform.position + _cameraPositionOffset;
    }

    // Lamp smooth movement
    private void LampMove()
    {
        if (!_playerLamp.IsUnityNull())
        {
            _playerLamp.position = _lampTarget.position;

            Quaternion newLampRotation = _lampTarget.rotation;

            if (newLampRotation.eulerAngles.x > 310)
            {
                newLampRotation = Quaternion.Euler(_playerLamp.rotation.eulerAngles.x, _lampTarget.rotation.eulerAngles.y, _lampTarget.rotation.eulerAngles.z);
            }

            _playerLamp.rotation = Quaternion.Slerp(_playerLamp.rotation, newLampRotation, 25 * Time.deltaTime);
        }
    }

    // Pause or unpause
    public void Pause()
    {
        if (!menus[0].activeInHierarchy)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            _playerCamera.gameObject.GetComponent<InteractionController>().IsPaused = true;
            Time.timeScale = 0;
            menus[0].SetActive(true);
        }
        else
        {
            menus[0].SetActive(false);
            menus[1].SetActive(false);
            Time.timeScale = 1;
            _playerCamera.gameObject.GetComponent<InteractionController>().IsPaused = false;
            if (!_playerCamera.gameObject.GetComponent<InteractionController>().IsInventoryOpen)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    //Game over
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Demon"))
        {
            _playerCamera.gameObject.GetComponent<InteractionController>().IsReadingNote = true;
            other.GetComponent<NavMeshAgent>().isStopped = true;
            _playerCamera.transform.LookAt(other.transform.position);
            _gameOverTimeline.Play();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
