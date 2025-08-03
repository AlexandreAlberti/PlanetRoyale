using System;
using Game.Enabler;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.AI
{
    public class AIInputManager : EnablerMonoBehaviour
    {
        [SerializeField] private float _moveTimeMin;
        [SerializeField] private float _moveTimeMax;
        [SerializeField] private float _changeDirectionTime;
        [SerializeField] private float _stayTimeMin;
        [SerializeField] private float _stayTimeMax;

        private Vector2 _direction;
        private Status _status;
        private float _moveTime;
        private float _changeTime;
        private float _stayTime;
        
        public Action<Vector2> OnDragVector;
        public Action OnDragStarted;
        public Action OnDragStopped;

        enum Status
        {
            Stay, Moving
        }

        public void Initialize()
        {
            _status = Status.Stay;
            _stayTime = Random.Range(_stayTimeMin, _stayTimeMax);
        }

        private void Update()
        {
            if (!_isEnabled)
            {
                return;
            }

            switch (_status)
            {
                case Status.Stay:
                    Stay();
                    break;
                case Status.Moving:
                    Move();
                    break;
            }
        }

        private void Move()
        {
            _moveTime -= Time.deltaTime;

            if (_moveTime < 0.0f)
            {
                OnDragStopped?.Invoke();
                _stayTime = Random.Range(_stayTimeMin, _stayTimeMax);
                _status = Status.Stay;
                return;
            }
            
            _changeTime -= Time.deltaTime;
            
            if (_changeTime < 0.0f)
            {
                _direction = Random.insideUnitCircle.normalized;
                _changeTime = _changeDirectionTime;
            }
            
            OnDragVector?.Invoke(_direction);
        }

        private void Stay()
        {
            _stayTime -= Time.deltaTime;

            if (_stayTime < 0.0f)
            {
                OnDragStarted?.Invoke();
                _moveTime = Random.Range(_moveTimeMin, _moveTimeMax);
                _status = Status.Moving;
                _direction = Random.insideUnitCircle.normalized;
                _changeTime = _changeDirectionTime;
            }
        }
    }
}
