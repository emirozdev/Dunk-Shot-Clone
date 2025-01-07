using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class Ball : MonoBehaviour
{
    // Scripts
    public HoopController hoopController;
    public Hoop hoop;
    public HoopWithCircleBlock hoopWithCircleBlock;
    public HoopWithSquareBlock hoopWithSquareBlock;
    public MoovingHoop MoovingHoop;
    // Vectors
    private Vector3 worldPositon;
    private Vector3 touchPosition;
    private Vector2 pivotPosition;
    private Vector3 cameraTargetPosition;
    private Vector3 velocity= Vector3.zero;

    // GameObjects
    private Rigidbody2D ballRigidBody;
    private Camera mainCamera;
    private SpringJoint2D ballSpringJoint;
    public Rigidbody2D pivotPoint;
    public GameObject point;
    private GameObject[] points;

    // Variables
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private float launchDelayTime;
    [SerializeField] private float maxDraggingDistance;
    [SerializeField] private float spaceBetweenPoints;
    [SerializeField] private int numberOfPoints;
    [SerializeField] private float smoothTime;
    public int HighScore = 0;
    private int scoreValue = 2;
    private int multipleValue = 1;
    // Bool
    private bool isDragging = false;
    private bool isFlying = false;
    private bool moveCamera=false;
    private void Start()
    {
        mainCamera = Camera.main;
        ballSpringJoint = GetComponent<SpringJoint2D>();
        ballRigidBody = GetComponent<Rigidbody2D>();
        ballSpringJoint.connectedBody = pivotPoint;

        points = new GameObject[numberOfPoints];
        for (int i = 0; i < numberOfPoints; i++)
        {
            points[i] = Instantiate(point, worldPositon, Quaternion.identity);
            points[i].SetActive(false); 
        }
    }
    private void Update()
    {
        highScoreText.text = HighScore.ToString();
        if (moveCamera)
        {
            mainCamera.transform.position = Vector3.SmoothDamp(mainCamera.transform.position, cameraTargetPosition, ref velocity, smoothTime);
        }
       
        if (Touchscreen.current.primaryTouch.press.isPressed && !isFlying)
        {
            TouchingTheScreen();
        }
        else
        {
            if (isDragging)
            {
                isDragging = false;
                ballRigidBody.isKinematic = false;
                foreach (var point in points)
                {
                    point.SetActive(false);
                }
                StartCoroutine(LaunchBall());
                isFlying = true;
            }
        }
    }

    private void TouchingTheScreen()
    {
        pivotPosition = pivotPoint.position;
        touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        worldPositon = mainCamera.ScreenToWorldPoint(touchPosition);
        worldPositon.z = 0f;
        isDragging = true;
        ballRigidBody.isKinematic = true;
        
        Vector2 dragPosition = worldPositon;
        Vector2 direction = (dragPosition - pivotPosition).normalized;
        if (isDragging)
        {
            
           float angle = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;

            
            hoopController.currentHoop.transform.rotation = Quaternion.Euler(0, 0, angle - 91.2f);
        }

        float distance = Vector2.Distance(worldPositon, pivotPosition);
        if (distance > maxDraggingDistance)
        {
            dragPosition = (pivotPosition + direction * maxDraggingDistance);
        }

        ballRigidBody.position = dragPosition;

        
        UpdateAimLine(dragPosition, direction);
    }

    private void UpdateAimLine(Vector2 dragPosition, Vector2 direction)
    {

        Vector2 PointPosition(float time)
        {
            
            Vector2 draggingDistance = dragPosition - pivotPosition;
            
            float launchForce = (draggingDistance.magnitude - ballSpringJoint.distance) * ballSpringJoint.frequency;
            Vector2 position = dragPosition + launchForce * time * -direction + (time * time) * 0.07f * Physics2D.gravity;
            return position;
        }
        for (int i = 0; i < numberOfPoints; i++)
        {
            points[i].SetActive(true);
            points[i].transform.position = PointPosition(i * spaceBetweenPoints);   
        }
    }
       IEnumerator LaunchBall()
    {
        yield return new WaitForSeconds(launchDelayTime);
        ballSpringJoint.enabled = false;
        yield return new WaitForSeconds(0.1f);
        if (hoop != null)
        {
            hoop.currentBallsCircleCollider.enabled = true;
            hoop.currentBallsEdgeCollider.enabled = true;
        }
        else if (hoopWithCircleBlock != null)
        {
            hoopWithCircleBlock.currentBallsCircleCollider.enabled = true;
            hoopWithCircleBlock.currentBallsEdgeCollider.enabled = true;
        }
        else if (hoopWithSquareBlock != null)
        {
            hoopWithSquareBlock.currentBallsCircleCollider.enabled = true;
            hoopWithSquareBlock.currentBallsEdgeCollider.enabled = true;
        }
        else if (MoovingHoop != null)
        {
            MoovingHoop.currentBallsCircleCollider.enabled = true;
            MoovingHoop.currentBallsEdgeCollider.enabled = true;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Hoop"))
        {

            hoop = collision.GetComponent<Hoop>();
            hoopWithCircleBlock = collision.GetComponent<HoopWithCircleBlock>();
            hoopWithSquareBlock = collision.GetComponent<HoopWithSquareBlock>();
            MoovingHoop = collision.GetComponent<MoovingHoop>();

            if (collision.gameObject == hoopController.currentHoop)
                {
                pivotPoint.position = ballRigidBody.position;
                ballSpringJoint.enabled = false; 
                ballSpringJoint.connectedBody = pivotPoint; 
                ballSpringJoint.enabled = true; 
                isFlying = false;
                }
            else
                {
               
                hoopController.prevHoop = hoopController.currentHoop;
                hoopController.currentHoop = collision.gameObject;
                pivotPoint.position = ballRigidBody.position;
                ballSpringJoint.enabled = false; 
                ballSpringJoint.connectedBody = pivotPoint; 
                ballSpringJoint.enabled = true; 
                isFlying = false;
                cameraTargetPosition = mainCamera.transform.position + new Vector3(0, 2f, 0);
                moveCamera = true;
                HighScore += scoreValue*multipleValue;
                multipleValue = 1;
                scoreValue += 1;
                }
        }
        else if (collision.CompareTag("LosingWall"))
        {
            SceneManager.LoadScene(0);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(isFlying)
        {
            if(collision.gameObject.CompareTag("Hoop"))
            {
                scoreValue = 1;
            }
            else if(collision.gameObject.CompareTag("Wall"))
            {
                multipleValue=2;
                
            }
        }
    }
}
