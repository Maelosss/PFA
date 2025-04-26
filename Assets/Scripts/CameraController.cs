using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace B1
{
    public class CameraController : MonoBehaviour
    {
        public enum MODE
        {
            Follow,
            Step
        }


        [Header("MAIN")]
        [SerializeField] private MODE _mode = MODE.Follow;
        [SerializeField] private float _smoothTime = 0.1f;

        [Header("FOLLOW")]
        [SerializeField] private GameObject _target;
        [SerializeField] private Vector2 _limiteFollow = Vector2.one;
        [SerializeField] private Vector2 _offset;

        [Header("STEP")]
        public List<Transform> points = new();
        [SerializeField] private Vector2 _limiteStep = Vector2.one;
        private int _currentIDStep;

        private Vector2 _velocity;

        void Start()
        {
            switch (_mode)
            {
                case MODE.Step:
                    var smoothPos = Vector2.SmoothDamp(transform.position, points[0].transform.position, ref _velocity, _smoothTime);
                    Vector3 targetPos = new(smoothPos.x, smoothPos.y, transform.position.z);
                    transform.position = targetPos;
                    break;
            }
        }

        void Update()
        {
            switch (_mode)
            {
                case MODE.Follow:
                    var outPosLeft = transform.TransformPoint(-_limiteFollow) - (Vector3)_offset;
                    var outPosRight = transform.TransformPoint(_limiteFollow) + (Vector3)_offset;
                    var outLeft = _target.transform.position.x < outPosLeft.x;
                    var outRight = _target.transform.position.x > outPosRight.x;

                    var targetOut = outLeft ? outPosLeft : outRight ? outPosRight : transform.position;

                    var smoothPos = Vector2.SmoothDamp(transform.position, targetOut, ref _velocity, _smoothTime);
                    Vector3 targetPos = new(smoothPos.x, transform.position.y, transform.position.z);
                    transform.position = targetPos;
                    break;

                case MODE.Step:
                    if (Vector2.Distance(transform.position, points[_currentIDStep].transform.position) < .1f)
                    {
                        if (_target.transform.position.x < transform.TransformPoint(-_limiteStep).x)
                        {
                            _currentIDStep--;
                        }
                        else if (_target.transform.position.x > transform.TransformPoint(_limiteStep).x)
                        {
                            _currentIDStep++;
                        }
                    }

                    _currentIDStep = Mathf.Clamp(_currentIDStep, 0, points.Count^1);

                    smoothPos = Vector2.SmoothDamp(transform.position, points[_currentIDStep].transform.position, ref _velocity, _smoothTime);
                    targetPos = new(smoothPos.x, smoothPos.y, transform.position.z);
                    transform.position = targetPos;
                    break;
            }

        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, _limiteFollow*2);

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, _limiteStep * 2);
        }
    }
}
