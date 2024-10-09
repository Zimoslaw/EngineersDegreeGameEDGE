using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControl : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 60;
    [SerializeField] private float _rotationSpeed = 4;
    [SerializeField] private float _cameraSmoothness = 10f;

    [SerializeField] private Vector3 _cameraPositionOffset = new(0, 1.7f, 0);

    // Angle constraints for X axis rotation
    [SerializeField] private float[] _cameraRotationVerticalConstraints = { 70f, 270f };


    [SerializeField] private Camera _playerCamera;
    [SerializeField] private Transform _playerLamp;

    // Lerping targets
    private Quaternion _cameraTarget;
    private Quaternion _playerTarget;
    [SerializeField] private Transform _lampTarget;

    private Rigidbody _rigidBody;

    private float _sprintTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = gameObject.GetComponent<Rigidbody>();

        if (_playerCamera.IsUnityNull())
            _playerCamera = Camera.main;

        _cameraTarget = _playerCamera.transform.rotation;
        _playerTarget = transform.rotation;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Cannot move if playerObkects is reading a note
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
        float deltaXRotation = Input.GetAxis("Camera Y") * _rotationSpeed;
        float deltaYRotation = Input.GetAxis("Camera X") * _rotationSpeed;

        // Calculating target rotation
        Vector3 newCameraRotation = _cameraTarget.eulerAngles + new Vector3(deltaXRotation, deltaYRotation, 0);
        //Vector3 newPlayerRotation = _playerTarget.eulerAngles + new Vector3(0, deltaYRotation, 0);

        // Rotating playerObkects (Y axis)
        //_playerTarget.eulerAngles = newPlayerRotation;
        //transform.localRotation = Quaternion.Slerp(transform.localRotation, _playerTarget, 1 / _cameraSmoothness * 100 * Time.deltaTime);

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

        // Sprinting (only for forward movement, for max 10 seconds)
        float sprintMultiplier = 1;
        if (verticalAxis > 0 && horizontalAxis == 0 && Input.GetKey(KeyCode.LeftShift))
        {
            _sprintTimer += Time.deltaTime;
            if (_sprintTimer < 10)
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
        if(!_playerLamp.IsUnityNull())
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
}
