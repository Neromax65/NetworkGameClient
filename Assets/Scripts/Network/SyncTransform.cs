using Network.NetworkData;
using UnityEngine;

namespace Network
{
    [RequireComponent(typeof(NetworkObject))]
    public class SyncTransform : MonoBehaviour
    {

        [SerializeField] private bool syncPosition = true;
        [SerializeField] private bool syncPosInterpolate;
        [SerializeField, Range(1, 50)] private int syncPosFrequency = 9;
        [SerializeField] private bool syncRotation;
        [SerializeField] private bool syncRotInterpolate;
        [SerializeField, Range(1, 50)] private int syncRotFrequency = 9;
        [SerializeField] private bool syncScale;
        [SerializeField] private bool syncSclInterpolate;
        [SerializeField, Range(1, 50)] private int syncSclFrequency = 9;
    
        private int _fixedUpdateFrameCounter;

        private int _id;
        // private Guid _guid;

        private Vector3 _oldPos;
        private Quaternion _oldRot;
        private Vector3 _oldScl;

        private Vector3 _interpolatePos;
        private Quaternion _interpolateRot;
        private Vector3 _interpolateScl;
    
    
        private void Awake()
        {
            // _guid = Guid.NewGuid();
            // Debug.LogErrorFormat($"Guid: {_id}");
            _oldPos = transform.position;
            _oldRot = transform.rotation;
            _oldScl = transform.localScale;
        }


        void Start()
        {
            _id = GetComponent<NetworkObject>().networkId;
            NetworkManager.DataReceived += DataReceived;
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

        void DataReceived(INetworkData data)
        {
            switch (data.Command)
            {
                case Command.Position:
                    if (syncPosition)
                        OnPositionDataReceived(data as Data_Position);
                    break;
                case Command.Rotation:
                    if (syncRotation)
                        OnRotationDataReceived(data as Data_Rotation);
                    break;
                case Command.Scale:
                    if (syncScale)
                        OnScaleDataReceived(data as Data_Scale);
                    break;
                default:
                    return;
            }
        }

        void OnPositionDataReceived(Data_Position positionData)
        {
            if (positionData.Id != _id)
                return;
            // Debug.LogError($"Move data received: Id:{positionData.Id}, X:{positionData.X}, Y:{positionData.Y}, Z: {positionData.Z}");
            Vector3 newPos = new Vector3(positionData.X, positionData.Y, positionData.Z);
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
    
        void OnRotationDataReceived(Data_Rotation rotationData)
        {
            if (rotationData.Id != _id)
                return;
            // Debug.LogError($"Move data received: Id:{positionData.Id}, X:{positionData.X}, Y:{positionData.Y}, Z: {positionData.Z}");
            var newRot = new Quaternion(rotationData.X, rotationData.Y, rotationData.Z, rotationData.W);
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
    
        void OnScaleDataReceived(Data_Scale scaleData)
        {
            if (scaleData.Id != _id)
                return;
            // Debug.LogError($"Move data received: Id:{positionData.Id}, X:{positionData.X}, Y:{positionData.Y}, Z: {positionData.Z}");
            Vector3 newScl = new Vector3(scaleData.X, scaleData.Y, scaleData.Z);
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
                            X = transform.position.x,
                            Y = transform.position.y,
                            Z = transform.position.z
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
                            X = transform.rotation.x,
                            Y = transform.rotation.y,
                            Z = transform.rotation.z,
                            W = transform.rotation.w,
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
                            X = transform.localScale.x,
                            Y = transform.localScale.y,
                            Z = transform.localScale.z
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
