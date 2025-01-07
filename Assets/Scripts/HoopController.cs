using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoopController : MonoBehaviour
{
    //Prefabs 
    public GameObject[] hoopPrefabs;
    //Vector
    public Vector3[] hoopPositions;
    private Vector3 nextHoopPosition;
    //Scripts
    public Ball ball;
    // Variables
    private int lastIndex;
    //GameObjects
    public GameObject currentHoop;
    public GameObject nextHoop;
    public GameObject prevHoop;

    private void Update()
    {
        DestroyHoop();
    }
    private void DestroyHoop()
    {
        if(prevHoop != null)
        {
          Destroy(prevHoop);
          CreateNextHoop();
        }
    }

    private void CreateNextHoop()
    {
        

        if (ball.HighScore < 10)
        {
            nextHoopPosition = new Vector3(-currentHoop.transform.position.x, currentHoop.transform.position.y + 2f, 0f);
            nextHoop = hoopPrefabs[0];
        }
        else
        {
            int randompositionIndex = Random.Range(0, hoopPositions.Length);
            if(randompositionIndex == lastIndex)
            {
                do
                {
                    randompositionIndex = Random.Range(0, hoopPositions.Length);
                }
                while (randompositionIndex == lastIndex); 

            }
            lastIndex = randompositionIndex;
            nextHoopPosition = hoopPositions[randompositionIndex] + new Vector3(0f, currentHoop.transform.position.y, 0f);
            if (randompositionIndex == 0)
            {
                int randomHoopIndex =new int[] { 0, 4, 7 }[Random.Range(0, 3)];
                nextHoop = hoopPrefabs[randomHoopIndex];
            }
            else if (randompositionIndex == 1)
            {
                int randomHoopIndex = new int[] { 0,1,2,4,5,6,7,8}[Random.Range(0, 8)];
                nextHoop = hoopPrefabs[randomHoopIndex];

            }
            else if (randompositionIndex == 2)
            {
                int randomHoopIndex = new int[] { 0, 1, 2, 3, 5, 6, 7, 8 }[Random.Range(0, 8)];
                nextHoop = hoopPrefabs[randomHoopIndex];

            }
            else if (randompositionIndex == 3)
            {
                int randomHoopIndex = new int[] { 0, 3, 8 }[Random.Range(0, 3)];
                nextHoop = hoopPrefabs[randomHoopIndex];
            }


        }
        
        Instantiate(nextHoop, nextHoopPosition, nextHoop.transform.rotation);
    }
}
