using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportDriveScript : MonoBehaviour
{
    Rigidbody rb;
    float lastTimeChecked;
    public float antiRoll = 5000f;
    [Header("0 - lewe kolo, 1 - prawe kolo")]
    public WheelCollider[] frontWheels = new WheelCollider[2];
    public WheelCollider[] rearWheels = new WheelCollider[2];

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (transform.up.y > 0.5f || rb.velocity.magnitude > 1) lastTimeChecked = Time.time;
        if (Time.time > lastTimeChecked + 3f) TurnBackCar();
    }
    private void FixedUpdate()
    {
        HoldWheelsOnGround(frontWheels);
        HoldWheelsOnGround(rearWheels);
    }
    private void TurnBackCar()
    {
        transform.position += Vector3.up; //new Vector3(0f, 1f, 0f);
        transform.rotation = Quaternion.LookRotation(transform.forward);
    }
    private void HoldWheelsOnGround(WheelCollider[] wheels)
    {
        WheelHit hit;
        float leftRiding = 1f;
        float rightRiding = 1f;

        bool groundedL = wheels[0].GetGroundHit(out hit);
        if (groundedL)
        {
            leftRiding = (-wheels[0].transform.InverseTransformPoint(hit.point).y
                - wheels[0].radius) / wheels[0].suspensionDistance;
        }
        else leftRiding = 1f;

        bool groundedR = wheels[1].GetGroundHit(out hit);
        if (groundedR)
        {
            rightRiding = (-wheels[1].transform.InverseTransformPoint(hit.point).y
                - wheels[1].radius) / wheels[1].suspensionDistance;
        }
        else rightRiding = 1f;

        float antiRollForce = (leftRiding - rightRiding) * antiRoll;

        if (groundedL)
            rb.AddForceAtPosition(wheels[0].transform.up * -antiRollForce,
                wheels[0].transform.position);
        if (groundedR)
            rb.AddForceAtPosition(wheels[1].transform.up * antiRollForce,
                wheels[1].transform.position);
    }
}
