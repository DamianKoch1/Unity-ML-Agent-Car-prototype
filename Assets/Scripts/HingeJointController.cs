using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HingeJointController : CarController<HingeJointAxle>
{
    protected override void Start()
    {
        frontAxle.maxSpeed = maxSpeed;
        rearAxle.maxSpeed = maxSpeed;
        base.Start();

    }

}


[System.Serializable]
public class HingeJointAxle : Axle
{
    public HingeJoint jointL;
    public HingeJoint jointR;

    private Rigidbody rbL;
    private Rigidbody rbR;

    [HideInInspector]
    public float maxSpeed;

    public float brakeStrength = 70;

    public override void Initialize()
    {
        rbL = jointL.GetComponent<Rigidbody>();
        rbR = jointR.GetComponent<Rigidbody>();
        rbL.maxAngularVelocity = maxSpeed;
        rbR.maxAngularVelocity = maxSpeed;
    }

    public override void OnBrakeStart()
    {
    }

    public override void OnBrakeStop()
    {
    }

    protected override void SetBrakeTorque(float brakeTorque)
    {
        if (brakeTorque <= 0) return;
        rbL.AddRelativeTorque(-Vector3.right * rbL.angularVelocity.magnitude * brakeStrength);
        rbR.AddRelativeTorque(-Vector3.right * rbL.angularVelocity.magnitude * brakeStrength);

    }

    protected override void SetSteerAngle(float steerAngle)
    {
        if (steerAngle == 0) return;
        jointL.transform.localEulerAngles = new Vector3(0, steerAngle, 0);
        jointR.transform.localEulerAngles = new Vector3(0, steerAngle, 0);
    }

    protected override void SetTorque(float torque)
    {
        rbL.AddRelativeTorque(Vector3.right * torque);
        rbR.AddRelativeTorque(Vector3.right * torque);
    }
}
