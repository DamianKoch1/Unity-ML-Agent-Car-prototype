using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyController : MonoBehaviour
{
    [HideInInspector]
    public bool controlledByAgent;

    private Rigidbody rb;

    [SerializeField]
    private float maxForwardForce;

    [SerializeField]
    private float rotateSpeed;

    [SerializeField]
    private float brakeStrength;

    [SerializeField]
    private float driftSidewaysSpeed;

    private RigidbodySettings baseRbSettings;

    [SerializeField]
    private RigidbodySettings brakeRbSettings;

    private bool braking;

    DebugGUI debugGUI;

    [SerializeField]
    private GameObject[] wheels;

    [SerializeField]
    private ParticleSystem[] driftParticles;

    float inputX;

    float inputZ;
    

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        baseRbSettings = new RigidbodySettings(rb);
        debugGUI = new DebugGUI();
    }

    private void Update()
    {
        if (controlledByAgent) return;
        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");
        if (Input.GetButtonDown("Jump"))
        {
            OnBrakeStart();
        }
        if (Input.GetButtonUp("Jump"))
        {
            OnBrakeStop();
        }
    }

    public void UpdateFromAgent(float x, float z, float brake)
    {
        inputX = x;
        inputZ = z;
        if (!braking)
        {
            if (brake > 0)
            {
                OnBrakeStart();
            }
        }
        else if (brake <= 0)
        {
            OnBrakeStop();
        }
    }

    void FixedUpdate()
    {
        rb.AddForce(rb.drag * Physics.gravity, ForceMode.Acceleration);

        if (!IsGrounded()) return;



        Movement(inputZ, inputX);

        if (braking)
        {
            BrakeMovement(inputZ, inputX);
        }

    }

    private void OnGUI()
    {
        if (controlledByAgent) return;
        debugGUI.Display("Velocity: " + debugGUI.LogVector3(rb.velocity));
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

        var localVelocity = transform.InverseTransformDirection(rb.velocity);
        if (localVelocity.z > 2)
        {
            rb.AddRelativeTorque(0, steer * rotateSpeed, 0, ForceMode.Acceleration);
        }
        else if (localVelocity.z < -2f)
        {
            rb.AddRelativeTorque(0, -steer * rotateSpeed, 0, ForceMode.Acceleration);

        }
    }

    private void BrakeMovement(float forward, float steer)
    {
        rb.AddForce(-rb.velocity * brakeStrength, ForceMode.Acceleration);
        if (forward > 0)
        {
            rb.AddForce(-transform.right * driftSidewaysSpeed * steer, ForceMode.Acceleration);
        }
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
