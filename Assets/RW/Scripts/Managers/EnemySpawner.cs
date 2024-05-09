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

using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Collections;
using Random = UnityEngine.Random;


/// <summary>
/// spawns a swarm of enemy entities offscreen, encircling the player
/// </summary>
public class EnemySpawner : MonoBehaviour
{

    [Header("Spawner")]
    // number of enemies generated per interval
    [SerializeField] private int spawnCount = 30;

    // time between spawns
    [SerializeField] private float spawnInterval = 3f;

    // enemies spawned on a circle of this radius
    [SerializeField] private float spawnRadius = 30f;

    // extra enemy increase each wave
    [SerializeField] private int difficultyBonus = 5;

    [Header("Enemy")]
    // random speed range
    [SerializeField] float minSpeed = 4f;
    [SerializeField] float maxSpeed = 12f;

    // counter
    private float spawnTimer;

    // flag from GameManager to enable spawning
    private bool canSpawn;

    private EntityManager entityManager;

    [SerializeField] private Mesh enemyMesh;
    [SerializeField] private Material enemyMaterial;

    [SerializeField] private GameObject enemyPrefab;
    private Entity enemyEntityPrefab;
    private void Start()
    {
        //All Entities, Components, and Systems exist in a World. Each World has one EntityManager.
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
        enemyEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(enemyPrefab, settings);

        SpawnWave();
    }

    // spawns enemies in a ring around the player
    private void SpawnWave()
    {
        NativeArray<Entity> enemyArray = new NativeArray<Entity>(spawnCount, Allocator.Temp);

        for (int i = 0; i < enemyArray.Length; i++)
        {
            enemyArray[i] = entityManager.Instantiate(enemyEntityPrefab);
            
            entityManager.SetComponentData(enemyArray[i], new Translation{Value = RandomPointOnCircle(spawnRadius)});
            entityManager.SetComponentData(enemyArray[i], new MoveForward{speed = Random.Range(minSpeed, maxSpeed)});
        }
        //Once the loop has finished, NativeArray.Dispose frees any temporarily allocated memory.
        enemyArray.Dispose();
        spawnCount += difficultyBonus;
    }

    // get a random point on a circle with given radius
    private float3 RandomPointOnCircle(float radius)
    {
        Vector2 randomPoint = Random.insideUnitCircle.normalized * radius;

        // return random point on circle, centered around the player position
        return new float3(randomPoint.x, 0.5f, randomPoint.y) + (float3)GameManager.GetPlayerPosition();
    }

    // signal from GameManager to begin spawning
    public void StartSpawn()
    {
        canSpawn = true;
    }

    private void Update()
    {
        // disable if the game has just started or if player is dead
        if (!canSpawn || GameManager.IsGameOver())
        {
            return;
        }

        // count up until next spawn
        spawnTimer += Time.deltaTime;

        // spawn and reset timer
        if (spawnTimer > spawnInterval)
        {
            SpawnWave();
            spawnTimer = 0;
        }
    }
}
