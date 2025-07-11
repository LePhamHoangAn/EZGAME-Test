using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image _healthBarImage;
    private Camera _camera;
    private void Start()
    {
        _camera=Camera.main;
    }
    public void UpdateHealhBarUI(float maxHealth,float currenrtHealth)
    {
        _healthBarImage.fillAmount=currenrtHealth/maxHealth;
    }

    private void Update()
    {
        transform.rotation=Quaternion.LookRotation(transform.position - _camera.transform.position);
    }
}
