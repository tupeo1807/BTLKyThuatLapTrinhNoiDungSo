using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class HelicopterMainEngine : MonoBehaviour
{
    Rigidbody helicopterRigid;
    public BladesController MainBlade;
    public BladesController SubBlade;

    private float enginePower;
    public float EnginePower
    {
        get
        {
            return enginePower; 
        }
        set
        {
            MainBlade.BladeSpeed = value * 250;
            SubBlade.BladeSpeed = value * 500;
            enginePower = value;
        }
    }
    public float effectiveHeight;
    public float engineStartSpeed;
    public float EngineLift = 0.01f;

    public float ForwardForce;
    public float BackwardForce;
    public float TurnForce;
    private float TurnForceHelper = 1.5f;
    public float ForwardTiltForce;
    public float TurnTiltForce;

    private Vector2 Movement = Vector2.zero;
    private Vector2 TILTING = Vector2.zero;

    public LayerMask groundLayer;

    private float distanceToGround;
    //set true for using HandleGround function
    public bool isOnGround = true;

    private float turning = 0f;

    public UnityEvent OnTakeOff;
    public UnityEvent Onland;
    bool isFirstTime;

    // Start is called before the first frame update
    void Start()
    {
        helicopterRigid = GetComponent<Rigidbody>(); 
    }

    // Update is called once per frame
    void Update()
    {
        HandleGroundCheck();
        HandleInput();
        HandleInvoks();
        HandleEngine();
    }

    protected void FixedUpdate()
    {
        HelicopterHover();
        HelicopterMovements();
        HelicopterTilting();
    }
    
    void HandleInput()
    {
        if (!isOnGround)
        {
            Movement.x = Input.GetAxis("Horizontal");
            Movement.y = Input.GetAxis("Vertical");

            if (Input.GetKey(KeyCode.C))
            {
                EnginePower -= EngineLift;
                if (EnginePower < 0)
                {
                    EnginePower = 0;
                }
            }
        }

/*        Movement.x = Input.GetAxis("Horizontal");
        Movement.y = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.C))
        {
            EnginePower -= EngineLift;
            if (EnginePower < 0)
            {
                EnginePower = 0;
            }
        }*/

        if (Input.GetAxis("Throttle") > 0)
        {
            EnginePower += EngineLift;
        }
        else if (Input.GetAxis("Vertical") > 0 && isOnGround)
        {
            EnginePower = Mathf.Lerp(EnginePower, 17.5f, 0.003f);
        }
        else if (Input.GetAxis("Throttle") < 0.5f && !isOnGround)
        {
            EnginePower = Mathf.Lerp(EnginePower, 10f, 0.003f);
        }
    }

    void HandleInvoks()
    {
        if (!isOnGround && isFirstTime)
        {
            OnTakeOff.Invoke();
            isFirstTime= false;
        } else if (isOnGround && !! !isFirstTime) 
        {
            Onland.Invoke();
            isFirstTime= true;
        }
    }

    void HandleGroundCheck()
    {
        RaycastHit hit;
        Vector3 direction = transform.TransformDirection(Vector3.down);
        Ray ray = new Ray(transform.position, direction);

        if (Physics.Raycast(ray, out hit, 3000f, groundLayer))
        {
            distanceToGround = hit.distance;
            if (distanceToGround == 0)
            {
                isOnGround = true;
            }
            else
            {
                isOnGround = false;
            }
        }
    }

    void HandleEngine()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            StartEngine();
        } else if(Input.GetKeyDown(KeyCode.Y) /*&& isOnGround*/) 
        {
            StopEngine();
        }
    }
    void HelicopterHover()
    {
        float upForce = 1f - Mathf.Clamp(helicopterRigid.transform.position.y / effectiveHeight,0,1);
        upForce = Mathf.Lerp(0,EnginePower, upForce) * helicopterRigid.mass;
        helicopterRigid.AddRelativeForce(Vector3.up * upForce);
    }

    void HelicopterMovements()
    {
        if (Input.GetAxis("Vertical") > 0)
        {
            helicopterRigid.AddRelativeForce(Vector3.forward * Mathf.Max(0f, Movement.y * ForwardForce * helicopterRigid.mass));
        }
        else if (Input.GetAxis("Vertical") < 0)
        {
            helicopterRigid.AddRelativeForce(Vector3.back * Mathf.Max(0f, -Movement.y * BackwardForce * helicopterRigid.mass));
        }

        float turn = TurnForce * Mathf.Lerp(Movement.x, Movement.x * (TurnForceHelper - Mathf.Abs(Movement.y)), Mathf.Max(0f, Movement.y));
        turning = Mathf.Lerp(turning, turn, Time.fixedDeltaTime * TurnForce);
        helicopterRigid.AddRelativeTorque(0f, turning * helicopterRigid.mass, 0f);
    }
    
    void HelicopterTilting()
    {
        TILTING.y = Mathf.Lerp(TILTING.y, Movement.y * ForwardTiltForce, Time.deltaTime);
        TILTING.x = Mathf.Lerp(TILTING.x, Movement.x * TurnTiltForce, Time.deltaTime);
        helicopterRigid.transform.localRotation = Quaternion.Euler(TILTING.y, helicopterRigid.transform.localEulerAngles.y, -TILTING.x);
    }

    public void StartEngine ()
    {
        DOTween.To(Starting, 0, 8.0f, engineStartSpeed);
    }

    void Starting (float value)
    {
        EnginePower = value;
    }

    public void StopEngine()
    {
        DOTween.To(Stopping, EnginePower, 0.0f, engineStartSpeed);
    }

    void Stopping(float value)
    {
        EnginePower = value;
    }
}
