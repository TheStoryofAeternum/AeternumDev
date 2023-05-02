
using UnityEngine;

/* 
 * Created: 1 May 2023
 * Last Change: 2 May 2023, Seth Martineau
 * 
 * A draft for third-person view of player. 
 * A temp created to test implementation without 
 * altering existing first person controller.
 *
 * Author: Seth Martineau
*/
public class ThirdPersonCamera : MonoBehaviour
{

    // Substitute for private fields from FirstPersonController.cs
    private bool invertCamera = false;
    private bool isWalking;
    private bool isCrouched;
    private bool isSprinting;
    private float mouseSensitivity = 2f;
    private float maxLookAngle = 50f;
    private float minDistance = 0.2f;
    private float maxDistance = 5f;
    private float distance;
    private float height;
    private float yaw = 0.0f;
    private float pitch = 0.0f;

    // Internal references
    private FirstPersonController _fpc;
    private Rigidbody _rb;

    // Internal fields
    private float timer;

    void Start()
    {
        _fpc = GetComponent<FirstPersonController>();
        _rb = GetComponent<Rigidbody>();
        _fpc.playerCamera.cullingMask -= 1 << LayerMask.NameToLayer("FirstPersonView");
    }

    void Update()
    {

        // Mouse look copied from FirstPersonController.cs
        _fpc.cameraCanMove = false;
        _fpc.enableHeadBob = false;

        yaw = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * mouseSensitivity;

        if (!invertCamera)
        {
            pitch -= mouseSensitivity * Input.GetAxis("Mouse Y");
        }
        else
        {
            // Inverted Y
            pitch += mouseSensitivity * Input.GetAxis("Mouse Y");
        }

        // Clamp pitch between lookAngle
        pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);


        // Enforce distance minimum.
        if(distance < minDistance) 
        {
            distance = 0;

            _fpc.playerCamera.cullingMask |= 1 << LayerMask.NameToLayer("FirstPersonView");
            _fpc.playerCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("ThirdPersonView"));

            if(Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                distance = minDistance + 0.1f;
            }
        }
        else
        {
            distance += Input.GetAxis("Mouse ScrollWheel");
            if(distance > maxDistance) distance = maxDistance;

            _fpc.playerCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("FirstPersonView"));
            _fpc.playerCamera.cullingMask |= 1 << LayerMask.NameToLayer("ThirdPersonView");
        }
        

        Vector3 jointPosition = new Vector3(
            0, 
            height + distance * Mathf.Cos((_fpc.playerCamera.transform.localEulerAngles.x - 90) * Mathf.Deg2Rad), 
            distance * Mathf.Sin((_fpc.playerCamera.transform.localEulerAngles.x - 90) * Mathf.Deg2Rad)
        );

        // Adjust distance if obstacles are present.
        RaycastHit hit; 
        if(Physics.Raycast(transform.position, transform.rotation * jointPosition, out hit, distance + 1))
        {
            jointPosition.y = height + (hit.distance - 1 > minDistance ? hit.distance - 1 : minDistance) * Mathf.Cos((_fpc.playerCamera.transform.localEulerAngles.x - 90) * Mathf.Deg2Rad);
            jointPosition.z = (hit.distance - 1 > minDistance ? hit.distance - 1 : minDistance) * Mathf.Sin((_fpc.playerCamera.transform.localEulerAngles.x - 90) * Mathf.Deg2Rad);
        }

        Debug.DrawRay(transform.position, transform.rotation * jointPosition, Color.yellow);


        // Rotate the player on yaw axis
        transform.localEulerAngles = new Vector3(0, yaw, 0);

        // Rotate camera on pitch axis
        _fpc.playerCamera.transform.localEulerAngles = new Vector3(pitch, 0, 0);

        // HeadBob
        _fpc.joint.localPosition = HeadBob(jointPosition);
    }

    private void FixedUpdate()
    {
        isSprinting = false;
        isCrouched = false;
        isWalking = false;

        // Get movement state based on rigidbody for headbob rate.

        if (_rb.velocity.magnitude > 0.1)
        {
            isWalking = true;
        }

        if (_fpc.enableSprint && Input.GetKey(_fpc.sprintKey))
        {
            if (_rb.velocity.x != 0 || _rb.velocity.z != 0)
            {
                isSprinting = true;
            }
        }

        else if(_fpc.enableCrouch && Input.GetKey(_fpc.crouchKey))
        {
            if (_rb.velocity.x != 0 || _rb.velocity.z != 0)
            {
                isCrouched = true;
            }
        }
    }
    

    // Modified copy of private method from FirstPersonController.cs
    private Vector3 HeadBob(Vector3 targetPosition)
    {
        Vector3 bobAmount = _fpc.bobAmount * 0.1f;

        if(isWalking)
        {
            // Calculates HeadBob speed during sprint
            if(isSprinting)
            {
                timer += Time.deltaTime * (_fpc.bobSpeed + _fpc.sprintSpeed);
            }
            // Calculates HeadBob speed during crouched movement
            else if (isCrouched)
            {
                timer += Time.deltaTime * (_fpc.bobSpeed * _fpc.speedReduction);
            }
            // Calculates HeadBob speed during walking
            else
            {
                timer += Time.deltaTime * _fpc.bobSpeed;
            }
            // Applies HeadBob movement
            return new Vector3(targetPosition.x + Mathf.Sin(timer) * bobAmount.x, targetPosition.y + Mathf.Sin(timer) * bobAmount.y, targetPosition.z + Mathf.Sin(timer) * bobAmount.z);
        }
        else
        {
            // Resets when play stops moving
            timer = 0;
            return new Vector3(Mathf.Lerp(_fpc.joint.localPosition.x, targetPosition.x, Time.deltaTime * _fpc.bobSpeed), Mathf.Lerp(_fpc.joint.localPosition.y, targetPosition.y, Time.deltaTime * _fpc.bobSpeed), Mathf.Lerp(_fpc.joint.localPosition.z, targetPosition.z, Time.deltaTime * _fpc.bobSpeed));
        }
    }
}
