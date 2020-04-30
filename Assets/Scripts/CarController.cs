using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CarController<T> : MonoBehaviour where T : Axle
{
    [SerializeField]
    protected T frontAxle;

    [SerializeField]
    protected T rearAxle;

    [SerializeField]
    protected Vector3 centerOfMass;

    [SerializeField]
    protected float maxTorque;

    [SerializeField]
    protected float maxSteeringAngle;

    [SerializeField]
    protected float maxSpeed;

    protected float torque;
    protected float steerAngle;
    protected float brakeTorque;

    protected Rigidbody rb;

    CarDebugGUI debugGUI;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMass;
        frontAxle.Initialize();
        rearAxle.Initialize();
        debugGUI = new CarDebugGUI(rb);
    }

    protected virtual void Update()
    {
        ProcessInput();

        UpdateAxles();
        //rb.AddForce(Vector3.forward * torque);

        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
    }

    private void FixedUpdate()
    {
        rb.AddForceAtPosition(-transform.up * rb.velocity.magnitude * 0.5f, transform.position + transform.rotation * centerOfMass);
    }

    private void OnGUI()
    {
        debugGUI.OnGUI();
    }

    protected virtual void ProcessInput()
    {
        torque = maxTorque * Input.GetAxis("Vertical");
        steerAngle = maxSteeringAngle * Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            frontAxle.OnBrakeStart();
            rearAxle.OnBrakeStart();
        }
        if (Input.GetButton("Jump"))
        {
            brakeTorque = maxTorque;
        }
        else
        {
            brakeTorque = 0;
        }
        if (Input.GetButtonUp("Jump"))
        {
            frontAxle.OnBrakeStop();
            rearAxle.OnBrakeStop();
        }
    }

    protected virtual void UpdateAxles()
    {
        frontAxle.Update(torque, steerAngle, brakeTorque);
        rearAxle.Update(torque, steerAngle, brakeTorque);
    }

}

public abstract class Axle
{
    public bool motor;
    public bool steering;
    public bool brake;

    public abstract void Initialize();

    public virtual void Update(float torque, float steerAngle, float brakeTorque)
    {
        if (motor && brakeTorque <= 0)
        {
            SetTorque(torque);
        }
        if (brake)
        {
            SetBrakeTorque(brakeTorque);
        }
        if (steering)
        {
            SetSteerAngle(steerAngle);
        }
    }

    protected abstract void SetTorque(float torque);

    protected abstract void SetBrakeTorque(float brakeTorque);

    protected abstract void SetSteerAngle(float steerAngle);

    public abstract void OnBrakeStart();
    public abstract void OnBrakeStop();
}

public class CarDebugGUI
{
    private Rigidbody target;

    private Rect windowRect = new Rect(20, 20, 250, 20);


    public CarDebugGUI(Rigidbody _target)
    {
        target = _target;
    }

    public void OnGUI()
    {
        windowRect = GUILayout.Window(0, windowRect, UpdateWindow, "Car info");
    }

    private void UpdateWindow(int id)
    {
        var vel = target.velocity;
        var magnitude = Mathf.Round(vel.magnitude * 10) / 10;
        vel = new Vector3(
            Mathf.Round(vel.x * 10) / 10,
            Mathf.Round(vel.y * 10) / 10,
            Mathf.Round(vel.z * 10) / 10);


        GUILayout.Label("Velocity: " + magnitude + " " + vel);
    }
}
