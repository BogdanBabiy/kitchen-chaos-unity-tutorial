using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    private float footstepTimer;
    private float footstepTimerMax = 0.1f;
    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        footstepTimer -= Time.deltaTime;
        if (footstepTimer < 0f)
        {
            footstepTimer = footstepTimerMax;
            if (player.IsWalking())
            {
                SoundManager.Instance.PlayFootstepsSound(player.transform.position);
            }
        }
    }
}
