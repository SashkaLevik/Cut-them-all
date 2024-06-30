using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem.Units
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] protected TMP_Text _amount;
        [SerializeField] protected Slider _slider;
        [SerializeField] protected float _healthChangeSpeed;

        protected Coroutine _currentCoroutine;

        public void SetValue(float max)
        {
            _slider.maxValue = max;
            _slider.value = max;
            _amount.text = max.ToString();
        }

        private IEnumerator UpdateBar(float target)
        {
            while (_slider.value != target)
            {
                _slider.value = Mathf.MoveTowards(_slider.value, target, _healthChangeSpeed * Time.deltaTime);
                yield return null;
            }
        }

        public void OnHealthChanged(float value)
        {
            if (_currentCoroutine != null)
            {
                StopCoroutine(_currentCoroutine);
            }

            _amount.text = value.ToString();
            _currentCoroutine = StartCoroutine(UpdateBar(value));
        }
    }
}