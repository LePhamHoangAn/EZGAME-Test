using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Movement
    Vector2 _movementVector;
    public float moveSpeed;
    public float rotationSpeed;
    private Quaternion targetRotation;

    // Animation
    private Animator _animator;

    // Attack settings
    public float attackDistance = 2.0f;
    public float attackAngle = 45f;
    public LayerMask enemyLayer;

    private bool _isMoving;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void InputPlayer(InputAction.CallbackContext _context)
    {
        _movementVector = _context.ReadValue<Vector2>();
    }

    private void Update()
    {
        Vector3 _movement = new Vector3(_movementVector.x, 0, _movementVector.y);

        // Rotate if moving
        if (_movementVector.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(_movementVector.x, _movementVector.y) * Mathf.Rad2Deg;
            targetRotation = Quaternion.Euler(0, angle, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            _animator.SetBool("_isMoving", true);
            _isMoving = true;
        }
        else
        {
            _animator.SetBool("_isMoving", false);
            _isMoving = false;
        }

        _movement.Normalize();
        transform.Translate(moveSpeed * _movement * Time.deltaTime, Space.World);

        // New: Distance-based attack check
        CheckForEnemyAndAttack();
    }

    private void CheckForEnemyAndAttack()
    {
        // Find all enemies in a small sphere
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackDistance, enemyLayer);

        bool shouldAttack = false;

        foreach (Collider enemy in hitEnemies)
        {
            Vector3 toEnemy = (enemy.transform.position - transform.position).normalized;
            float angleToEnemy = Vector3.Angle(transform.forward, toEnemy);

            if (angleToEnemy < attackAngle)
            {
                shouldAttack = true;
                break; // Found valid enemy
            }
        }

        if (!_isMoving && shouldAttack)
        {
            _animator.SetBool("_Attacking", true);
        }
        else
        {
            _animator.SetBool("_Attacking", false);
        }
    }

    // OPTIONAL: Visualize attack range in Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}
