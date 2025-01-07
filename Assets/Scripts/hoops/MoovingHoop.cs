using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MoovingHoop : MonoBehaviour
{
    //Prefabs
    public GameObject point;
    //Vectors
    private Vector3 initialPosA;
    private Vector3 initialPosB;
    public Transform PosA;
    public Transform PosB;
    //Variables
    [SerializeField] private float moveSpeed;
    //Bools
    private bool moveTowards = true;
    private bool ballISInside = false;
    //GameObjects
    public Ball ball;
    private EdgeCollider2D edgeCollider;
    private CircleCollider2D circleCollider;
    public LineRenderer lineRenderer;
    public CircleCollider2D currentBallsCircleCollider;
    public EdgeCollider2D currentBallsEdgeCollider;
   
    void Start()
    {
        initialPosA = PosA.position;
        initialPosB = PosB.position;    
        edgeCollider=GetComponentInChildren<EdgeCollider2D>();
        circleCollider=GetComponentInChildren<CircleCollider2D>();
        lineRenderer=GetComponent<LineRenderer>();

        
    }
    private void Update()
    {
        if(!ballISInside)
        {
            updateLineRenderer();
            if (moveTowards)
            {

                transform.position = Vector3.MoveTowards(transform.position, initialPosA, moveSpeed * Time.deltaTime);
                if (Vector3.Distance(initialPosA, transform.position) < 0.01f)
                {
                    moveTowards = false;
                }
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, initialPosB, moveSpeed * Time.deltaTime);
                if (Vector3.Distance(initialPosB, transform.position) < 0.01f)
                {
                    moveTowards = true;
                }
            }
        }
       
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if(collision.CompareTag("Ball"))
        {
            ballISInside = true;
            currentBallsCircleCollider = circleCollider;
            currentBallsEdgeCollider = edgeCollider;
            edgeCollider.enabled = false;
            circleCollider.enabled = false;
            lineRenderer.enabled = false;
        }
    }
    private void updateLineRenderer()
    {
        lineRenderer.SetPosition(0, initialPosA);
        lineRenderer.SetPosition(1, initialPosB);
    }
}
