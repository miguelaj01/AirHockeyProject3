using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class DefenderAgent : Agent
{
    public Transform puck;
    public Rigidbody puckRb;

    public float moveSpeed = 8f;

    public float minX = 3.1f;
    public float maxX = 5.8f;
    public float minZ = -2.7f;
    public float maxZ = 2.7f;

    private Rigidbody rb;
    private Vector3 startPosition;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
    }

    public override void OnEpisodeBegin()
    {
        transform.position = startPosition;

        //if (rb != null)
       // {
        //    rb.linearVelocity = Vector3.zero;
         //   rb.angularVelocity = Vector3.zero;
      //  }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (puck == null || puckRb == null) return;
        sensor.AddObservation(transform.position.x);
        sensor.AddObservation(transform.position.z);

        sensor.AddObservation(puck.position.x);
        sensor.AddObservation(puck.position.z);

        sensor.AddObservation(puckRb.linearVelocity.x);
        sensor.AddObservation(puckRb.linearVelocity.z);

        sensor.AddObservation(puck.position.x - transform.position.x);
        sensor.AddObservation(puck.position.z - transform.position.z);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (actions.ContinuousActions.Length < 2)
            return;

        float moveX = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float moveZ = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

        if (float.IsNaN(moveX) || float.IsNaN(moveZ))
            return;

        Vector3 movement = new Vector3(moveX, 0f, moveZ) * moveSpeed * Time.fixedDeltaTime;
        Vector3 newPosition = transform.position + movement;

        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);
        newPosition.y = startPosition.y;

        transform.position = newPosition;

        AddReward(-0.001f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Puck")
        {
            AddReward(0.25f);

            if (puckRb.linearVelocity.x < 0)
            {
                AddReward(0.25f);
            }
        }
    }
    void FixedUpdate()
    {
        RequestDecision();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> actions = actionsOut.ContinuousActions;

        actions[0] = 0f;
        actions[1] = 0f;

        if (Input.GetKey(KeyCode.LeftArrow)) actions[0] = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) actions[0] = 1f;
        if (Input.GetKey(KeyCode.UpArrow)) actions[1] = 1f;
        if (Input.GetKey(KeyCode.DownArrow)) actions[1] = -1f;
    }
}