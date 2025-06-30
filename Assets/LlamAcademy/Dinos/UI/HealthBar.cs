using UnityEngine;
using UnityEngine.UI;

namespace LlamAcademy.Dinos.UI
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Image FillImage;
        [SerializeField] private Gradient Gradient;
        [field: SerializeField] public DeathBehavior OnDeathBehavior { get; private set; }
        [field: SerializeField] public Vector3 FollowOffset { get; private set; }= new (0, 3, 0);

        public enum DeathBehavior
        {
            Disable,
            Destroy
        }

        public void SetProgress(float progress)
        {
            FillImage.fillAmount = Mathf.Clamp01(progress);
            FillImage.color = Gradient.Evaluate(FillImage.fillAmount);
        }
    }
}
