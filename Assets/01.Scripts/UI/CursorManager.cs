using UnityEngine;

public class CursorManager : MonoBehaviour
{
    Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f; // カメラとの距離
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        transform.position = Vector3.Lerp(transform.position, worldPos, 0.5f); // 滑らか補間
    }


}
