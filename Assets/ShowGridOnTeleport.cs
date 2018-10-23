using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ShowGridOnTeleport : MonoBehaviour {

    public Teleport tele;

    public float lineSize = 0.01f;
    public float gridSize = 5f;
    public int cellCount = 100;

    public bool useChaperoneSize = false;

    public GameObject quadIndicator;

    private Player p;
    private GameObject lines;

    private void Start()
    {
        p = tele.GetPlayer();

        lines = new GameObject("Lines");
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
            poses[0] = new Vector3(0, p.transform.position.y, z * gridSize);
            poses[1] = new Vector3(cellCount * gridSize, p.transform.position.y, z * gridSize);

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
            poses[0] = new Vector3(x * gridSize, p.transform.position.y, 0);
            poses[1] = new Vector3(x * gridSize, p.transform.position.y, cellCount * gridSize);

            lr.SetPositions(poses);
        }
    }

    void Update () {
        lines.SetActive(false);
        quadIndicator.SetActive(false);
        foreach (Hand hand in tele.GetPlayer().hands)
        {
            if (tele.IsTeleportButtonDown(hand))
            {
                lines.SetActive(true);
                quadIndicator.SetActive(true);
            }
        }
	}
}
