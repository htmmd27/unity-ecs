using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class FacePlayerSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        if (GameManager.IsGameOver())
        {
            return;
        }

        float3 playerPos = (float3)GameManager.GetPlayerPosition();
        
        Entities.WithAll<EnemyTag>().ForEach((Entity entity, ref Translation trans, ref Rotation rot) =>
        {
            float3 direction = playerPos - trans.Value;
            direction.y = 0f;
            
            rot.Value = quaternion.LookRotation(direction, math.up());
        });
        
    }
}
