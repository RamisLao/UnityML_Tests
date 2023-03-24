using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToGoalAgent : Agent
{
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private Material _winMat;
    [SerializeField] private Material _loseMat;
    [SerializeField] private MeshRenderer _floorMeshRenderer;
    [SerializeField] private float _moveSpeed = 1;
    [SerializeField] private Transform _spawnPos;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = _spawnPos.localPosition;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Observation
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(_targetTransform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Decision
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        // Action
        Vector3 direction = new Vector3(moveX, 0, moveZ);
        transform.localPosition += direction * _moveSpeed * Time.deltaTime;
    }

    // To test we can modify the ActionBuffers ourselves
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Goal goal))
        {
            // Reward
            SetReward(1f);
            _floorMeshRenderer.material = _winMat;
            EndEpisode();
        }
        if (other.TryGetComponent(out Wall wall))
        {
            SetReward(-1f);
            _floorMeshRenderer.material = _loseMat;
            EndEpisode();
        }
    }
}
