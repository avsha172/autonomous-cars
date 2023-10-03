using UnityEngine;
using System.Collections.Generic;
namespace Barmetler.RoadSystem
{
    public class RoadGenerator : MonoBehaviour
    {

        public GameObject roadSegmentPrefab;
        public float minSegmentLength = 5f;
        public float maxSegmentLength = 20f;
        public float maxSegmentAngle = 30f;
        public float maxHeightDifference = 5f;
        public int maxIterations = 100;

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
                float heightDifference = Random.Range(-maxHeightDifference, maxHeightDifference);
                Vector3 nextPos = lastPos + lastRot * Vector3.forward * segmentLength;
                Quaternion nextRot = lastRot * Quaternion.Euler(0, segmentAngle, 0);
                nextPos.y += heightDifference;
                GameObject nextSegment = Instantiate(roadSegmentPrefab, nextPos, nextRot);

                // Check for intersections
                bool intersects = false;
                foreach (GameObject segment in roadSegments)
                {
                    if (segment != lastSegment)
                    {
                        intersects = true;
                        break;
                    }
                }

                if (!intersects)
                {
                    roadSegments.Add(nextSegment);
                }

                iteration++;
            }
        }

        bool CheckIntersection(GameObject segment1, GameObject segment2)
        {
            // Get the four points that define the two segments
            Vector3 p1 = segment1.transform.position;
            Vector3 p2 = p1 + segment1.transform.forward * segment1.transform.localScale.z;
            Vector3 p3 = segment2.transform.position;
            Vector3 p4 = p3 + segment2.transform.forward * segment2.transform.localScale.z;

            // Calculate the line segment vectors
            Vector3 v1 = p2 - p1;
            Vector3 v2 = p4 - p3;
            Vector3 v3 = p3 - p1;

            // Calculate the determinants
            float d1 = v2.x * v1.z - v2.z * v1.x;
            float d2 = v3.x * v1.z - v3.z * v1.x;
            float d3 = v2.z * v3.x - v2.x * v3.z;

            // Check for intersection
            if (d1 == 0f)
            {
                // Segments are parallel
                return false;
            }

            float t1 = d2 / d1;
            float t2 = d3 / d1;

            if (t1 >= 0f && t1 <= 1f && t2 >= 0f && t2 <= 1f)
            {
                // Segments intersect
                return true;
            }

            return false;
        }
    }
}   