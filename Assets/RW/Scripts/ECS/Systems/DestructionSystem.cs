using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class DestructionSystem : ComponentSystem
{
    private float thresholdDistance = 2f;
    protected override void OnUpdate()
    {
        if (GameManager.IsGameOver())
        {
            return;
        }

        float3 playerPosition = GameManager.GetPlayerPosition();
        Entities.WithAll<EnemyTag>().ForEach((Entity enemy, ref Translation enemyPos) =>
        {
            playerPosition.y = enemyPos.Value.y;

            if (math.distance(enemyPos.Value, playerPosition) <= thresholdDistance)
            {
                FXManager.Instance.CreateExplosion(enemyPos.Value);
                FXManager.Instance.CreateExplosion(playerPosition);
                GameManager.EndGame();
                
                PostUpdateCommands.DestroyEntity(enemy);
            }

            float3 enemyPosition = enemyPos.Value;
            
            Entities.WithAll<BulletTag>().ForEach((Entity bullet, ref Translation bulletPos) =>
            {
                if (math.distance(enemyPosition, bulletPos.Value) <= thresholdDistance)
                {
                    PostUpdateCommands.DestroyEntity(enemy);
                    PostUpdateCommands.DestroyEntity(bullet);
                    
                    FXManager.Instance.CreateExplosion(enemyPosition);
                    GameManager.AddScore(1);
                }
            });
        });
    }
}
