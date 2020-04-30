using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyController : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField]
    private float maxForwardForce;

    [SerializeField]
    private float rotationForce;

    [SerializeField]
    private float brakeStrength;

    private RigidbodySettings baseRbSettings;

    [SerializeField]
    private RigidbodySettings brakeRbSettings;

    private bool braking;

    CarDebugGUI debugGUI;

    [SerializeField]
    private GameObject[] wheels;

    [SerializeField]
    private ParticleSystem[] driftParticles;
    

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        baseRbSettings = new RigidbodySettings(rb);
        debugGUI = new CarDebugGUI(rb);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            OnBrakeStart();
        }
        if (Input.GetButtonUp("Jump"))
        {
            OnBrakeStop();
        }
    }

    void FixedUpdate()
    {
        rb.AddForce(rb.drag * Physics.gravity, ForceMode.Acceleration);

        if (!IsGrounded()) return;

        var inputX = Input.GetAxis("Horizontal");
        var inputZ = Input.GetAxis("Vertical");


        Movement(inputZ, inputX);

        if (braking)
        {
            BrakeMovement(inputZ, inputX);
        }

    }

    private void OnGUI()
    {
        debugGUI.OnGUI();
    }

    private void OnBrakeStart()
    {
        braking = true;
        brakeRbSettings.Apply(rb);
        foreach (var vfx in driftParticles)
        {
            vfx.Play();
        }
    }

    private void OnBrakeStop()
    {
        braking = false;
        baseRbSettings.Apply(rb);
        foreach (var vfx in driftParticles)
        {
            vfx.Stop();
        }
    }

    private void Movement(float forward, float steer)
    {
        var dir = transform.forward;
        dir.y = 0;
        dir.Normalize();
        rb.AddForce(dir * maxForwardForce * forward);

        if (rb.velocity.magnitude > 2)
        {
            rb.AddRelativeTorque(0, steer * rotationForce, 0, ForceMode.Acceleration);
        }
    }

    private void BrakeMovement(float forward, float steer)
    {
        rb.AddForce(-rb.velocity * brakeStrength, ForceMode.Acceleration);
    }

    private bool IsGrounded()
    {
        int numGroundedWheels = 0;
        foreach (var wheel in wheels)
        {
            if (Physics.Raycast(wheel.transform.position, -Vector3.up, 0.4f))
            {
                if (numGroundedWheels > 0) return true; 
                numGroundedWheels++;
            }
        }
        return false;
    }
}

[System.Serializable]
public class RigidbodySettings
{
    public float drag;
    public float angularDrag;

    public RigidbodySettings(Rigidbody rb)
    {
        drag = rb.drag;
        angularDrag = rb.angularDrag;
    }

    public void Apply(Rigidbody rb)
    {
        rb.drag = drag;
        rb.angularDrag = angularDrag;
    }
}
