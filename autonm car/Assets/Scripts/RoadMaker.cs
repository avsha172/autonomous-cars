using UnityEngine;
using System.Collections.Generic;

public class RoadGenerator : MonoBehaviour
{
    public GameObject roadSegmentPrefab;
    public float minSegmentLength = 5f;
    public float maxSegmentLength = 20f;
    public float maxSegmentAngle = 30f;
    public int maxIterations = 100;
    public float turnProbability = 0.3f;
    public float turnAngle = 90f;

    private List<GameObject> roadSegments = new List<GameObject>();

    void Start()
    {
        GenerateRoad();
    }

    void GenerateRoad()
    {
        // Create the starting segment
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        GameObject startSegment = Instantiate(roadSegmentPrefab, startPos, startRot);
        roadSegments.Add(startSegment);

        // Generate the rest of the road
        int iteration = 0;
        while (iteration < maxIterations)
        {
            // Get the last segment
            GameObject lastSegment = roadSegments[roadSegments.Count - 1];
            Vector3 lastPos = lastSegment.transform.position;
            Quaternion lastRot = lastSegment.transform.rotation;

            // Generate the next segment
            float segmentLength = Random.Range(minSegmentLength, maxSegmentLength);
            float segmentAngle = Random.Range(-maxSegmentAngle, maxSegmentAngle);
            Vector3 nextPos = lastPos + lastRot * Vector3.forward * segmentLength;
            Quaternion nextRot = lastRot * Quaternion.Euler(0, segmentAngle, 0);
            GameObject nextSegment = Instantiate(roadSegmentPrefab, nextPos, nextRot);

            // Add the next segment
            roadSegments.Add(nextSegment);

            // Check for turn
            if (Random.value < turnProbability)
            {
                // Generate turn segments
                int numTurnSegments = Mathf.RoundToInt(turnAngle / maxSegmentAngle);
                float turnAngleSign = Mathf.Sign(segmentAngle);
                float turnAngleIncrement = turnAngle / numTurnSegments;
                Quaternion turnRot = lastRot;
                for (int i = 0; i < numTurnSegments; i++)
                {
                    // Generate turn segment
                    Vector3 turnPos = lastPos + turnRot * Vector3.forward * segmentLength / numTurnSegments;
                    Quaternion turnNextRot = turnRot * Quaternion.Euler(0, turnAngleSign * turnAngleIncrement, 0);
                    GameObject turnSegment = Instantiate(roadSegmentPrefab, turnPos, turnNextRot);

                    // Add the turn segment
                    roadSegments.Add(turnSegment);

                    // Update turn rotation and position
                    turnRot = turnNextRot;
                    lastPos = turnPos;
                }
            }

            iteration++;
        }

        // Connect the road
        ConnectRoad();
    }

    void ConnectRoad()
    {
        // Make the road continuous
        for (int i = 1; i < roadSegments.Count; i++)
        {
            GameObject currentSegment = roadSegments[i];
            GameObject previousSegment = roadSegments[i - 1];

            Vector3 currentPos = currentSegment.transform.position;
            Quaternion currentRot = currentSegment.transform.rotation;

            Vector3 previousPos = previousSegment.transform.position;
            Quaternion previousRot = previousSegment.transform.rotation;

            Vector3 deltaPos = previousPos + previousRot * Vector3.forward * previousSegment.transform.localScale.z - currentPos;
            Quaternion deltaRot = Quaternion.Inverse(currentRot) * previousRot;

            currentSegment.transform.position += deltaPos;
            currentSegment.transform.rotation *= deltaRot;
        }
    }
}
