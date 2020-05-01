using MLAgents;
using MLAgents.Sensors;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class RigidbodyAgent : Agent
{
    private Rigidbody rb;
    private Vector3 startPos;
    private Quaternion startRotation;
    private RigidbodyController controller;

    [SerializeField]
    private float stepPunishment = -0.05f;

    [SerializeField]
    private float collisionPunishment = -0.2f;

    [SerializeField]
    private float forwardReward = 0.02f;

    [SerializeField]
    private float maxStandStillSeconds = 5;

    [SerializeField]
    private GameObject milestonesParent;

    private TrackMilestone[] milestones;

    private DebugGUI debugGUI;

    [SerializeField]
    private bool debug;

    [SerializeField]
    private bool manualControl;

    private Vector3 lastAction = Vector3.zero;

    private Stopwatch standStillStopwatch;

    [SerializeField]
    private GameObject raycastTargetsParent;

    private Vector3 raycastStartPos => raycastTargetsParent.transform.position;

    [SerializeField]
    private LayerMask raycastLayer;

    [SerializeField]
    private float raycastDistance;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        controller = GetComponent<RigidbodyController>();
        controller.controlledByAgent = true;
        startPos = transform.position;
        startRotation = transform.rotation;
        milestones = milestonesParent.GetComponentsInChildren<TrackMilestone>();
        if (debug)
        {
            debugGUI = new DebugGUI();
        }
    }

    public override void OnEpisodeBegin()
    {
        rb.MovePosition(startPos);
        rb.MoveRotation(startRotation);
        rb.velocity = Vector3.zero;
        controller.UpdateFromAgent(0, 0, 0);
        foreach (var milestone in milestones)
        {
            milestone.activated = false;
        }
        standStillStopwatch = null;
    }

    private void OnGUI()
    {
        if (!debug) return;
        if (debugGUI == null) return;
        debugGUI.OnGUI("Velocity: " + debugGUI.LogVector3(rb.velocity), "Reward: " + GetCumulativeReward(), "Action: " + lastAction, "Steps: " + StepCount);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(rb.velocity);
        CollectRaycastObservations(sensor);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        controller.UpdateFromAgent(vectorAction[0], vectorAction[1], vectorAction[2]);

        AddReward(stepPunishment);

        GetMovementRewards(vectorAction);

        CheckLoseConditions();

        if (debug)
        {
            lastAction = new Vector3(vectorAction[0], vectorAction[1], vectorAction[2]);
        }
    }

    private void GetMovementRewards(float[] input)
    {
        if (rb.velocity.magnitude > 5)
        {
            if (input[1] > 0)
            {
                AddReward(forwardReward);
            }
            standStillStopwatch = null;
        }
        else
        {
            if (standStillStopwatch == null)
            {
                standStillStopwatch = Stopwatch.StartNew();
            }
            else if (standStillStopwatch.ElapsedMilliseconds * Time.timeScale > maxStandStillSeconds * 1000)
            {
                SetReward(-1);
                EndEpisode();
            }
        }
    }

    private void CheckLoseConditions()
    {
        if (transform.up.y < 0.3f)
        {
            SetReward(-1);
            EndEpisode();
        }

        if (GetCumulativeReward() < -100)
        {
            SetReward(-1);
            EndEpisode();
        }
    }

    private void CollectRaycastObservations(VectorSensor sensor)
    {
        RaycastHit hit;
        foreach (Transform target in raycastTargetsParent.transform)
        {
            if (Physics.Raycast(raycastStartPos, target.position - raycastStartPos, out hit, raycastDistance, raycastLayer))
            {
                UnityEngine.Debug.DrawLine(raycastStartPos, hit.point, Color.red, 0.2f);
                sensor.AddObservation(true);
                sensor.AddObservation(hit.distance);
            }
            else
            {
                UnityEngine.Debug.DrawLine(raycastStartPos, raycastStartPos + (target.position - raycastStartPos).normalized * raycastDistance, Color.green, 0.2f);
                sensor.AddObservation(false);
                sensor.AddObservation(raycastDistance);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        AddReward(collisionPunishment);
    }

    /// <summary>
    /// For manual testing
    /// </summary>
    /// <returns></returns>
    public override float[] Heuristic()
    {
        if (!manualControl)
        {
            return base.Heuristic();
        }
        var action = new float[3];
        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetAxis("Vertical");
        action[2] = Input.GetButton("Jump") ? 1 : 0;
        return action;
    }

}
