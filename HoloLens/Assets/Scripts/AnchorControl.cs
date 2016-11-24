using UnityEngine;
using System.Collections;
using System.Text;
using HoloToolkit.Unity;
using UnityEngine.VR.WSA.Persistence;

public class AnchorControl : MonoBehaviour
{
    public GameObject PlacementObject;
    public string SavedAnchorFriendlyName;

    private WorldAnchorManager anchorManager;
    private WorldAnchorStore anchorStore;    
    private TextToSpeechManager ttsMgr;
    
    public enum ControlState
    {
        WaitingForAnchorStore,
        CheckAnchorStatus,
        Ready,
        PlaceAnchor
    }

    public ControlState CurentState;

    // Use this for initialization
    void Start()
    {
        CurentState = ControlState.WaitingForAnchorStore;

        ttsMgr = GetComponent<TextToSpeechManager>();
        if (ttsMgr == null)
        {
            Debug.LogError("TextToSpeechManager Required");
        }

        anchorManager = WorldAnchorManager.Instance;
        if (anchorManager == null)
        {
            Debug.LogError("This script expects that you have a WorldAnchorManager component in your scene.");
        }        

        WorldAnchorStore.GetAsync(AnchorStoreReady);
    }

    void AnchorStoreReady(WorldAnchorStore store)
    {
        anchorStore = store;
        CurentState = ControlState.CheckAnchorStatus;
        Debug.Log("Anchor Store Ready");        
    }
    
    void Update()
    {
        switch (CurentState)
        {            
            case ControlState.CheckAnchorStatus:
                // Anchor Diagnostics
                var cnt = anchorStore.anchorCount;                
                if (cnt > 0)
                {
                    var sb = new StringBuilder("Found Anchor" + (cnt == 1 ? " " : "s "));
                    foreach (var ids in anchorStore.GetAllIds())
                    {
                        sb.Append(ids);
                    }
                    Debug.Log(sb.ToString());
                    ttsMgr.SpeakText(sb.ToString());
                    DisplayUI.Instance.AppendText(sb.ToString());
                }
                else
                {
                    var msg = "No Anchors Found, Creating Anchor";
                    ttsMgr.SpeakText(msg);
                    Debug.Log(msg);
                }

                // Creates new anchor (based on name) if needed or attach existing
                anchorManager.AttachAnchor(PlacementObject, SavedAnchorFriendlyName);
                CurentState = ControlState.Ready;
                break;
            case ControlState.Ready:
                break;
            case ControlState.PlaceAnchor:                
                break;
        }
    }

    public void PlaceAnchor()
    {
        if (CurentState != ControlState.Ready)
        {
            ttsMgr.SpeakText("AnchorStore Not Ready");
            return;
        }

        anchorManager.RemoveAnchor(PlacementObject);
        CurentState = ControlState.PlaceAnchor;
    }

    public void LockAnchor()
    {
        if (CurentState != ControlState.PlaceAnchor)
        {
            ttsMgr.SpeakText("Not in Anchor Placement State");
            return;
        }
        
        // Add world anchor when object placement is done.
        anchorManager.AttachAnchor(PlacementObject, SavedAnchorFriendlyName);
        CurentState = ControlState.Ready;
        ttsMgr.SpeakText("Anchor Placed");
    }
}
