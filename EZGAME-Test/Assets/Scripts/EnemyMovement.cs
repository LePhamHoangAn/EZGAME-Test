using System.Collections;
using UnityEngine;

public class EnemyMovement : MonoBehaviour, AIUnits
{
    // Movement
    public float speed;
    public float stopDistance;
    public float rotationSpeed;
    public Transform target;  // Now it's general: can be Player or Ally
    private Vector3 _offset;
    private bool _isMoving = true;

    // Attack
    private bool _Attacking = false;
    [SerializeField] private GameObject _attackHitbox;
    [SerializeField] private GameObject _attackCollider;

    // Animation
    private Animator _animator;

    private void Start()
    {
        //Opt
        EntitiesManager.Instance.Register(this);


        _offset = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        _attackHitbox.SetActive(false);
        _animator = GetComponent<Animator>();
        speed -= Random.Range(0f, 1f);
    }
    public void Tick()
    {
        FindClosestTarget();
        if (target == null) return;

        Vector3 targetPosition = target.position + _offset;
        Vector3 direction = (targetPosition - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, targetPosition);

        // Don't rotate while attacking
        if (!_Attacking)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }

        if (distance > stopDistance && _isMoving)
        {
            _animator.SetBool("_isMoving", true);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }
        else if (!_Attacking && IsTargetInFront())
        {
            StartCoroutine(Attack());
        }
        else
        {
            _animator.SetBool("_isMoving", false); // Idle while turning
        }
    }

    private void OnDisable()
    {
        EntitiesManager.Instance.Unregister(this);
    }
    void Update()
    {
        
    }

    IEnumerator Attack()
    {
        _Attacking = true;
        _isMoving = false;
        _animator.SetBool("_isMoving", false);
        _attackHitbox.SetActive(true);
        yield return new WaitForSeconds(1f);
        _animator.SetBool("_Attacking", true);
        _attackCollider.SetActive(true);
        yield return new WaitForSeconds(0.7f);
        _attackCollider.SetActive(false);
        _animator.SetBool("_Attacking", false);
        _attackHitbox.SetActive(false);
        _Attacking = false;
        _isMoving = true;
    }

    private bool IsTargetInFront()
    {
        Vector3 toTarget = (target.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, toTarget);
        return angle < 45f;
    }

    private void FindClosestTarget()
    {
        GameObject[] possibleTargets = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] allies = GameObject.FindGameObjectsWithTag("Ally");

        float closestDistance = Mathf.Infinity;
        Transform closest = null;

        foreach (GameObject obj in possibleTargets)
        {
            float dist = Vector3.Distance(transform.position, obj.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closest = obj.transform;
            }
        }

        foreach (GameObject ally in allies)
        {
            float dist = Vector3.Distance(transform.position, ally.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closest = ally.transform;
            }
        }

        target = closest;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f); //Red with custom alpha
        Gizmos.DrawSphere(transform.position, stopDistance);
    }
}
