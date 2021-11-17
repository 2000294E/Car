using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    // variable setting for vehicles
    public List<AxleInfo> axleInfos;
    public float maxMotorTorque; // maximum torque the motor can apply to wheel
    public float maxSteeringAngle; // maximum steer angle wheel can have

    public float brakes = 0;

    private void CarMovement()
    {
        // get controls to steer, move the car.
        float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");

        // add a checklist to determine attribute of the tyres:
        // 1. the tyres be able to steer?
        // 2. should the tyres be able to add power to move?
        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
            UpdateWheel(axleInfo.leftWheel, axleInfo.leftWheelTransform);
            UpdateWheel(axleInfo.rightWheel, axleInfo.rightWheelTransform);
        }
    }

    private void UpdateWheel(WheelCollider collider, Transform transform)
    {
        // Get wheel collider state
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation); // gets world space pos of wheel accounting for ground contact,
                                                           // suspension limits, steer angle & rotation angle (in degrees)

        // Set wheel transform state
        transform.position = position;
        transform.rotation = rotation;
    }

    // update the physics of the vehicle here
    public void FixedUpdate()
    {
        CarMovement();
    }
}

[System.Serializable]
public class AxleInfo
{
    // add wheel collider option into inspector
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;

    // add mesh rendered wheel option into inspector
    public Transform leftWheelTransform;
    public Transform rightWheelTransform;

    // note: the motor option determines whether car is Front-Wheel, All-Wheel, or Rear-Wheel
    // if front wheel motor option is selected, it is Front-Wheel
    // if both wheels motor option is selected, it is All-Wheel
    // if rear wheel motor option is selected, it is Rear-Wheel
    public bool motor;
    public bool steering;
}