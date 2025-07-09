using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image _healthBarImage;
    public void UpdateHealhBarUI(float maxHealth,float currenrtHealth)
    {
        _healthBarImage.fillAmount=currenrtHealth/maxHealth;
    }
}
