using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;

public class BallPhysics : MonoBehaviour
{
    [SerializeField]
    private Vector3 m_vAimPosition; // vector which will hold the position for the aim of the ball
    [SerializeField]
    private Vector3 m_vInitialVel; // vector for initial velocity of ball
    [SerializeField]
    private TextMeshProUGUI m_ScoreText = null; // to show the score on the screen

    private bool m_bIsBallKicked = false; // bool to know if the ball has already been kicked by the player. if this is set to true it shouldn't be able to be kicked again until the player resets the ball which will set this bool back to false

    private Rigidbody m_rb = null; // rigid body
    private GameObject m_AimDisplay = null; // object used to display where the ball is currently being aimed

    private float m_fDistanceToTarget = 0f; // float holding the distance to the target

    private int m_iScore = 0; // integer to hold the player's score
    private bool m_bIsGoalScored = false; // a bool to be triggered when the ball hits the net. when this happens the score will increase by 1

    private Vector3 vDebugHeading;

    // Start is called before the first frame update
    void Start()
    {
        m_rb = GetComponent<Rigidbody>(); // Makes sure there is a rigidbody in the ball component
        Assert.IsNotNull(m_rb, "Problem: missing Rigidbody component");

        CreateLandingDisplay(); // creates the item used to show the player where the apex of the balls path will be
        m_fDistanceToTarget = (m_AimDisplay.transform.position - transform.position).magnitude;
        m_ScoreText.text = m_iScore.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        HandleUserInput(); // makes the aim location and ball respond to the player

        if (m_AimDisplay != null) // sets the Aim Display's position and Debug Heading
        {
            m_AimDisplay.transform.position = m_vAimPosition;
            vDebugHeading = m_vAimPosition - transform.position;
        }

    }

    private void CreateLandingDisplay() // Creates a visual display of a green cylinder and sets its position and colour before the player begins input on it
    {
        m_AimDisplay = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        m_AimDisplay.transform.position = Vector3.zero;
        m_AimDisplay.transform.localScale = new Vector3(0.5f, 0.1f, 0.5f);
        m_AimDisplay.transform.rotation = Quaternion.Euler(90f, 0f, 0f); // sets rotation so that the display looks correct
          
        m_AimDisplay.GetComponent<Renderer>().material.color = Color.green;
        m_AimDisplay.GetComponent<Collider>().enabled = false; // disables collider to let the ball pass through it
    }

    public void OnKickBall() // the math done when the ball is kicked
    {
        m_fDistanceToTarget = (m_AimDisplay.transform.position - transform.position).magnitude;
        
        float fMaxHeight = m_AimDisplay.transform.position.y;
        float fForwardRange = (m_fDistanceToTarget * 2);
        float fLeftRightRange = (m_AimDisplay.transform.position.x * 2);
        float fVerticalAngle = Mathf.Atan((4 * fMaxHeight) / (fForwardRange));
        float fHorizontalAngle = Mathf.Atan(fLeftRightRange / fForwardRange);

        float fInitialMag = Mathf.Sqrt((2 * Mathf.Abs(Physics.gravity.y) * fMaxHeight)) / (Mathf.Sin(fVerticalAngle));

        // Setting the values for the initial velocity
        m_vInitialVel.x = fInitialMag * Mathf.Cos(fVerticalAngle) * Mathf.Sin(fHorizontalAngle);
        m_vInitialVel.y = fInitialMag * Mathf.Sin(fVerticalAngle);
        m_vInitialVel.z = fInitialMag * Mathf.Cos(fVerticalAngle) * Mathf.Cos(fHorizontalAngle);

        m_rb.velocity = m_vInitialVel;
    }

    private void HandleUserInput()
    {
        if (m_bIsBallKicked == false) // the following inputs will only be tracked when the ball hasnt been kicked yet
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                m_bIsBallKicked = true; // triggers the flag that doesnt let the ball be kicked anymore until reset
                OnKickBall(); // when space is pressed and the ball is still at the start position it kicks the ball
            }

            if (Input.GetKey(KeyCode.A))
            {
                m_vAimPosition.x -= 0.05f; // when pressing the 'a' key it moves the max point of the jump to the left
            }

            if (Input.GetKey(KeyCode.D))
            {
                m_vAimPosition.x += 0.05f; // when pressing the 'd' key it moves the max point of the jump to the right
            }

            if (Input.GetKey(KeyCode.W))
            {
                m_vAimPosition.z += 0.05f; // when pressing the 'w' key it moves the max point of the jump further from the ball
            }

            if (Input.GetKey(KeyCode.S))
            {
                m_vAimPosition.z -= 0.05f; // when pressing the 's' key it moves the max point of the jump closer to the ball
            }

            if (Input.GetKey(KeyCode.Q))
            {
                m_vAimPosition.y += 0.05f; // when pressing the 'q' key it moves the max point of the jump higher 
            }

            if (Input.GetKey(KeyCode.E))
            {
                m_vAimPosition.y -= 0.05f; // when pressing the 'e' key it moves the max point of the jump lower 
            }
        }

        if(Input.GetKey(KeyCode.R) && m_bIsBallKicked == true) // the ball can only reset when it has already been kicked
        {
            reset(); // calls reset function
        }
    }

    void reset()
    {
        transform.position = new Vector3(0.0f, 0.1f, 0.0f); // moves it back to the start
        m_bIsBallKicked = false; // resets the IsBallKicked bool to allow it to be kicked again
        m_rb.velocity = Vector3.zero; // resets velocity to 0
        m_rb.angularVelocity = Vector3.zero; // resets angular velocity to 0
    }

    void OnCollisionEnter(Collision col) // detects collision
    {
        if(col.gameObject.name == "Net") // if collision is with the net
        {
            m_iScore++; // increase score
            m_ScoreText.text = m_iScore.ToString(); // updates UI
            reset(); // call reset function
        }
    }

}
