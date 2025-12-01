using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AWITLOTF.Assets.Code.Scripts.Interface
{
    public class Knob: MonoBehaviour
    {
        public Camera MachineCamera;

        public bool IsSelected { get; private set; }

        public int InputChange { get; private set; } = 0;

        public void TurnToPercent(float percent)
        {
            percent = Mathf.Clamp01(percent);
            float angle = Mathf.Lerp(135f, -135f, percent);
            transform.localEulerAngles = new Vector3(0f, 180f, angle);
        }

        private void Update()
        {
            if (!IsSelected && Input.GetKeyDown(KeyCode.Mouse0) && IsKnobBeingSelected())
            {
                Cursor.visible = false;
                IsSelected = true;
                InputChange = 0;
            }

            if (IsSelected && Input.GetKeyUp(KeyCode.Mouse0))
            {
                Cursor.visible = true;
                IsSelected = false;
                InputChange = 0;
            }

            if (IsSelected)
                UpdateInputChange();
        }

        private bool IsKnobBeingSelected()
        {
            if (MachineCamera == null)
                return false;

            var ray = MachineCamera.ScreenPointToRay(Input.mousePosition);

            // Check if any raycast hits are hitting this knob
            //return Physics.RaycastAll(cameraTransform.position, cameraTransform.forward, 10f)
            //    .Where(x => x.transform.TryGetComponent<Knob>(out _))
            //    .Any(x => x.transform.GetComponent<Knob>() == this);

            var hitInfos = Physics.RaycastAll(ray, 10f);
            //var hitInfos = Physics.RaycastAll(cameraTransform.position, cameraTransform.forward, 10f);
            var knobHitInfos = hitInfos.Where(x => x.transform.TryGetComponent<Knob>(out _));
            var thisKnobHit = knobHitInfos.Any(x => x.transform.GetComponent<Knob>() == this);
            return thisKnobHit;
        }

        private List<(float, Vector2)> _lastMouseDeltas = new();
        private float _lastValidMouseDeltaTime = float.PositiveInfinity;
        private void UpdateInputChange()
        {
            var x = Input.GetAxis("Mouse X");
            var y = Input.GetAxis("Mouse Y");
            var mouseDelta = new Vector2(x, y);

            var maxDeltas = 15;
            var minDeltas = 5;

            if (mouseDelta != Vector2.zero)
            {
                _lastValidMouseDeltaTime = Time.time;
                _lastMouseDeltas.Add((Time.time, mouseDelta));
            }

            if (Time.time - _lastValidMouseDeltaTime > .1f)
                _lastMouseDeltas.Clear();

            if (_lastMouseDeltas.Count < minDeltas)
            {
                InputChange = 0;
                return;
            }

            if (_lastMouseDeltas.Count > maxDeltas)
                _lastMouseDeltas.RemoveAt(0);

            var lastDelta = Vector2.zero;
            var lastDeltaTime = float.PositiveInfinity;

            var totalArcLength = 0f;
            foreach (var (time, delta) in _lastMouseDeltas)
            {
                if (lastDelta != Vector2.zero)
                {
                    var angle = Vector2.SignedAngle(delta, lastDelta);
                    var averageMagnitude = (delta.magnitude + lastDelta.magnitude) / 2f;
                    var arcLength = (averageMagnitude * angle * MathF.PI) / 180f;
                    totalArcLength += arcLength;
                }

                lastDeltaTime = time;
                lastDelta = delta;
            }

            var averageArcLength = totalArcLength / (_lastMouseDeltas.Count - 1);
            var quantityPerLength = 40f;

            var quantityChange = averageArcLength * quantityPerLength;
            //Debug.Log($"quantityChange: {quantityChange}");

            InputChange = (int)Mathf.Clamp(-1 * quantityChange, -1f, 1f);
        }
    }
}