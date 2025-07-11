using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float playerHealth=100f;
    private float _currentHealth;

    [SerializeField] private HealthBarUI _healthBarUI;
    // Start is called before the first frame update
    void Start()
    {
        _currentHealth=playerHealth;
        _healthBarUI.UpdateHealhBarUI(playerHealth, _currentHealth);    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetPlayerHealth(float health)
    {
        _currentHealth=health;
        _healthBarUI.UpdateHealhBarUI(playerHealth, _currentHealth);

    }
    public float GetPlayerHealth()
    {
        return _currentHealth;
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyAttack"))
        {
            _currentHealth=_currentHealth - 10;
            _healthBarUI.UpdateHealhBarUI(playerHealth, _currentHealth);

        }
    }
}
