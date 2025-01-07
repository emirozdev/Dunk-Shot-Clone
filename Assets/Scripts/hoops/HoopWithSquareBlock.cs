using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoopWithSquareBlock : MonoBehaviour
{
    
    private EdgeCollider2D edgeCollider;
    private CircleCollider2D circleCollider;
    private GameObject hoopBlock;
    public CircleCollider2D currentBallsCircleCollider;
    public EdgeCollider2D currentBallsEdgeCollider;
    
    void Start()
    {
        hoopBlock = GameObject.FindGameObjectWithTag("SquareBlock");
        edgeCollider=GetComponent<EdgeCollider2D>();
        circleCollider=GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Ball"))
        {
            currentBallsCircleCollider = circleCollider;
            currentBallsEdgeCollider = edgeCollider;
            edgeCollider.enabled = false;
            circleCollider.enabled = false;
            Destroy(hoopBlock);
            

        }
    }
}
