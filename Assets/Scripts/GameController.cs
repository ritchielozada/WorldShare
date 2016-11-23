using UnityEngine;
using System.Collections;
using System.Text;

public class GameController : MonoBehaviour
{   
    public enum SystemState { Normal, Scanning, MarkerFound, MarkerLost, ShutdownScanning }
    public GameObject VuforiaGroup;
    public GameObject TrackingObject;
    public GameObject AnchorObject;

    private SystemState currentState;
    private float LockTime;

    void Start()
    {
        currentState = SystemState.Scanning;
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
                    Instantiate(AnchorObject, TrackingObject.transform.position, TrackingObject.transform.rotation);
                    currentState = SystemState.ShutdownScanning;
                }
                break;
            case SystemState.ShutdownScanning:
                StopScan();
                break;
        }
    }

    public void ScanMarker()
    {
        if(currentState == SystemState.Normal)
        {
            currentState = SystemState.Scanning;
            VuforiaGroup.SetActive(true);
        }
        
    }

    public void StopScan()
    {
        if((currentState == SystemState.Scanning) || (currentState == SystemState.ShutdownScanning))
        {
            currentState = SystemState.Normal;
            VuforiaGroup.SetActive(false);
        }
    }

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
}
