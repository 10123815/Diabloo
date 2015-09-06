using UnityEngine;
using System.Collections;
using fuckRPGLib;

public class NPC : ClickableEntity
{

    public string name;

    public GameCode.NPCType npcType = GameCode.NPCType.Normal;

    // a ui panel that will be opned when player click the NPC
    // it may be a quest UI or a simple char panel
    public GameObject NPCOnClickUI;

    // a ui tip that let player know who is this NPC
    public Transform NPCUI;

    public override void OnClick ( )
    {
        // open the quest claim UI
        NPCOnClickUI.SetActive(true);
    }

    public void OnCloseNPCUI ( )
    {
        NPCOnClickUI.SetActive(false);
    }

    protected void Start ( )
    {
        // get the words this NPC would say
        
    }

    void Update ( )
    {
        base.Update();
        SetNPCUI();
    }

    private void SetNPCUI ( )
    {
        Vector3 UIPosition = new Vector3(transform.position.x, transform.position.y + 2.5f, transform.position.z);
        NPCUI.position = Camera.main.WorldToScreenPoint(UIPosition);
    }
}
