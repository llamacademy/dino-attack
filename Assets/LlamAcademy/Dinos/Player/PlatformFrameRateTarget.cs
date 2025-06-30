using UnityEngine;

namespace LlamAcademy.Dinos.Player
{
    public class PlatformFrameRateTarget : MonoBehaviour
    {
        private void Awake()
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                Application.targetFrameRate = 30;
            }
            else
            {
                Application.targetFrameRate = 200;
            }
        }
    }
}
