using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackMilestone : MonoBehaviour
{
    [SerializeField]
    private float reward = 0.1f;

    [HideInInspector]
    public bool activated;

    private void OnTriggerEnter(Collider other)
    {
        if (activated) return;
        if (other.isTrigger) return;
        var agent = other.GetComponent<RigidbodyAgent>();
        if (!agent) return;
        agent.AddReward(reward);
        activated = true;
    }
}
