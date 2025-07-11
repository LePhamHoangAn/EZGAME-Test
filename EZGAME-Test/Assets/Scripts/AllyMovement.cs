using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AllyMovement : MonoBehaviour, AIUnits
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;
    public float attackRange = 2.0f;

    [Header("Attack")]
    [SerializeField] private GameObject _attackHitbox;
    [SerializeField] private GameObject _attackCollider;

    // Internal
    private Animator _animator;
    private Transform _currentTarget;
    private bool _isAttacking = false;
    private bool _isMoving = true;

    private string enemyTag;
    private GameObject[] _enemiesInScene;

    private void Start()
    {
        //Opt
        EntitiesManager.Instance.Register(this);

        // Auto find enemy tag logic: assume enemies use tag "Enemy"
        enemyTag = "Enemy";

        _animator = GetComponent<Animator>();
        _attackHitbox.SetActive(false);
        _attackCollider.SetActive(false);

        // Cache all enemies in scene at start if you prefer (optional)
        _enemiesInScene = GameObject.FindGameObjectsWithTag(enemyTag);
    }

    public void Tick()
    {
        FindNearestEnemy();

        if (_currentTarget == null)
        {
            _animator.SetBool("_isMoving", false);
            return;
        }

        Vector3 direction = (_currentTarget.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, _currentTarget.position);

        if (!_isAttacking)
        {
            // Rotate toward enemy
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }

        if (distance > attackRange && !_isAttacking)
        {
            // Move closer
            transform.position = Vector3.MoveTowards(transform.position, _currentTarget.position, moveSpeed * Time.deltaTime);
            _animator.SetBool("_isMoving", true);
        }
        else if (!_isAttacking && IsTargetInFront())
        {
            StartCoroutine(Attack());
        }
        else
        {
            _animator.SetBool("_isMoving", false);
        }

    }

    private void OnDisable()
    {
        EntitiesManager.Instance.Unregister(this);
    }
    private void Update()
    {
    }

    private void FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float closestDistance = Mathf.Infinity;
        Transform nearest = null;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                nearest = enemy.transform;
            }
        }

        _currentTarget = nearest;
    }

    private bool IsTargetInFront()
    {
        if (_currentTarget == null) return false;

        Vector3 toTarget = (_currentTarget.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, toTarget);
        return angle < 45f;
    }

    IEnumerator Attack()
    {
        _isAttacking = true;
        _isMoving = false;
        _animator.SetBool("_isMoving", false);
        _attackHitbox.SetActive(true);

        yield return new WaitForSeconds(0.5f); // Windup
        _animator.SetBool("_Attacking", true);
        _attackCollider.SetActive(true);

        yield return new WaitForSeconds(0.7f); // Attack duration
        _attackCollider.SetActive(false);
        _animator.SetBool("_Attacking", false);
        _attackHitbox.SetActive(false);

        _isAttacking = false;
        _isMoving = true;
    }
}