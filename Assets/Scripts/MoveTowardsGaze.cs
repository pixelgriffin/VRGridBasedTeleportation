using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class MoveTowardsGaze : MonoBehaviour
{
    [SteamVR_DefaultAction("Teleport", "default")]
    public SteamVR_Action_Boolean teleportAction;
    [SteamVR_DefaultAction("Drag", "default")]
    public SteamVR_Action_Vector2 dragAction;

    public Transform hmd;
    public Player p;

    public float speed = 2f;

    public Transform gazePointer;

    private Vector3 offset = Vector3.zero;
    private bool isTeleporting = false;
    private float currentFadeTime = 0.25f;

    private MeshRenderer rend;

    private void Start()
    {
        rend = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        rend.enabled = isTeleporting;
        gazePointer.gameObject.SetActive(isTeleporting);

        RaycastHit hit;
        if(Physics.Raycast(hmd.transform.position, hmd.transform.forward, out hit))
        {
            Vector3 target = hit.point;
            target.y = this.transform.position.y;
            target.x += offset.x;
            target.z += offset.z;

            this.transform.position = Vector3.MoveTowards(this.transform.position, target, Time.deltaTime * speed);

            gazePointer.transform.position = hit.point;
        }

        foreach (Hand hand in p.hands)
        {
            if (teleportAction.GetStateDown(hand.handType))
            {
                isTeleporting = !isTeleporting;

                if (!isTeleporting)
                {
                    SteamVR_Fade.Start(Color.clear, 0);
                    SteamVR_Fade.Start(Color.black, currentFadeTime);

                    Invoke("TeleportPlayer", currentFadeTime);
                }

                break;
            }

            if (isTeleporting)
            {
                Vector2 dragDir = dragAction.GetAxis(hand.handType);
                Vector3 drag = new Vector3(dragDir.x, 0f, dragDir.y);
                drag = (Quaternion.AngleAxis(p.hmdTransform.transform.eulerAngles.y, Vector3.up) * drag);

                offset.x += drag.x * Time.deltaTime;
                offset.z += drag.z * Time.deltaTime;
            }
        }
    }

    private void TeleportPlayer()
    {
        SteamVR_Fade.Start(Color.clear, currentFadeTime);

        Vector3 playerFeetOffset = p.trackingOriginTransform.position - p.feetPositionGuess;

        Vector3 to = this.transform.position;
        to.y = p.trackingOriginTransform.position.y;

        p.trackingOriginTransform.position = to + playerFeetOffset;
        offset = Vector3.zero;
    }
}
