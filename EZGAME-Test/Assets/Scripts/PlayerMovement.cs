using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    // Movement
    Vector2 _movementVector;
    public float moveSpeed;
    public float rotationSpeed;
    private Quaternion _targetRotation;

    // Animation
    private Animator _animator;
    private bool _isMoving;

    // Attack
    public float attackDistance = 2.0f;
    public float attackAngle = 45f;
    private bool _isAttacking = false;
    [SerializeField] private GameObject _attackCollider;

    // Rigidbody for physics movement
    private Rigidbody _rb;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _attackCollider.SetActive(false);

        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; //Allow Y rotation only
    }

    // Using the new input system
    public void InputPlayer(InputAction.CallbackContext _context)
    {
        _movementVector = _context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Vector3 _movement = new Vector3(_movementVector.x, 0, _movementVector.y);

        if (_movementVector.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(_movementVector.x, _movementVector.y) * Mathf.Rad2Deg;
            _targetRotation = Quaternion.Euler(0, angle, 0);

            _isMoving = true;
            _animator.SetBool("_isMoving", true);
        }
        else
        {
            _isMoving = false;
            _animator.SetBool("_isMoving", false);
        }

        // Physics movement
        _movement.Normalize();
        Vector3 moveAmount = _movement * moveSpeed * Time.fixedDeltaTime;
        _rb.MovePosition(_rb.position + moveAmount);

        // Apply rotation through Rigidbody
        if (_isMoving)
        {
            Quaternion smoothedRotation = Quaternion.Slerp(_rb.rotation, _targetRotation, rotationSpeed * Time.fixedDeltaTime);
            _rb.MoveRotation(smoothedRotation);
        }

        // Attack if idle & enemy in range
        if (!_isMoving && !_isAttacking && IsEnemyInFrontAndInRange())
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        _isAttacking = true;
        _animator.SetBool("_Attacking", true);

        yield return new WaitForSeconds(0.3f);
        _attackCollider.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        _attackCollider.SetActive(false);
        _animator.SetBool("_Attacking", false);

        _isAttacking = false;
    }

    private bool IsEnemyInFrontAndInRange()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Vector3 _toEnemy = (enemy.transform.position - transform.position);
            float distance = _toEnemy.magnitude;

            if (distance <= attackDistance)
            {
                float angleToEnemy = Vector3.Angle(transform.forward, _toEnemy.normalized);
                if (angleToEnemy < attackAngle)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
