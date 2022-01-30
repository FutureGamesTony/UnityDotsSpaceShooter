using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject playerPrefab;
    public GameObject bulletPrefab;
    public GameObject enemyPrefab;

    public Entity playerEntityPrefab;
    public Entity bulletEntityPrefab;
    public Entity enemyEntityPrefab;
    public EntityManager manager;

    public float playerSpeed;
    public float enemySpeed;
    public float bulletSpeed;
    public float bulletCooldown;

    public float3 playerSpawn;
    public float3 enemySpawnOrigin;
    public float3 playerPosition;
    private float2 enemyPosition;

    public BlobAssetStore blobAssetStore;
    public Camera mainCamera;

    private int enemiesDisposed = 0;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        //mainCamera = Camera.main;
        manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        blobAssetStore = new BlobAssetStore();
        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore);
        
        playerEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(playerPrefab, settings);
        bulletEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(bulletPrefab, settings);
        enemyEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(enemyPrefab, settings);
    }
    private void Start()
    {
        SpawnPlayer();
    }
    private void OnDestroy()
    {
        blobAssetStore.Dispose();
    }

    public float3 SetPlayerPosition(float3 playerPos)
    {
        

        playerPosition = playerPos;
        return playerPosition;
    }
    public float2 SetEnemyPosition(float2 enemyPos)
    {
        enemyPosition = enemyPos;
        return enemyPosition;
    }

    public float3 GetPlayerPosition()
    {
        return playerPosition;
    }
    public float2 GetEnemyPosition()
    {
        return enemyPosition;
    }
    public float3 GetEnemySpawnOrigin() { return enemySpawnOrigin; }

    public void SpawnPlayer()
    {

        Translation playerTrans = new Translation
        {
            Value = playerSpawn
        };
        Entity newPlayerEntity = manager.Instantiate(playerEntityPrefab);

        manager.AddComponentData(newPlayerEntity, playerTrans);   
    }

    public float3 GetCameraPosition()
    {
        return mainCamera.transform.position;
    }
    public float2 GetCameraExtent()
    {
        Vector3 pos = mainCamera.transform.position;
        float width = mainCamera.orthographicSize * mainCamera.aspect;
        
        return new float2(width, mainCamera.orthographicSize);
    }
    bool canFire = true;
    public bool CanFire()
    {
        return canFire;
    }
   public void Fire()
    {
        canFire = false;
        StartCoroutine(Cooldown());
    }
    public void EnemyKilled()
    {
        enemiesDisposed++;  
    }
    public bool GetDisposedEnemies(int spawnedEnemies)
    {
        if (enemiesDisposed >= spawnedEnemies) 
        { 
            enemiesDisposed = 0;
            return true;
        }
        return false;
    }
    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(bulletCooldown);
        canFire = true;
    }
}
