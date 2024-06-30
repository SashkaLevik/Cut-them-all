using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.GameSystem.PlayerDice
{
    public class Dice : MonoBehaviour
    {
        [SerializeField] private List<Transform> _faces;
        //[SerializeField] private List<DiceFace> _faces;
        [SerializeField] private float _throwForce;
        [SerializeField] private float _minRollValue;
        [SerializeField] private float _maxRollValue;

        private bool _isRolling;
        private int _topFace;
        private Rigidbody _rigidbody;
        private Vector3 _startPosition;

        public bool IsRolling => _isRolling;
        public List<Transform> Faces => _faces;

        public event UnityAction<int> OnDiceResult;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.isKinematic = true;
            _startPosition = transform.position;
        }

        public void Roll()
        {
            StartCoroutine(RollDice());
        }        

        private IEnumerator RollDice()
        {
            _isRolling = true;

            float randX = Random.Range(_minRollValue, _maxRollValue);
            float randY = Random.Range(_minRollValue, _maxRollValue);
            float randZ = Random.Range(_minRollValue, _maxRollValue);

            _rigidbody.isKinematic = false;
            _rigidbody.AddForce(new Vector3(0, 0, 1) * (_throwForce), ForceMode.Impulse);
            _rigidbody.AddTorque(randX, randY, randZ);
            yield return new WaitForSeconds(0.2f);
            yield return new WaitUntil(() => _rigidbody.velocity.magnitude == 0);
            GetDiceFace();
            yield return new WaitForSeconds(0.2f);
            _isRolling = false;
            yield return new WaitForSeconds(0.3f);
            _rigidbody.isKinematic = true;
            transform.position = _startPosition;
        }

        private void GetDiceFace()
        {
            _topFace = 0;
            float lastPosition = _faces[0].position.z;

            for (int i = 0; i < _faces.Count; i++)
            {
                if (_faces[i].position.z < lastPosition)
                {
                    lastPosition = _faces[i].position.z;
                    _topFace = i;
                }
            }

            OnDiceResult?.Invoke(_topFace + 1);
        }
    }
}