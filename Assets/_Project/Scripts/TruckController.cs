using UnityEngine;

public class TruckController : MonoBehaviour
{
    public WheelCollider frontLeft;
    public WheelCollider frontRight;
    public WheelCollider rearLeft;
    public WheelCollider rearRight;

    public float motorForce = 14000f;  
    public float steerAngle = 40f;    
    public float brakeForce = 1200f;    

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // stability (important for trucks)
        rb.centerOfMass = new Vector3(0, -0.6f, 0);

        // correct Unity physics drag (NOT linearDamping)
        rb.linearDamping = 0.05f;
        rb.angularDamping = 0.8f;
    }

    void FixedUpdate()
    {
        float motorInput = Input.GetAxis("Vertical");
        float steerInput = Input.GetAxis("Horizontal");

        float speed = Vector3.Dot(transform.forward, rb.linearVelocity);

        float motor = motorInput * motorForce;

        // SPEED-BASED STEERING
        float speedFactor = Mathf.Clamp01(1f - (Mathf.Abs(speed) / 25f));
        float steer = steerInput * steerAngle * (0.4f + speedFactor);

        // Steering (front wheels only)
        frontLeft.steerAngle = steer;
        frontRight.steerAngle = steer;

        bool reversingAgainstMotion =
            Mathf.Abs(motorInput) > 0.1f &&
            Mathf.Sign(motorInput) != Mathf.Sign(speed) &&
            Mathf.Abs(speed) > 1f;

        float brake = 0f;

        // natural rolling resistance
        if (Mathf.Abs(motorInput) < 0.05f)
        {
            brake = brakeForce * 0.8f;
        }

        // direction switching brake
        if (reversingAgainstMotion)
        {
            brake = brakeForce * 2f;
            motor = 0f;
        }

        ApplyBrakes(brake);

        // Drive
        rearLeft.motorTorque = motor;
        rearRight.motorTorque = motor;
    }

    void ApplyBrakes(float amount)
    {
        frontLeft.brakeTorque = amount;
        frontRight.brakeTorque = amount;
        rearLeft.brakeTorque = amount;
        rearRight.brakeTorque = amount;
    }
}