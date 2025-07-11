using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitiesManager : MonoBehaviour
{
    public static EntitiesManager Instance;

    private readonly List<AIUnits> _units = new List<AIUnits>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Register(AIUnits unit)
    {
        if (!_units.Contains(unit))
            _units.Add(unit);
    }

    public void Unregister(AIUnits unit)
    {
        if (_units.Contains(unit))
            _units.Remove(unit);
    }

    private void Update()
    {
        for (int i = 0; i < _units.Count; i++)
        {
            _units[i].Tick();
        }
    }
}
