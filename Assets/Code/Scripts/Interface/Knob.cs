using UnityEngine;

namespace AWITLOTF.Assets.Code.Scripts.Interface
{
    public class Knob: MonoBehaviour
    {
        public void TurnToPercent(float percent)
        {
            percent = Mathf.Clamp01(percent);
            float angle = Mathf.Lerp(135f, -135f, percent);
            transform.localEulerAngles = new Vector3(0f, 180f, angle);
        }    
    }
}