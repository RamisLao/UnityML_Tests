using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class GetFoodAgent : Agent
{
    public event EventHandler OnAteFood;
    public event EventHandler OnEpisodeBeginEvent;

    [SerializeField] private FoodSpawner _foodSpawner;
    [SerializeField] private FoodButton _foodButton;
    [SerializeField] private float _moveSpeed;

    private Rigidbody _agentRigidbody;

    private void Awake()
    {
        _agentRigidbody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(UnityEngine.Random.Range(-5f, 5f), 1f, UnityEngine.Random.Range(-2.5f, 2.5f));

        OnEpisodeBeginEvent?.Invoke(this, EventArgs.Empty);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(_foodButton.CanUseButton() ? 1 : 0);
        Vector3 dirToFoodButton = (_foodButton.transform.localPosition - transform.localPosition).normalized;
        sensor.AddObservation(dirToFoodButton.x);
        sensor.AddObservation(dirToFoodButton.z);

        sensor.AddObservation(_foodSpawner.HasFoodSpawned() ? 1 : 0);
        if (_foodSpawner.HasFoodSpawned())
        {
            Vector3 dirToFood = (_foodSpawner.GetLastFoodTransform().localPosition - transform.localPosition).normalized;
            sensor.AddObservation(dirToFood.x);
            sensor.AddObservation(dirToFood.z);
        }
        else
        {
            sensor.AddObservation(0); // x
            sensor.AddObservation(0); // z
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int moveX = actions.DiscreteActions[0]; // 0 = Dont Move; 1 = Left; 2 = Right
        int moveZ = actions.DiscreteActions[1]; // 0 = Dont Move; 1 = Back; 2 = Forward

        Vector3 addForce = new Vector3(0, 0, 0);

        switch(moveX)
        {
            case 0: addForce.x = 0f; break;
            case 1: addForce.x = -1f; break;
            case 2: addForce.x = 1f; break;
        }

        switch (moveZ)
        {
            case 0: addForce.z = 0f; break;
            case 1: addForce.z = -1f; break;
            case 2: addForce.z = 1f; break;
        }

        _agentRigidbody.velocity = addForce * _moveSpeed + new Vector3(0, _agentRigidbody.velocity.y, 0);

        bool isUseButtonDown = actions.DiscreteActions[2] == 1;
        if (isUseButtonDown)
        {
            Collider[] colliderArray = Physics.OverlapBox(transform.position, Vector3.one * .5f);
            foreach (Collider collider in colliderArray)
            {
                if (collider.TryGetComponent(out FoodButton foodButton))
                {
                    if (foodButton.CanUseButton())
                    {
                        foodButton.UseButton();
                        AddReward(1);
                    }
                }
            }
        }

        // For optimizing once the Agent has already learned
        AddReward(-1 / MaxStep);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Food food))
        {
            AddReward(1f);
            Destroy(food.gameObject);
            OnAteFood?.Invoke(this, EventArgs.Empty);

            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        switch (Mathf.RoundToInt(Input.GetAxisRaw("Horizontal")))
        {
            case -1: discreteActions[0] = 1; break;
            case 0: discreteActions[0] = 0; break;
            case 1: discreteActions[0] = 2; break;
        }
        switch (Mathf.RoundToInt(Input.GetAxisRaw("Vertical")))
        {
            case -1: discreteActions[1] = 1; break;
            case 0: discreteActions[1] = 0; break;
            case 1: discreteActions[1] = 2; break;
        }

        discreteActions[2] = Input.GetKey(KeyCode.E) ? 1 : 0;
    }
}
