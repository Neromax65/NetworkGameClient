using Network.NetworkData;
using UnityEngine;

namespace Network
{
    [RequireComponent(typeof(NetworkObject))]
    public class SyncTransform : MonoBehaviour
    {
        /// <summary>
        /// Should the position of the object be synchronized through network
        /// </summary>
        [SerializeField] private bool syncPosition = true;
        /// <summary>
        /// Smooth position synchronization
        /// </summary>
        [SerializeField] private bool syncPosInterpolate;
        /// <summary>
        /// Synchronization position update frequency
        /// </summary>
        [SerializeField, Range(1, 50)] private int syncPosFrequency = 9;
        /// <summary>
        /// Should the rotation of the object be synchronized through network
        /// </summary>
        [SerializeField] private bool syncRotation;
        /// <summary>
        /// Smooth rotation synchronization
        /// </summary>
        [SerializeField] private bool syncRotInterpolate;
        /// <summary>
        /// Synchronization rotation update frequency
        /// </summary>
        [SerializeField, Range(1, 50)] private int syncRotFrequency = 9;
        /// <summary>
        /// Should the scale of the object be synchronized through network
        /// </summary>
        [SerializeField] private bool syncScale;
        /// <summary>
        /// Smooth scale synchronization
        /// </summary>
        [SerializeField] private bool syncSclInterpolate;
        /// <summary>
        /// Synchronization scale update frequency
        /// </summary>
        [SerializeField, Range(1, 50)] private int syncSclFrequency = 9;
    
        private int _fixedUpdateFrameCounter;

        private int _id;

        private Vector3 _oldPos;
        private Quaternion _oldRot;
        private Vector3 _oldScl;

        private Vector3 _interpolatePos;
        private Quaternion _interpolateRot;
        private Vector3 _interpolateScl;
    
    
        private void Awake()
        {
            _oldPos = transform.position;
            _oldRot = transform.rotation;
            _oldScl = transform.localScale;
        }


        void Start()
        {
            _id = GetComponent<NetworkObject>().networkId;
        }

        private void Update()
        {
            if (_interpolatePos != Vector3.zero)
            {
                transform.position = Vector3.Lerp(transform.position, _interpolatePos, Time.deltaTime);
                _oldPos = transform.position;
                if (Vector3.Distance(transform.position, _interpolatePos) <= 0.01f)
                {
                    transform.position = _interpolatePos;
                    _oldPos = transform.position;
                    _interpolatePos = Vector3.zero;
                }
            }

            if (_interpolateRot != Quaternion.identity)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, _interpolateRot, Time.deltaTime);
                _oldRot = transform.rotation;
                if (transform.rotation == _interpolateRot)
                {
                    _interpolateRot = Quaternion.identity;
                }
            }

            if (_interpolateScl != Vector3.zero)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, _interpolateScl, Time.deltaTime);
                _oldScl = transform.localScale;
                if (Vector3.Distance(transform.localScale, _interpolateScl) <= 0.01f)
                {
                    transform.localScale = _interpolateScl;
                    _oldScl = transform.localScale;
                    _interpolateScl = Vector3.zero;
                }
            }
        }

        public void OnPositionDataReceived(Data_Position positionData)
        {
            if (positionData.Id != _id)
                return;
            Vector3 newPos = positionData.Position;
            if (syncPosInterpolate)
            {
                _interpolatePos = newPos;
            }
            else
            {
                transform.position = newPos;
                _oldPos = transform.position;
            }
        }
    
        public void OnRotationDataReceived(Data_Rotation rotationData)
        {
            if (rotationData.Id != _id)
                return;
            var newRot = rotationData.Rotation;
            if (syncRotInterpolate)
            {
                _interpolateRot = newRot;
            }
            else
            {
                transform.rotation = newRot;
                _oldRot = transform.rotation;
            }
        }
    
        public void OnScaleDataReceived(Data_Scale scaleData)
        {
            if (scaleData.Id != _id)
                return;
            Vector3 newScl = scaleData.Scale;
            if (syncSclInterpolate)
            {
                _interpolateScl = newScl;
            }
            else
            {
                transform.localScale = newScl;
                _oldScl = transform.localScale;
            }
        }


        private void FixedUpdate()
        {
            _fixedUpdateFrameCounter++;
            if (syncPosition)
            {
                if (syncPosFrequency % _fixedUpdateFrameCounter == 0)
                {
                    if (_oldPos != transform.position)
                    {
                        NetworkManager.SendDataToServer(new Data_Position()
                        {
                            Id = _id,
                            Position = transform.position
                        });
                        _oldPos = transform.position;
                    }
                }
            }

            if (syncRotation)
            {
                if (syncRotFrequency % _fixedUpdateFrameCounter == 0)
                {
                    if (_oldRot != transform.rotation)
                    {
                        NetworkManager.SendDataToServer(new Data_Rotation()
                        {
                            Id = _id,
                            Rotation = transform.rotation
                        });
                        _oldRot = transform.rotation;
                    }
                }
            }

            if (syncScale)
            {
                if (syncSclFrequency % _fixedUpdateFrameCounter == 0)
                {
                    if (_oldScl != transform.localScale)
                    {
                        NetworkManager.SendDataToServer(new Data_Scale()
                        {
                            Id = _id,
                            Scale = transform.localScale
                        });
                        _oldScl = transform.localScale;
                    }
                }
            }

            if (_fixedUpdateFrameCounter >= 50)
                _fixedUpdateFrameCounter = 0;
        }
    }
}
