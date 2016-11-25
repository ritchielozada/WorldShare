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

    public ControlState CurrentState;

    // Use this for initialization
    void Start()
    {
        CurrentState = ControlState.WaitingForAnchorStore;

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
        CurrentState = ControlState.CheckAnchorStatus;
        DisplayUI.Instance.AppendText("Anchor Store Ready");
    }
    
    void Update()
    {
        switch (CurrentState)
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
                    //Debug.Log(sb.ToString());
                    //ttsMgr.SpeakText(sb.ToString());
                    DisplayUI.Instance.AppendText(sb.ToString());
                }
                else
                {
                    var msg = "No Anchors Found, Creating Anchor";
                    DisplayUI.Instance.AppendText(msg);
                    //Debug.Log(msg);
                }

                // Creates new anchor (based on name) if needed or attach existing
                anchorManager.AttachAnchor(PlacementObject, SavedAnchorFriendlyName);
                CurrentState = ControlState.Ready;
                DisplayUI.Instance.AppendText("Anchor System READY");
                break;
            case ControlState.Ready:                
                break;
            case ControlState.PlaceAnchor:                
                break;
        }
    }

    public void PlaceAnchor()
    {
        if (CurrentState != ControlState.Ready)
        {
            DisplayUI.Instance.AppendText("AnchorStore Not Ready");
            return;
        }

        anchorManager.RemoveAnchor(PlacementObject);
        CurrentState = ControlState.PlaceAnchor;
        DisplayUI.Instance.AppendText("Anchor Placement Started");
    }

    public void LockAnchor()
    {
        if (CurrentState != ControlState.PlaceAnchor)
        {
            DisplayUI.Instance.AppendText("Not in Anchor Placement State");
            return;
        }
        
        // Add world anchor when object placement is done.
        anchorManager.AttachAnchor(PlacementObject, SavedAnchorFriendlyName);
        CurrentState = ControlState.Ready;
        DisplayUI.Instance.AppendText("Anchor Placed");
    }
}
