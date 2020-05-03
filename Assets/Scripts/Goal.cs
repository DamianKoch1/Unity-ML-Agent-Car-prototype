using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : TrackMilestone
{
    [SerializeField]
    private GameObject milestonesParent;

    private TrackMilestone[] milestones;

    protected override void Start()
    {
        base.Start();
        milestones = milestonesParent.GetComponentsInChildren<TrackMilestone>();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
        var agent = other.GetComponent<RigidbodyAgent>();
        if (!agent) return;
        if (!CanFinishTrack(agent))
        {
            base.OnTriggerEnter(other);
            return;
        }
        agent.SetReward(1);
        agent.EndEpisode();
        print("Goal reached");
    }

    private bool CanFinishTrack(RigidbodyAgent agent)
    {
        foreach (var milestone in milestones)
        {
            if (!milestone.enteredPlayers.Contains(agent)) return false;
        }
        return true;
    }
}
