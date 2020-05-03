using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackMilestone : MonoBehaviour
{
    [SerializeField]
    protected float reward = 1f;

    [SerializeField]
    protected float enterAgainPunishment = -1f;

    public List<RigidbodyAgent> enteredPlayers;

    public TrackMilestone next;

    protected virtual void Start()
    {
        enteredPlayers = new List<RigidbodyAgent>();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
        var agent = other.GetComponent<RigidbodyAgent>();
        if (!agent) return;
        if (enteredPlayers.Contains(agent))
        {
            agent.SetReward(enterAgainPunishment);
            agent.EndEpisode();
            return;
        }
        enteredPlayers.Add(agent);
        agent.AddReward(reward);
        agent.numHitMilestones++;
        agent.target = next;
    }
}
