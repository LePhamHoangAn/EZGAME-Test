using System.Collections;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float speed;
    public float stopDistance;
    public float rotationSpeed;
    public Transform player;

    private bool _isMoving = true;
    private bool _Attacking = false;

    [SerializeField] private GameObject _attackHitbox;

    private Vector3 _offset;
    private Animator _animator;

    private void Start()
    {
        _offset = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        _attackHitbox.SetActive(false);
        _animator = GetComponent<Animator>();
        speed = speed - Random.Range(0f, 1f);
    }

    void Update()
    {
        if (player == null) return;

        Vector3 targetPosition = player.position + _offset;
        Vector3 direction = (targetPosition - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, targetPosition);

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
        else if (!_Attacking && IsPlayerInFront())
        {
            StartCoroutine(Attack());
        }
        else
        {
            _animator.SetBool("_isMoving", false); // Idle while turning
        }
    }

    IEnumerator Attack()
    {
        _Attacking = true;
        _isMoving = false;
        _animator.SetBool("_isMoving", false);
        _attackHitbox.SetActive(true);
        yield return new WaitForSeconds(1f);
        _animator.SetBool("_Attacking", true);
        yield return new WaitForSeconds(0.7f);
        _animator.SetBool("_Attacking", false);
        _attackHitbox.SetActive(false);
        _Attacking = false;
        _isMoving = true;
    }

    private bool IsPlayerInFront()
    {
        Vector3 toPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, toPlayer);
        return angle < 45f;
    }



private void OnDrawGizmos()
    {
        // Set the color with custom alpha.
        Gizmos.color = new Color(1f, 0f, 0f,0.3f); // Red with custom alpha

        // Draw the sphere.
        Gizmos.DrawSphere(transform.position, stopDistance);
    }
}
