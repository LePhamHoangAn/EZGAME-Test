using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class PlayerMovement : MonoBehaviour
{
    //Movement
    Vector2 _movementVector;
    public float moveSpeed;
    public float rotationSpeed;
    private Quaternion targetRotation;

    //Animation
    private Animator _animator;
    private Rigidbody _rigidbody;
    bool _isMoving;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody= GetComponent<Rigidbody>();
    }
    public void InputPlayer(InputAction.CallbackContext _context)
    {
        _movementVector=_context.ReadValue<Vector2>();
    }
    private void Update()
    {
        /*---------Logic----------*/

        Vector3 _movement = new Vector3(_movementVector.x, 0, _movementVector.y);

        //Don't do anything if there's no movement input
        if (_movementVector.sqrMagnitude > 0.01f)
        {
            //Rotation
            float angle = Mathf.Atan2(_movementVector.x, _movementVector.y) * Mathf.Rad2Deg;
            targetRotation = Quaternion.Euler(0, angle, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            /*---------Animation----------*/
            _animator.SetBool("_isMoving", true);
            _isMoving = true;
        }
        else
        {
            _animator.SetBool("_isMoving", false);
            _isMoving= false;
        }

        //Movement
        _movement.Normalize();
        transform.Translate(moveSpeed * _movement * Time.deltaTime, Space.World);      
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (!_isMoving)
            {
                _animator.SetBool("_Attacking", true);
            }
            else
            {
                _animator.SetBool("_Attacking", false);
            }
        }
    }
}
