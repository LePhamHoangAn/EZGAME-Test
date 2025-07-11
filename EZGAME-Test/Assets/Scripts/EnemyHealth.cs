using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float enemyHealth = 100f;
    private float _currentHealth;

    [SerializeField] private HealthBarUI _healthBarUI;
    [SerializeField] private Rigidbody _rb;

    void Start()
    {
        _currentHealth = enemyHealth;
        _healthBarUI.UpdateHealhBarUI(enemyHealth, _currentHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentHealth <= 0)
        {
            _rb.AddForce(Vector3.up * 10, ForceMode.Impulse);
            Destroy(this.gameObject,0.5f);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            _currentHealth = _currentHealth - 10;
            _healthBarUI.UpdateHealhBarUI(enemyHealth, _currentHealth);

        }
    }

}
