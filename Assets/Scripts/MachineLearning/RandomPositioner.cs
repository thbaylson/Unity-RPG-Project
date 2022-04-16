using UnityEngine;
using UnityEngine.AI;

/**
 * <summary>
 * Places the GameObject at a random (x, 0, z) coordinate for a given Terrain.
 * </summary>
 */
public class RandomPositioner : MonoBehaviour
{
    [SerializeField] Terrain terrain;
    [SerializeField] float terrainBorder = 0;

    void Start()
    {
        Vector3 result = Vector3.zero;
        float randomX = 0f;
        float randomZ = 0f;

        float xMax = terrain.terrainData.size.x;
        float zMax = terrain.terrainData.size.z;
        bool positionFound = false;
        int cnt = 0;
        while (!positionFound)
        {
            randomX = Random.Range(terrainBorder, xMax - terrainBorder);
            randomZ = Random.Range(terrainBorder, zMax - terrainBorder);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(new Vector3(randomX, 0f, randomZ), out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                positionFound = true;
            }
            if(cnt >= 1000000)
            {
                Debug.Log("Something went wrong");
                return;
            }

            cnt++;
        }

        transform.position = result;
    }

}
