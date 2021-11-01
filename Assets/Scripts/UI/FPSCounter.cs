using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class FPSCounter : MonoBehaviour
    {
        [SerializeField] Text text;
        [SerializeField] float hudRefreshRate = .5f;
 
        float _timer;
 
        void Update()
        {
            if (Time.unscaledTime > _timer)
            {
                var fps = (int)(1f / Time.unscaledDeltaTime);
                text.text = "FPS: " + fps;
                _timer = Time.unscaledTime + hudRefreshRate;
            }
        }
    }
}