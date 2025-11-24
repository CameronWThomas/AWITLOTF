using UnityEngine;

namespace AWITLOTF.Assets.Code.Scripts
{
    public class TeleporterSphere : MonoBehaviour
    {

        public Vector3 deactivatedScale = new Vector3(0f, 0f, 0f);
        public Vector3 activatedScale = new Vector3(50f, 50f, 50f);
        public float scaleSpeed = 1f;
        public Coroutine scalingCoroutine;

        public void Start()
        {
            transform.localScale = deactivatedScale;
        }
        public void ActivateTeleporter()
        {
            transform.localScale = deactivatedScale;
            if (scalingCoroutine != null)
            {
                StopCoroutine(scalingCoroutine);
            }
            scalingCoroutine = StartCoroutine(ScaleOverTime(activatedScale));
        }
        public void DeactivateTeleporter()
        {
            transform.localScale = activatedScale;
            if (scalingCoroutine != null)
            {
                StopCoroutine(scalingCoroutine);
            }
            scalingCoroutine = StartCoroutine(ScaleOverTime(deactivatedScale));
        }
        private System.Collections.IEnumerator ScaleOverTime(Vector3 targetScale)
        {
            while (Vector3.Distance(transform.localScale, targetScale) > 0.01f)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
                yield return null;
            }
            transform.localScale = targetScale;
        }


        
    }
}