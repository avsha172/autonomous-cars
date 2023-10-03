using UnityEditor;
using UnityEngine;


#if UNITY_EDITOR
[CustomEditor(typeof(CamController))]
public class CameraEditor : Editor
{

    private const int NumberOfRoads = 6;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        CamController camController = (CamController)target;
        if (GUILayout.Button("Generate"))
        {

            for (int i = 0; i < NumberOfRoads; i++)
            {   
                if (camController.ZoomFieldOfView[i] == 0)
                {

                    camController.ZoomCamPosition[i] = camController.CamTransform.position;
                    camController.ZoomSightDistance[i] = camController.camera.farClipPlane;
                    camController.ZoomFieldOfView[i] = camController.camera.fieldOfView;
                    break;
                }
            }
        }
    }
}
#endif