using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class CameraShaker : MonoBehaviour
{
    public IEnumerator Shake(float duration, float magnitude, float frequency)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0f;

        // ランダムなオフセットを与えてパターンをずらす
        float randomSeedX = Random.Range(0f, 100f);
        float randomSeedY = Random.Range(0f, 100f);

        while (elapsed < duration)
        {
            float x = (Mathf.PerlinNoise(randomSeedX, Time.time * frequency) - 0.5f) * 2f * magnitude;
            float y = (Mathf.PerlinNoise(randomSeedY, Time.time * frequency) - 0.5f) * 2f * magnitude;

            transform.localPosition = originalPos + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}

