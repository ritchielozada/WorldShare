using UnityEngine;
using System.Collections;
using System.Text;
using HoloToolkit.Unity;

public class GameController : MonoBehaviour
{
    public enum SystemState
    {
        Normal,
        Scanning,
        MarkerFound,
        MarkerLost,
        ShutdownScanning,
        PlaceAnchor
    }
    public GameObject VuforiaGroup;
    public GameObject TrackingObject;

    private SpatialMappingManager spatialMappingManager;
    private SystemState currentState;
    private AnchorControl anchorControl;
    private float LockTime;

    #region MONOBEHAVIOR METHODS
    void Start()
    {
        currentState = SystemState.Normal;
        anchorControl = GetComponent<AnchorControl>();
        if (anchorControl == null)
        {
            Debug.LogError("AnchorControl not found");
        }

        spatialMappingManager = SpatialMappingManager.Instance;
        if (spatialMappingManager == null)
        {
            Debug.LogError("This script expects that you have a SpatialMappingManager component in your scene.");
        }
    }

    void Update()
    {
        switch (currentState)
        {
            case SystemState.Scanning:                
                break;
            case SystemState.MarkerFound:
                if (Time.time > LockTime)
                {                                        
                    anchorControl.PlacementObject.transform.position = TrackingObject.transform.position;
                    anchorControl.PlacementObject.transform.rotation = TrackingObject.transform.rotation;
                    currentState = SystemState.ShutdownScanning;
                }
                break;
            case SystemState.ShutdownScanning:
                anchorControl.LockAnchor();
                StopScan();
                break;
            case SystemState.PlaceAnchor:
                // TODO: Use GazeManager + Cursor Tracking instead of another Raycast
                var headPosition = Camera.main.transform.position;
                var gazeDirection = Camera.main.transform.forward;
                RaycastHit hitInfo;
                if (Physics.Raycast(headPosition, gazeDirection, out hitInfo,
                    30.0f, spatialMappingManager.LayerMask))
                {                    
                    anchorControl.PlacementObject.transform.position = hitInfo.point;                    

                    // Rotate this object to face the user.
                    //Quaternion toQuat = Camera.main.transform.localRotation;
                    //toQuat.x = 0;
                    //toQuat.z = 0;
                    //this.transform.rotation = toQuat;
                }
                break;
        }
    }
    #endregion

    #region INTERNAL CONTROL METHODS
    public void FoundMarker(int MarkerId)
    {        
        if ((currentState == SystemState.Scanning) || (currentState == SystemState.MarkerLost))
        {            
            currentState = SystemState.MarkerFound;
            LockTime = Time.time + 3f;      // Stay in view for 3 seconds
        }
    }

    public void LostMarker(int MarkerId)
    {
        if ((currentState == SystemState.Scanning) || (currentState == SystemState.MarkerFound))
        {
            currentState = SystemState.MarkerLost;
        }
    }
    #endregion

    #region CONTROL METHODS
    public void ScanMarker()
    {
        if ((anchorControl.CurentState == AnchorControl.ControlState.Ready) 
            && (currentState == SystemState.Normal))
        {
            currentState = SystemState.Scanning;
            anchorControl.PlaceAnchor();
            VuforiaGroup.SetActive(true);
        }

    }

    public void StopScan()
    {
        if ((anchorControl.CurentState == AnchorControl.ControlState.PlaceAnchor) && 
            ((currentState == SystemState.Scanning) || 
            (currentState == SystemState.ShutdownScanning)))
        {
            currentState = SystemState.Normal;
            anchorControl.LockAnchor();
            VuforiaGroup.SetActive(false);
        }
    }

    public void PlaceAnchor()
    {
        if ((anchorControl.CurentState == AnchorControl.ControlState.Ready) &&
            (currentState == SystemState.Scanning))
        {
            currentState = SystemState.PlaceAnchor;
            anchorControl.PlaceAnchor();            
        }
    }

    public void LockAnchor()
    {
        if ((anchorControl.CurentState == AnchorControl.ControlState.PlaceAnchor) &&
            (currentState == SystemState.PlaceAnchor))
        {
            currentState = SystemState.Normal;
            anchorControl.LockAnchor();            
        }
    }
    #endregion
}
