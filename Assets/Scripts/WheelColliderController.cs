using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelColliderController : CarController<WheelColliderAxle>
{
  
    protected override void Start()
    {
        base.Start();
        frontAxle.wheelL.ConfigureVehicleSubsteps(5f, 12, 15);
    }

    protected override void Update()
    {
        base.Update();
    }

   
}

[System.Serializable]
public class WheelColliderAxle : Axle
{
    public WheelCollider wheelL;
    public WheelCollider wheelR;

    [HideInInspector]
    public Transform meshL;

    [HideInInspector]
    public Transform meshR;

    public WheelFrictionCurve baseFriction;

    public FrictionSettings brakeSidewaysFriction;

    public override void Initialize()
    {
        meshL = wheelL.transform.GetChild(0);
        meshR = wheelR.transform.GetChild(0);
        baseFriction = wheelL.sidewaysFriction;
    }

    public override void Update(float torque, float steerAngle, float brakeTorque)
    {
        base.Update(torque, steerAngle, brakeTorque);
        ApplyToMeshes();
    }

    public override void OnBrakeStart()
    {
        var sidewaysFriction = wheelL.sidewaysFriction;
        sidewaysFriction = brakeSidewaysFriction.Apply(sidewaysFriction);
        wheelL.sidewaysFriction = sidewaysFriction;
        wheelR.sidewaysFriction = sidewaysFriction;
    }

    public override void OnBrakeStop()
    {
        wheelL.sidewaysFriction = baseFriction;
        wheelR.sidewaysFriction = baseFriction;
    }

    public void ApplyToMeshes()
    {
        if (meshL)
        {
            wheelL.GetWorldPose(out var pos, out var rot);
            meshL.transform.position = pos;
            meshL.transform.rotation = rot;
        }
        if (meshR)
        {
            wheelR.GetWorldPose(out var pos, out var rot);
            meshR.transform.position = pos;
            meshR.transform.rotation = rot;
        }
    }

    protected override void SetTorque(float torque)
    {
        wheelL.motorTorque = torque;
        wheelR.motorTorque = torque;
    }

    protected override void SetBrakeTorque(float brakeTorque)
    {
        wheelL.brakeTorque = brakeTorque;
        wheelR.brakeTorque = brakeTorque;
    }

    protected override void SetSteerAngle(float steerAngle)
    {
        wheelL.steerAngle = steerAngle;
        wheelR.steerAngle = steerAngle;
    }
}

[System.Serializable]
public class FrictionSettings
{
    public float extremumSlip;
    public float extremumValue;
    public float stiffness;

    public FrictionSettings(WheelFrictionCurve wfc)
    {
        extremumSlip = wfc.extremumSlip;
        extremumValue = wfc.extremumValue;
        stiffness = wfc.stiffness;
    }

    public WheelFrictionCurve Apply(WheelFrictionCurve wfc)
    {
        wfc.extremumSlip = extremumSlip;
        wfc.extremumValue = extremumValue;
        wfc.stiffness = stiffness;
        return wfc;
    }
}