using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControl : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 12;
    [SerializeField] private float _rotationSpeed = 4;
    [SerializeField] private float _cameraSmoothness = 10f;

    [SerializeField] private Vector3 _cameraPositionOffset = new(0, 1.7f, 0);

    // Angle constraints for X axis rotation
    [SerializeField] private float[] _cameraRotationVerticalConstraints = { 80f, 280f };

    // Lerping targets
    private Quaternion _cameraTarget;
    private Quaternion _playerTarget;

    [SerializeField] private Camera _playerCamera;

    private Rigidbody _rigidBody;

    public TextMeshProUGUI test;

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
        CameraMove();

        Move();
        
        test.text = "" + _rigidBody.velocity;
    }

    // Camera rotation and movement
    private void CameraMove()
    {
        // Mouse Input
        float deltaXRotation = Input.GetAxis("Mouse Y") * _rotationSpeed;
        float deltaYRotation = Input.GetAxis("Mouse X") * _rotationSpeed;

        // Calculating target rotation
        Vector3 newCameraRotation = _cameraTarget.eulerAngles + new Vector3(deltaXRotation, deltaYRotation, 0);
        Vector3 newPlayerRotation = _playerTarget.eulerAngles + new Vector3(0, deltaYRotation, 0);

        // Rotating player (Y axis)
        _playerTarget.eulerAngles = newPlayerRotation;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, _playerTarget, 1 / _cameraSmoothness * 100 * Time.deltaTime);

        // Checking rotation angles constraints
        if (newCameraRotation.x > _cameraRotationVerticalConstraints[0] && newCameraRotation.x < _cameraRotationVerticalConstraints[1])
            newCameraRotation.x = _cameraTarget.eulerAngles.x;

        // Rotating camera (X axis)
        _cameraTarget.eulerAngles = newCameraRotation;
        _playerCamera.transform.localRotation = Quaternion.Slerp(_playerCamera.transform.localRotation, _cameraTarget, 1 / _cameraSmoothness * 100 * Time.deltaTime);
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

        _rigidBody.AddForce(backwardsMultiplier * forwardMultipllier * _movementSpeed * (100 - (currentSpeed * 50)) * verticalAxis * transform.forward);

        _rigidBody.AddForce(sideMultiplier * _movementSpeed * (100 - (currentSpeed * 50)) * horizontalAxis * transform.right);

        // Move camera
        _playerCamera.transform.position = transform.position + _cameraPositionOffset;
    }
}
