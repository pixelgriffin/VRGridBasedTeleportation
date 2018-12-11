using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class MoveTowardsTeleport : MonoBehaviour
{
    public Teleport tele;

    public float speed = 2f;

    void Update()
    {
        Vector3 target = tele.GetTeleportPointerPosition();
        target.y = this.transform.position.y;

        this.transform.position = Vector3.MoveTowards(this.transform.position, target, Time.deltaTime * speed);
    }
}
