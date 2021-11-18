using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    // initialize rigidbody of vehicle
    [SerializeField] private Rigidbody carRB;
    [SerializeField] private float DownForceValue;
    private GameObject centerOfMass;

    // variable settings for vehicles
    [SerializeField] private float maxMotorTorque; // maximum torque the motor can apply to wheels
    [SerializeField] private float maxSteeringAngle; // maximum steer angle wheel can have
    [SerializeField] private float brakingForce; // force the brake can apply to wheels
    [SerializeField] private float currentSpeed; // this is for checking in unity inspector -> the current speed the vehicle is at
    [SerializeField] private float topSpeed; // set top speed of vehicle

    public bool isBraking; // check if the car is using brake
    
    // on default, this is 0f, because the value only adds on
    // when player hits the brakes
    // this appears in the inspector to check if brakes are being applied
    public float currentBrakingForce;

    // variables for input settings
    private float horizontalInput;
    private float verticalInput;

    // wheel colliders and transform
    // colliders is used to make the vehicle move
    // transform is used to make the meshes rotate
    [SerializeField] private WheelCollider frontLeftWC;
    [SerializeField] private WheelCollider frontRightWC;
    [SerializeField] private WheelCollider rearLeftWC;
    [SerializeField] private WheelCollider rearRightWC;

    [SerializeField] private Transform frontLeftTF;
    [SerializeField] private Transform frontRightTF;
    [SerializeField] private Transform rearLeftTF;
    [SerializeField] private Transform rearRightTF;

    void Start()
    {
        carRB = carRB.GetComponent<Rigidbody>();
    }

    // update the physics of the vehicle here
    void FixedUpdate()
    {
        GetInput();
        CarMotor();
        CarSteering();
        UpdateWheelRotation();
        AddDownForce();
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal"); // car turns
        verticalInput = Input.GetAxisRaw("Vertical"); // car moves
        isBraking = Input.GetKey(KeyCode.Space); // when space key is hit, isBraking = true
    }

    private void CarMotor()
    {
        currentSpeed = Mathf.RoundToInt(carRB.velocity.magnitude * 3.6f);

        if (currentSpeed < topSpeed)
        {
            // make car an All-Wheel drivetrain by making
            // both front and rear wheels use the motor
            frontLeftWC.motorTorque = maxMotorTorque * verticalInput;
            frontRightWC.motorTorque = maxMotorTorque * verticalInput;
            rearLeftWC.motorTorque = maxMotorTorque * verticalInput;
            rearRightWC.motorTorque = maxMotorTorque * verticalInput;
        }
        else
        {
            // prevent car from adding more velocity once reach top speed
            frontLeftWC.motorTorque = 0;
            frontRightWC.motorTorque = 0;
            rearLeftWC.motorTorque = 0;
            rearRightWC.motorTorque = 0;
        }

        currentBrakingForce = isBraking ? brakingForce : 0f;
        ApplyBraking();
    }

    private void ApplyBraking()
    {
        frontLeftWC.brakeTorque = currentBrakingForce;
        frontRightWC.brakeTorque = currentBrakingForce;
        rearLeftWC.brakeTorque = currentBrakingForce;
        rearRightWC.brakeTorque = currentBrakingForce;
    }

    private void CarSteering()
    {
        frontLeftWC.steerAngle = maxSteeringAngle * horizontalInput;
        frontRightWC.steerAngle = maxSteeringAngle * horizontalInput;
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

    // initialize UpdateWheel() via this function
    private void UpdateWheelRotation()
    {
        UpdateWheel(frontLeftWC, frontLeftTF);
        UpdateWheel(frontRightWC, frontRightTF);
        UpdateWheel(rearLeftWC, rearLeftTF);
        UpdateWheel(rearRightWC, rearRightTF);
    }

    // car flips around when driving. this is because there is a lack of anti-roll stabilizers
    // hence, using the downforce method, an invisible game object named "mass", representing
    // center of gravity is present. this keeps the car's suspension to be stiff, preventing rollovers
    // (this does not mean suspension physics are gone, as it is present when car is jumping)
    private void AddDownForce()
    {
        carRB.AddForce(-transform.up * DownForceValue * carRB.velocity.magnitude); // add downforce while rigidbody accelerates
        centerOfMass = GameObject.Find("mass"); // find the gameobject mass
        carRB.centerOfMass = centerOfMass.transform.localPosition; // set custom center mass to the local position
                                                                   // (using car's position instead of world position)
    }
}