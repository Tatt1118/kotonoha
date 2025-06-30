using System.Collections.Generic;
using UnityEngine;

public class MoveText : MonoBehaviour
{
    private RectTransform rectTransform;
    private RectTransform parentRect;
    private float noiseSeedX;
    private float noiseSeedY;
    private float timer;
    private Vector2 velocity;
    private Vector2 noiseOffset;
    public float speed = 30f;
    public float noiseScale = 0.5f;
    public float repelDistance = 30f;
    public float repelStrength = 80f;
    public float directionChangeInterval = 3f;
    private static List<MoveText> allKeywords = new List<MoveText>();

    void OnEnable() => allKeywords.Add(this);
    void OnDisable() => allKeywords.Remove(this);

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        parentRect = transform.parent as RectTransform;

        velocity = Random.insideUnitCircle.normalized * speed;
        noiseSeedX = Random.Range(0f, 100f);
        noiseSeedY = Random.Range(0f, 100f);
        timer = Random.Range(0f, directionChangeInterval); // ランダムな初期変化時間
    }

    void Update()
    {
        // ふわふわノイズの変化
        noiseSeedX += Time.deltaTime * noiseScale;
        noiseSeedY += Time.deltaTime * noiseScale;
        noiseOffset = new Vector2(
            Mathf.PerlinNoise(noiseSeedX, 0f) - 0.5f,
            Mathf.PerlinNoise(0f, noiseSeedY) - 0.5f
        );

        // 移動先の候補位置
        Vector2 move = (velocity + noiseOffset * speed) * Time.deltaTime;
        Vector2 newPos = rectTransform.anchoredPosition + move;

        // 親UIの範囲でバウンド
        Vector2 halfParentSize = parentRect.rect.size / 2f;
        Vector2 halfSelfSize = rectTransform.rect.size / 2f;

        float xMin = -halfParentSize.x + halfSelfSize.x;
        float xMax = halfParentSize.x - halfSelfSize.x;
        float yMin = -halfParentSize.y + halfSelfSize.y;
        float yMax = halfParentSize.y - halfSelfSize.y;

        if (newPos.x < xMin || newPos.x > xMax)
        {
            velocity.x *= -1;
            newPos.x = Mathf.Clamp(newPos.x, xMin, xMax);
        }
        if (newPos.y < yMin || newPos.y > yMax)
        {
            velocity.y *= -1;
            newPos.y = Mathf.Clamp(newPos.y, yMin, yMax);
        }
        // 更新
        rectTransform.anchoredPosition = newPos;

        // 定期的にゆるく方向変更
        timer += Time.deltaTime;
        if (timer >= directionChangeInterval)
        {
            velocity = Random.insideUnitCircle.normalized * speed;
            timer = 0f;
        }
    }

    internal void UpdateBasePosition()
    {
        throw new System.NotImplementedException();
    }
}
