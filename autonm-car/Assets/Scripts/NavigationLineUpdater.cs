using System.Linq;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Barmetler.RoadSystem
{
	/// <summary>
	/// Updates a LineRenderer based on a RoadSystemNavigator
	/// </summary>
	[ExecuteAlways, RequireComponent(typeof(LineRenderer))]
	public class NavigationLineUpdater : MonoBehaviour
	{
		[SerializeField] private int NormalDir = 1;
		[SerializeField] private float BarrierDistance;
		[SerializeField] private Vector3 OffsetMeshCollider;
		/*[SerializeField] private int XLeft = 1;
		[SerializeField] private int ZLeft = 1;*/
		[SerializeField] private int Right = 1;
		[SerializeField]
		private RoadSystemNavigator navigator;
		[SerializeField]
		private float Tolerance = 0.1f;
		[SerializeField]
		private float LineWidth = 2;
		[HideInInspector] public Vector3[] points;
		[SerializeField, HideInInspector]
		private LineRenderer lineRenderer;
		private Vector3[] new_points;
		AsyncUpdater<Vector3[]> pathPoints;

		private void OnValidate()
		{
			lineRenderer = GetComponent<LineRenderer>();
		}

		private void Awake()
		{
			OnValidate();
		}

		// Update is called once per frame
		void Start()
		{
			StartCoroutine(CalcBarriers());
		}

		private IEnumerator CalcBarriers()
		{
            while (true)
            {
				yield return new WaitForSeconds(10f / 144);
				pathPoints ??= new AsyncUpdater<Vector3[]>(this, UpdateData, new Vector3[] { }, 10f / 144);
				pathPoints.Update();
				points = pathPoints.GetData();
				if (points.Length != 0)
                {
                    //dataAnalyser.LinePoints = points;
					new_points = new Vector3[points.Length];
					double XChange = points[1].x - points[0].x;
					double ZChange = points[1].z - points[0].z;
					double Teta;
					Teta = System.Math.Acos((ZChange) / System.Math.Sqrt(
					System.Math.Pow(ZChange, 2) +
					System.Math.Pow(XChange, 2)));
					Teta = System.Math.PI / 2 - Teta;
					int Xdir = -1;
					int Zdir = 1;
					if (XChange > 0)
					{
						if (ZChange > 0)
						{
							Zdir = -1;
						}
						else
						{
							Zdir = -1;

						}
					}
					else
					{
						if (ZChange > 0)
						{

						}
						else
						{
						}
					}
					if (System.Math.Sign(XChange) == 0)
					{
						Xdir = 0;
					}
					new_points[0] = new Vector3((float)(points[0].x + Xdir * -Right * BarrierDistance * System.Math.Sin(Teta)), points[0].y, (float)(points[0].z + Zdir * BarrierDistance * Right * System.Math.Cos(Teta)));
					for (int i = 1; i < new_points.Length; i++)
					{
                        ZChange = points[i].z - points[i - 1].z;
                        XChange = points[i].x - points[i - 1].x;
                        Teta = System.Math.Acos((ZChange) / System.Math.Sqrt(
                            System.Math.Pow(ZChange, 2) +
                            System.Math.Pow(XChange, 2)));
						Teta = System.Math.PI / 2 - Teta;
						if (float.IsNaN((float) System.Math.Sin(Teta)) || float.IsNaN((float) System.Math.Cos(Teta))){
							new_points[i] = new_points[i - 1];
	                    }
                        else
                        {
							Xdir = -1;
							Zdir = 1;
							if (XChange > 0) 
							{
								if (ZChange > 0)
								{
									Zdir = -1;
								}
								else
								{
									Zdir = -1;

								}
							}
                            else
                            {
								if (ZChange > 0)
								{

								}
								else
								{
								}
							}
							if (System.Math.Sign(XChange) == 0)
							{
								Xdir = 0;
							}

							new_points[i] = new Vector3((float)(points[i].x + Xdir * -Right * BarrierDistance * System.Math.Sin(Teta)), points[i].y, (float)(points[i].z + Zdir * BarrierDistance * Right * System.Math.Cos(Teta)));
						}
					}
					LineRenderer lineRenderer = GetComponent<LineRenderer>();
					lineRenderer.positionCount = new_points.Length;
					lineRenderer.SetPositions(new_points);
					lineRenderer.widthMultiplier = LineWidth;

					MeshFilter meshFilter = GetComponent<MeshFilter>();
					MeshCollider meshCollider = GetComponent<MeshCollider>();

					// Create a mesh that follows the shape of the line renderer
					Mesh mesh = new Mesh();
					int numVertices = lineRenderer.positionCount * 2;
					Vector3[] vertices = new Vector3[numVertices];
					int[] triangles = new int[(numVertices - 2) * 3];

					for (int i = 0; i < lineRenderer.positionCount; i++)
					{
						Vector3 position = lineRenderer.GetPosition(i) - OffsetMeshCollider;

						// Calculate the vertices for this point on the line
						Vector3 normal = Vector3.Cross(Vector3.forward * NormalDir * Right, lineRenderer.GetPosition((i + 1) % lineRenderer.positionCount) - position).normalized;
						Vector3 vertex1 = position + normal * lineRenderer.startWidth / 2f;
						Vector3 vertex2 = position - normal * lineRenderer.endWidth / 2f;

						vertices[i * 2] = vertex1;
						vertices[i * 2 + 1] = vertex2;

						// Add triangles for this point on the line
						if (i < lineRenderer.positionCount - 1)
						{
							int triangleIndex = i * 6;
							triangles[triangleIndex] = i * 2;
							triangles[triangleIndex + 1] = i * 2 + 1;
							triangles[triangleIndex + 2] = i * 2 + 2;
							triangles[triangleIndex + 3] = i * 2 + 2;
							triangles[triangleIndex + 4] = i * 2 + 1;
							triangles[triangleIndex + 5] = i * 2 + 3;
						}
					}

					mesh.vertices = vertices;
					mesh.triangles = triangles;

					// Assign the mesh to the mesh filter and mesh collider
					meshFilter.mesh = mesh;
					meshCollider.sharedMesh = mesh;
					//meshCollider.sharedMesh.RecalculateBounds();
					break;
                }
            }
		}
		Vector3[] UpdateData()
		{
			var points = navigator.CurrentPoints.Select(e => e.position).ToList();

			LineUtility.Simplify(points.ToList(), Tolerance, points);

			return points.Select(e =>
				Vector3.Scale(e, Vector3.forward + Vector3.right) + Vector3.up * 1
			).ToArray();
		}
	}
}
