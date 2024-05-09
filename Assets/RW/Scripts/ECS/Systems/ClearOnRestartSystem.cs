/*
 * Copyright (c) 2020 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
 * distribute, sublicense, create a derivative work, and/or sell copies of the 
 * Software in any work that is designed, intended, or marketed for pedagogical or 
 * instructional purposes related to programming, coding, application development, 
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works, 
 * or sale is expressly withheld.
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using Unity.Entities;

namespace DroneSwarm
{
    /// <summary>
    /// Assigns Lifetime components to any enemies on game over,
    /// removing any leftover drones when the scene restarts.
    /// </summary>
    public class ClearOnRestartSystem : ComponentSystem
    {
        // approximate the screen fader transition time
        float endLifetime = 2f;

        protected override void OnUpdate()
        {
            // check if the game is over
            if (GameManager.IsGameOver())
            {
                // default EntityManager
                EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

                // find all enemies that currently do not have the Lifetime ComponentData
                Entities.WithAll<EnemyTag>().WithNone<Lifetime>().ForEach((Entity enemy) =>
                {
                    // add the Lifetime component to time out Entity automatically
                    entityManager.AddComponentData(enemy, new Lifetime { Value = endLifetime });
                });
            }
        }
    }
}