using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackMilestone : MonoBehaviour
{
    [SerializeField]
    protected float reward = 1f;

    public bool activated;

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (activated) return;
        if (other.isTrigger) return;
        var agent = other.GetComponent<RigidbodyAgent>();
        if (!agent) return;
        agent.AddReward(reward);
        activated = true;
    }
}
