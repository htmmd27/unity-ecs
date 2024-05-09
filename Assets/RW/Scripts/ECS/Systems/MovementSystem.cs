using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

//MovementSystem inherits from ComponentSystem, which is an abstract class needed to implement a system.
public class MovementSystem : ComponentSystem
{
    //This must implement protected override void OnUpdate(), which invokes every frame.
    protected override void OnUpdate()
    {
        //Use Entities with a static ForEach to run logic over every Entity in the World
        Entities.WithAll<MoveForward>().ForEach(
            (ref Translation trans, ref Rotation rot, ref MoveForward moveForward) =>
            {
                //The lambda expression calculates the speed relative to one frame, moveForward.speed * Time.DeltaTime. Then, it multiplies that by the local forward vector, (math.forward(rot.Value).
                trans.Value += moveForward.speed * Time.DeltaTime * math.forward(rot.Value);
            });
        
    }
}
