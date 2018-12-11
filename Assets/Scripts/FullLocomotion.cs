using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class FullLocomotion : MonoBehaviour
{
    [SteamVR_DefaultAction("Drag", "default")]
    public SteamVR_Action_Vector2 dragAction;

    public Player player;

    public bool allowHorizontalMotion = false;

    void Update()
    {
        Vector3 motion = Vector3.zero;

        foreach (Hand hand in player.hands)
        {
            Vector2 input = dragAction.GetAxis(hand.handType);
            if (input != Vector2.zero)
            {
                motion += FlattenVectorZ(player.hmdTransform.forward) * input.y * Time.deltaTime;

                if(allowHorizontalMotion)
                {
                    motion += FlattenVectorZ(player.hmdTransform.right) * input.x * Time.deltaTime;
                }
            }
        }

        if (motion.magnitude > 1)
            motion.Normalize();

        player.transform.position += motion;
    }

    private Vector3 FlattenVectorZ(Vector3 vec)
    {
        vec.y = 0;
        return vec;
    }
}
