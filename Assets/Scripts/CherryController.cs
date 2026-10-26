using System.Collections;
using UnityEngine;

public class CherryController : MonoBehaviour
{

    [SerializeField] private GameObject cherryPrefab;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float spawnDelay = 5f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float spawnRadius = 21f;

    private readonly Vector3 levelCentre = new Vector3(13.5f, -14f, 0f);
    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnDelay);
            yield return StartCoroutine(SpawnAndMove());
        }
    }

    IEnumerator SpawnAndMove()
    {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);

        Vector3 start = levelCentre + direction * spawnRadius;
        Vector3 end = levelCentre - direction * GetOffScreenRadius();

        GameObject cherry = Instantiate(cherryPrefab, start, Quaternion.identity, transform);

        float duration = Vector3.Distance(start, end) / moveSpeed;
        float startTime = Time.time;

        while (cherry != null)
        {
            float t = (Time.time - startTime) / duration;

            if (t >= 1f)
            {
                Destroy(cherry);
                yield break;
            }

            cherry.transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }
    }

    float GetOffScreenRadius()
    {
        float halfHeight = mainCamera.orthographicSize;
        float halfWidth = halfHeight * mainCamera.aspect;
        return Mathf.Sqrt(halfWidth * halfWidth + halfHeight * halfHeight) + 1f ;
    }
}
