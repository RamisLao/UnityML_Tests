using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField] private Food _foodPrefab;
    [SerializeField] private FoodButton _foodButton;
    [SerializeField] private GetFoodAgent _foodAgent;
    private bool _foodHasSpawned = false;
    private Transform _lastFoodTransform;

    private void Start()
    {
        _foodButton.OnUsed += SpawnFood;
        _foodAgent.OnEpisodeBeginEvent += ResetSpawner;
    }

    private void ResetSpawner(object sender, EventArgs e)
    {
        _foodHasSpawned = false;
        if (_lastFoodTransform != null)
        {
            Destroy(_lastFoodTransform.gameObject);
        }
    }

    private void SpawnFood(object sender, EventArgs e)
    {
        Food food = Instantiate(_foodPrefab, transform.parent);
        food.transform.localPosition = new Vector3(UnityEngine.Random.Range(-5, 5), 3, UnityEngine.Random.Range(-2.5f, 2.5f));
        _foodHasSpawned = true;
        _lastFoodTransform = food.transform;
    }

    public bool HasFoodSpawned()
    {
        return _foodHasSpawned;
    }

    public Transform GetLastFoodTransform()
    {
        return _lastFoodTransform;
    }
}
