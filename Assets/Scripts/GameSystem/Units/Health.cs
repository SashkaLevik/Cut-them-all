using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.GameSystem.Units
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private float _maxHealth;
        [SerializeField] private GameObject _shieldAnim;

        private float _currentHealth;
        protected bool _isDefending;

        public event UnityAction<float> HealthChanged;
        public event UnityAction<Unit> Died;

        public float CurrentHP
        {
            get => _currentHealth;
            set
            {
                _currentHealth = value;
                HealthChanged?.Invoke(CurrentHP);
            }
        }

        public float MaxHP => _maxHealth;

        private void Start()
        {
            _currentHealth = _maxHealth;
        }

        public void TakeDamage(float damage)
        {
            if (_isDefending)
            {
                _isDefending = false;
                _shieldAnim.SetActive(false);
            }
            else
            {
                CurrentHP -= damage;
                if (CurrentHP <= 0)
                    Die();
            }            
        }

        public void Heal()
        {
            if (CurrentHP < _maxHealth)
                CurrentHP ++;
        }

        public void SetDefence()
        {
            _isDefending = true;
            _shieldAnim.SetActive(true);
        }            

        private void Die()
        {
            Died?.Invoke(GetComponent<Unit>());            
            Destroy(gameObject, 0.3f);
        }
    }
}