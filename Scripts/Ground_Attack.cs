using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground_Attack : MonoBehaviour
{
    public GameObject spikePrefab;
    public float warningTime = 1.5f;
    public float spikeDuration = 2f;
    public float spawnOffsetY = -1f;

    public void SpawnSpike(Vector3 position)
    {
        StartCoroutine(SpikeRoutine(position));
    }

    private IEnumerator SpikeRoutine(Vector3 position)
    {
        GameObject warning = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        warning.transform.position = position;
        warning.transform.localScale = new Vector3(1, 0.1f, 1);
        warning.GetComponent<Renderer>().material.color = Color.red;

        yield return new WaitForSeconds(warningTime);

        Destroy(warning);

        GameObject spike = Instantiate(spikePrefab, position + Vector3.up * spawnOffsetY, Quaternion.identity);


        float riseSpeed = 4f;
        Vector3 targetPosition = position;
        while (Vector3.Distance(spike.transform.position, targetPosition) > 0.01f)
        {
            spike.transform.position = Vector3.MoveTowards(spike.transform.position, targetPosition, riseSpeed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(spikeDuration);

        Destroy(spike);
    }

    private IEnumerator SpawnRippleSpikes(Transform target, int ringCount, int spikesPerRing, float spacing, float bossRadius, float delayBetweenRings)
    {
        Vector3 center = target.position;

        for (int r = 1; r <= ringCount; r++)
        {
            float radius = bossRadius + r * spacing;

            for (int i = 0; i < spikesPerRing; i++)
            {
                //Spike Offset
                float angleOffsetPerRing = (Mathf.PI * 5f / spikesPerRing) / 2f;
                float angle = (i * Mathf.PI * 2f / spikesPerRing) + r * angleOffsetPerRing;

                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;

                float waistHeightOffset = 0f;
                Vector3 spawnPos = new Vector3(center.x + x, center.y - 26f, center.z + z);
                SpawnSpike(spawnPos);
            }

            yield return new WaitForSeconds(delayBetweenRings);
        }
    }


    public void StartRippleSpikes(Transform target, int ringCount, int spikesPerRing, float spacing, float bossRadius, float delayBetweenRings)
    {
        StartCoroutine(SpawnRippleSpikes(target, ringCount, spikesPerRing, spacing, bossRadius, delayBetweenRings));
    }
}