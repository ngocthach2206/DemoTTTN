using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public List<GameObject> terrainChunks;
    public GameObject player;
    public float checkerRadius;
    public LayerMask terrainMask;
    public GameObject currentChunk;
    Vector3 playerLastPosition;

    [Header("Optimization")]
    public List<GameObject> spawnedChunks;
    public GameObject lastestChunk;
    public float maxOpDist; //must be greater than the lenght and width of the tilemap
    float opDist;
    float opCooldown;
    public float opCooldownDur;

    // Start is called before the first frame update
    void Start()
    {
        playerLastPosition = player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        ChunkChecker();
        ChunkOptimizer();
    }

    void ChunkChecker()
    {
        if(!currentChunk)
        {
            return;
        }

        Vector3 moveDir = player.transform.position - playerLastPosition;
        playerLastPosition = player.transform.position;

        string directionName = GetDirectionName(moveDir);
        CheckAndSpawnChunk(directionName);
        //check additional adjacent direction for diagonal chunks
        if(directionName.Contains("Up"))
        {
            CheckAndSpawnChunk("Up");
        }
        if (directionName.Contains("Down"))
        {
            CheckAndSpawnChunk("Down");
        }
        if (directionName.Contains("Right"))
        {
            CheckAndSpawnChunk("Right");
        }
        if (directionName.Contains("Left"))
        {
            CheckAndSpawnChunk("Left");
        }
    }

    void CheckAndSpawnChunk(string direction)
    {
        if(!Physics2D.OverlapCircle(currentChunk.transform.Find(direction).position, checkerRadius, terrainMask))
        {
            SpawnChunk(currentChunk.transform.Find(direction).position);
        }
    }

    string GetDirectionName(Vector3 direction)
    {
        direction = direction.normalized;
        
        if(Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            //moving horizontal more than vertical
            if(direction.y > 0.5f)
            {
                //also moving upwards
                return direction.x > 0 ? "Right Up" : "Left Up";
            }
            else if(direction.y < -0.5f)
            {
                //also moving downwards
                return direction.x > 0 ? "Right Down" : "Left Down";
            }
            else
            {
                //moving straight horizontal
                return direction.x > 0 ? "Right" : "Left";
            }
        }
        else
        {
            //moving vertical more than horizontal
            if (direction.x > 0.5f)
            {
                //also moving right
                return direction.y > 0 ? "Right Up" : "Right Down";
            }
            else if (direction.x < -0.5f)
            {
                //also moving right
                return direction.y > 0 ? "Left Up" : "Left Down";
            }
            else
            {
                //moving straight vertical
                return direction.y > 0 ? "Up" : "Down";
            }
        }
    }

    void SpawnChunk(Vector3 spawnPosition)
    {
        int rand = Random.Range(0, terrainChunks.Count);
        lastestChunk = Instantiate(terrainChunks[rand], spawnPosition, Quaternion.identity);
        spawnedChunks.Add(lastestChunk);
    }

    void ChunkOptimizer()
    {
        opCooldown -= Time.deltaTime;

        if(opCooldown <= 0f)
        {
            opCooldown = opCooldownDur;
        }
        else
        {
            return;
        }

        foreach(GameObject chunk in spawnedChunks)
        {
            opDist = Vector3.Distance(player.transform.position, chunk.transform.position);
            if(opDist > maxOpDist)
            {
                chunk.SetActive(false);
            }
            else
            {
                chunk.SetActive(true);
            }
        }
    }
}
