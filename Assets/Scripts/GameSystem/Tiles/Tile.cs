using Assets.Scripts.GameSystem.Units;
using System;
using UnityEngine;

namespace Assets.Scripts.GameSystem.Tiles
{
    public class Tile : MonoBehaviour
    {
        private const string IsBlinking = "IsBlinking";

        [SerializeField] private bool _isWalkable;

        private Animator _animator;
        private Unit _ocupiedUnit;
        //private bool _isFree = true;

        public bool IsWalkable => _isWalkable;
        public bool IsOcupied => _ocupiedUnit != null;
        public bool IsFree => _isWalkable && _ocupiedUnit == null;

        public Unit OcupiedUnit => _ocupiedUnit;

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        public void ShowTile()=>
            _animator.SetBool(IsBlinking, true);

        public void HideTile() =>
            _animator.SetBool(IsBlinking, false);

        public void Init(int x, int y) { }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Unit unit ))
            {
                if (_ocupiedUnit != null) return;
                _ocupiedUnit = unit;
            }
        }      

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Unit unit))
            {
                _ocupiedUnit = null;
            }
        }
    }
}