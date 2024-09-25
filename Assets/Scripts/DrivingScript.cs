using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrivingScript : MonoBehaviour
{
    public WheelScript[] wheels;
    public float torque = 200f;
    public float maxSteerAngle = 30f;
    public float maxBrakeTorque = 500f;
    public float maxSpeed = 150f;
    public Rigidbody rb;
    public float currentSpeed;
    public float previousSpeed = 0f;
    public AudioSource audioSource;
    public int currentGear = 1;
    const float GEAR_FACTOR = 0.05f;

    public void Drive(float accel, float brake, float steer)
    {
        accel = Mathf.Clamp(accel, -1f, 1f);
        steer = Mathf.Clamp(steer, -1f, 1f) * maxSteerAngle;
        brake = Mathf.Clamp(brake, 0f, 1f) * maxBrakeTorque;

        currentSpeed = rb.velocity.magnitude * 3f;

        float thrustTorque = 0f;
        if (currentSpeed < maxSpeed)
            thrustTorque = accel * torque; 

        foreach(WheelScript wheel in wheels)
        {
            wheel.wheelCollider.motorTorque = thrustTorque;
            if (wheel.isFrontWheel)
                wheel.wheelCollider.steerAngle = steer;
            else
                wheel.wheelCollider.brakeTorque = brake;

            Quaternion quat; 
            Vector3 pos;
            wheel.wheelCollider.GetWorldPose(out pos, out quat);
            wheel.wheelModel.transform.position = pos;
            wheel.wheelModel.transform.rotation = quat;
        }
    }

    public void EngineSound()
    {
        if (audioSource.pitch >= 1f && previousSpeed < currentSpeed)
            currentGear++;
        else if (audioSource.pitch <= 0.8f && previousSpeed > currentSpeed && currentGear > 1)
            currentGear--;

        float speedPerc = Mathf.InverseLerp(0f, maxSpeed * currentGear * GEAR_FACTOR, currentSpeed);
        float pitch = Mathf.Lerp(0.3f, 1f, speedPerc);
    }
}
