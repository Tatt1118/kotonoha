using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TapEffect : MonoBehaviour
{
    [SerializeField] ParticleSystem tapEffect;           
    [SerializeField] Camera _camera;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // マウスのワールド座標までパーティクルを移動し、パーティクルエフェクトを1つ生成する
            var pos = _camera.ScreenToWorldPoint(Input.mousePosition + _camera.transform.forward * 10);
            tapEffect.transform.position = pos;
            tapEffect.Emit(1);
        }
    }
}
