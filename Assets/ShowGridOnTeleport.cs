using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class ShowGridOnTeleport : MonoBehaviour {

    [SteamVR_DefaultAction("Teleport", "default")]
    public SteamVR_Action_Boolean teleportAction;
    [SteamVR_DefaultAction("Drag", "default")]
    public SteamVR_Action_Vector2 dragAction;

    public float lineSize = 0.01f;
    public float gridSize = 5f;
    public int cellCount = 100;

    public bool useChaperoneSize = false;

    public GameObject quadIndicator;

    public Player p;

    private GameObject lines;
    private Vector3 toPosition;

    private bool isTeleporting = false;
    private float currentFadeTime = 0.25f;

    private void Start()
    {
        lines = new GameObject("Lines");
        lines.SetActive(false);
        lines.transform.parent = this.transform;

        for (int z = 0; z < cellCount; z++)
        {
            GameObject line = new GameObject("Line");
            line.transform.parent = lines.transform;

            LineRenderer lr = line.AddComponent<LineRenderer>();

            lr.useWorldSpace = true;
            lr.startWidth = lineSize;
            lr.endWidth = lineSize;

            Vector3[] poses = new Vector3[2];
            poses[0] = new Vector3(0 - (((cellCount * gridSize) / 2f)), p.transform.position.y, z * gridSize - (((cellCount * gridSize) / 2f)));
            poses[1] = new Vector3(cellCount * gridSize - (((cellCount * gridSize) / 2f)), p.transform.position.y, z * gridSize - (((cellCount * gridSize) / 2f)));

            lr.SetPositions(poses);
        }

        for (int x = 0; x < cellCount; x++)
        {
            GameObject line = new GameObject("Line");
            line.transform.parent = lines.transform;

            LineRenderer lr = line.AddComponent<LineRenderer>();

            lr.useWorldSpace = true;
            lr.startWidth = lineSize;
            lr.endWidth = lineSize;

            Vector3[] poses = new Vector3[2];
            poses[0] = new Vector3(x * gridSize -(((cellCount * gridSize) / 2f)), p.transform.position.y, -(((cellCount * gridSize) / 2f)));
            poses[1] = new Vector3(x * gridSize - (((cellCount * gridSize) / 2f)), p.transform.position.y, cellCount * gridSize - (((cellCount * gridSize) / 2f)));

            lr.SetPositions(poses);
        }
    }

    void Update () {
        foreach (Hand hand in p.hands)
        {
            if (teleportAction.GetStateDown(hand.handType))
            {
                isTeleporting = !isTeleporting;
                Debug.Log("teleporting: " + isTeleporting);

                lines.SetActive(isTeleporting);
                quadIndicator.SetActive(isTeleporting);

                if(!isTeleporting)
                {
                    SteamVR_Fade.Start(Color.clear, 0);
                    SteamVR_Fade.Start(Color.black, currentFadeTime);

                    Invoke("TeleportPlayer", currentFadeTime);
                    Debug.Log("Teleported!!!");
                }

                break;
            }

            if (isTeleporting)
            {
                Vector2 dragDir = dragAction.GetAxis(hand.handType);
                toPosition = p.trackingOriginTransform.position + (Quaternion.AngleAxis(p.hmdTransform.transform.eulerAngles.y, Vector3.up) * new Vector3(dragDir.x * 3 * gridSize, 0f, dragDir.y * 3 * gridSize));

                quadIndicator.transform.position = Snap(toPosition, (int)gridSize, quadIndicator.transform.position.y);
            }
        }
	}

    private void TeleportPlayer()
    {
        SteamVR_Fade.Start(Color.clear, currentFadeTime);

        p.trackingOriginTransform.position = quadIndicator.transform.position;
    }

    private Vector3 Snap(Vector3 pos, int v, float oldY)
    {
        float x = pos.x;
        float y = oldY;
        float z = pos.z;
        x = Mathf.FloorToInt(x / v) * v;
        //y = Mathf.FloorToInt(y / v) * v;
        z = Mathf.FloorToInt(z / v) * v;
        return new Vector3(x + (v / 2f), y, z + (v / 2f));
    }
}
