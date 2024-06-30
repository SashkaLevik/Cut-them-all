using UnityEngine;

namespace Assets.Scripts.GameSystem.Units
{
    public class Unit : MonoBehaviour
    {
        [SerializeField] protected float _damage;

        public float Damage => _damage;               
    }
}