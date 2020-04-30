using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField]
    private GameObject milestonesParent;

    private TrackMilestone[] milestones;

    private void Start()
    {
        milestones = milestonesParent.GetComponentsInChildren<TrackMilestone>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!MetAllMilestones()) return;
        if (other.isTrigger) return;
        var agent = other.GetComponent<RigidbodyAgent>();
        if (!agent) return;
        agent.SetReward(1);
        agent.EndEpisode();
        print("Goal reached");
    }

    private bool MetAllMilestones()
    {
        foreach (var milestone in milestones)
        {
            if (!milestone.activated) return false;
        }
        return true;
    }
}
