using Assets.Scripts.GameSystem.PlayerDice;
using Assets.Scripts.GameSystem.Tiles;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem.Units
{
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField] private Dice _dice;
        [SerializeField] private Transform _movePoint;       
        [SerializeField] private float _moveSpeed;
        [SerializeField] private GridSpawner _grid;
        [SerializeField] private Button _right;
        [SerializeField] private Button _left;
        [SerializeField] private Button _up;
        [SerializeField] private Button _down;
        [SerializeField] private Button _rollDice;
        [SerializeField] private TMP_Text _rollCounter;

        private float _step;
        private int _rollAmount;
        private int _reroll = 1;
        public Unit _unit;
        private Player _player;
        private Vector3 _nextMovePoint;
        private Vector3 _nextPlayerPos;
        private Vector3 _nextPathRight;
        private Vector3 _nextPathLeft;
        private Vector3 _nextPathUp;
        private Vector3 _nextPathDown;
        private Tile _nextTile;
        private Tile _currentTile;
        private Tile _endTile;
        private List<Tile> _rightTiles = new List<Tile>();
        private List<Tile> _leftTiles = new List<Tile>();
        private List<Tile> _upTiles = new List<Tile>();
        private List<Tile> _downTiles = new List<Tile>();

        public event UnityAction PlayerMoved;

        private void Start()
        {
            _movePoint.parent = null;
            _rollAmount = _reroll;
            _rollCounter.text = _rollAmount.ToString();
        }

        private void OnEnable()
        {
            _right.onClick.AddListener(() => OnDirectionButton(_rightTiles));
            _left.onClick.AddListener(() => OnDirectionButton(_leftTiles));
            _up.onClick.AddListener(() => OnDirectionButton(_upTiles));
            _down.onClick.AddListener(() => OnDirectionButton(_downTiles));
            _rollDice.onClick.AddListener(RollDice);
            _dice.OnDiceResult += CalculatePath;
        }

        private void OnDestroy()
        {
            _right.onClick.RemoveListener(() => OnDirectionButton(_rightTiles));
            _left.onClick.RemoveListener(() => OnDirectionButton(_leftTiles));
            _up.onClick.RemoveListener(() => OnDirectionButton(_upTiles));
            _down.onClick.RemoveListener(() => OnDirectionButton(_downTiles));
            _rollDice.onClick.RemoveListener(RollDice);
            _dice.OnDiceResult -= CalculatePath;
        }

        public void InitPlayer(Player player)
        {
            _player = player;
            _movePoint.position = _player.transform.position;
        }

        private void RollDice()
        {            
            ClearPath(_rightTiles);
            ClearPath(_leftTiles);
            ClearPath(_upTiles);
            ClearPath(_downTiles);
            _rollAmount--;
            _rollCounter.text = _rollAmount.ToString();
            _dice.Roll();
            DisactivateDice();
        }

        public void GetReroll()
        {
            _reroll++;
            _rollAmount = _reroll;
            _rollCounter.text = _rollAmount.ToString();
        }

        public void ActivateDice()
        {
            _rollDice.interactable = true;
            _rollAmount = _reroll;
            _rollCounter.text = _rollAmount.ToString();
        }


        private void DisactivateDice() =>
            _rollDice.interactable = false;
       

        private void OnDirectionButton(List<Tile> tiles)
        {
            if (tiles.Count == 0) return;

            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[0].IsWalkable == false) return;

                if (tiles[i].IsWalkable == false)
                {
                    _nextMovePoint = tiles[i - 1].transform.position;
                    _movePoint.transform.position = _nextMovePoint;
                    _nextPlayerPos = _movePoint.position;
                    StartCoroutine(Move(_nextPlayerPos));
                    ClearPath(_rightTiles);
                    ClearPath(_leftTiles);
                    ClearPath(_upTiles);
                    ClearPath(_downTiles);
                    return;
                }
            }

            _nextMovePoint = tiles[tiles.Count - 1].transform.position;
            _movePoint.transform.position = _nextMovePoint;
            _nextPlayerPos = _movePoint.position;            

            StartCoroutine(Move(_nextPlayerPos));
            ClearPath(_rightTiles);
            ClearPath(_leftTiles);
            ClearPath(_upTiles);
            ClearPath(_downTiles);
        }                     

        private IEnumerator Move(Vector3 newPos)
        {
            DisactivateDice();

            while (_player.transform.position != newPos)
            {
                _player.transform.position = Vector3.MoveTowards(_player.transform.position, newPos, _moveSpeed * Time.deltaTime);
                yield return null;
            }

            _endTile = _grid.GetTile(newPos);

            if (_endTile.IsOcupied)
            {
                _unit = _endTile.OcupiedUnit;
                if (_unit.GetComponent<Enemy>())
                {
                    _player.GetComponent<Health>().TakeDamage(_unit.Damage);
                    _unit.GetComponent<Health>().TakeDamage(_unit.Damage);
                }                
            }

            yield return new WaitForSeconds(0.1f);
            PlayerMoved?.Invoke();
        }

        private void CalculatePath(int step)
        {
            if (_rollAmount > 0) _rollDice.interactable = true;

            _step = step;
            _currentTile = _grid.GetTile(_player.transform.position);
            float positiveX = _currentTile.transform.position.x;
            float negativeX = _currentTile.transform.position.x;
            float positiveY = _currentTile.transform.position.y;
            float negativeY = _currentTile.transform.position.y;
            float z = _currentTile.transform.position.z;

            for (int i = 0; i < _step; i++)
            {
                positiveX += 1;
                positiveY += 1;
                negativeX -= 1;
                negativeY -= 1;

                _nextPathRight = new Vector3(positiveX, _currentTile.transform.position.y, z);
                _nextTile = _grid.GetTile(_nextPathRight);

                if (_nextTile != null)
                    _rightTiles.Add(_nextTile);

                _nextPathLeft = new Vector3(negativeX, _currentTile.transform.position.y, z);
                _nextTile = _grid.GetTile(_nextPathLeft);

                if (_nextTile != null)
                    _leftTiles.Add(_nextTile);

                _nextPathUp = new Vector3(_currentTile.transform.position.x, positiveY, z);
                _nextTile = _grid.GetTile(_nextPathUp);

                if (_nextTile != null)
                    _upTiles.Add(_nextTile);

                _nextPathDown = new Vector3(_currentTile.transform.position.x, negativeY, z);
                _nextTile = _grid.GetTile(_nextPathDown);

                if (_nextTile != null)
                    _downTiles.Add(_nextTile);
            }

            ShowEndPoint(_rightTiles);
            ShowEndPoint(_leftTiles);
            ShowEndPoint(_upTiles);
            ShowEndPoint(_downTiles);
        }

        private void ClearPath(List<Tile> tiles)
        {
            foreach (var tile in tiles)
            {
                if (tile.IsWalkable) tile.HideTile();
            }

            tiles.Clear();
        }

        private void ShowEndPoint(List<Tile> tiles)
        {
            if (tiles.Count == 0) return;

            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[0].IsWalkable == false) return;

                if (tiles[i].IsWalkable == false)
                {
                    tiles[i - 1].ShowTile();
                    return;
                }                
            }

            tiles[tiles.Count - 1].ShowTile();
        }                
    }
}