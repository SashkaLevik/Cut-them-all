using Assets.Scripts.GameSystem.Tiles;
using Assets.Scripts.GameSystem.Units;
using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
    public class BattleSystem : MonoBehaviour
    {
        private const string Score = "Score";

        [SerializeField] private Player _playerPrefab;
        [SerializeField] private Enemy _enemyPrefab;
        [SerializeField] private GridSpawner _gridSpawner;
        [SerializeField] private GameObject _battleField;
        [SerializeField] private PlayerMover _playerMover;
        [SerializeField] private HealthBar _healthBar;
        [SerializeField] private Soul _soul;
        [SerializeField] private GameObject _scorePosition;
        [SerializeField] private TMP_Text _scoreCounter;
        [SerializeField] private Unit _heartPrefab, _shieldPrefab;
        [SerializeField] private Button _exterminator;

        private float _delay = 0.1f;
        private int _score;
        private int _enemiesCount = 3;
        private int _boostChance = 70;
        private int _destroyNumber = 4;
        private Soul _spawnedSoul;
        private Player _player;
        private Health _playerHealth;
        private Unit _enemy;
        private List<Unit> _spawnedEnemies = new List<Unit>();

        private void Awake()=>
            _gridSpawner.GenerateGrid();

        private void OnEnable()
        {
            _exterminator.onClick.AddListener(Exterminate);
        }        

        public void Spawn()=>
            StartCoroutine(SpawnPlayer());

        public void RiseScore()
        {
            _score++;
            _scoreCounter.text = _score.ToString();
            PlayerPrefs.SetInt(Score, _score);
            CheckUpgrade();
        }

        private IEnumerator SpawnPlayer()
        {
            var heroTile = _gridSpawner.GetSpawnTile();
            _player = Instantiate(_playerPrefab, heroTile.transform.position, Quaternion.identity);
            _player.transform.SetParent(_battleField.transform);
            _playerMover.InitPlayer(_player);
            _playerMover.PlayerMoved += EndTurn;
            _playerHealth = _player.GetComponent<Health>();
            _playerHealth.HealthChanged += _healthBar.OnHealthChanged;
            _playerHealth.Died += OnPlayerDie;
            _healthBar.SetValue(_playerHealth.MaxHP);
            yield return new WaitForSeconds(_delay);
            StartCoroutine(SpawnEnemies());
        }

        private void OnPlayerDie(Unit player)
        {
            _playerHealth.HealthChanged -= _healthBar.OnHealthChanged;
        }        

        private IEnumerator SpawnEnemies()
        {            
            for (int i = 0; i < _enemiesCount; i++)
            {
                var enemyTile = _gridSpawner.GetSpawnTile();
                _enemy = Instantiate(_enemyPrefab, enemyTile.transform.position, Quaternion.identity);
                _enemy.transform.SetParent(_battleField.transform);
                _enemy.GetComponent<Health>().Died += OnEnemyDie;
                _spawnedEnemies.Add(_enemy);
                yield return new WaitForSeconds(_delay);
            }
        }

        private void OnEnemyDie(Unit enemy)
        {
            var enemyTile = _gridSpawner.GetTile(enemy.transform.position);
            _spawnedSoul =  Instantiate(_soul, enemyTile.transform.position, Quaternion.identity);
            _spawnedSoul.transform.SetParent(_battleField.transform);
            _spawnedSoul.Init(_scorePosition, this);
            _spawnedEnemies.Remove(enemy);
            enemy.GetComponent<Health>().Died -= OnEnemyDie;
        }        

        private void CheckUpgrade()
        {
            if (_score == 10)
                _playerMover.GetReroll();
            else if (_score == 30)
                GetExterminator();
            else if (_score == 60)
                _playerMover.GetReroll();
            else if (_score == 100)
                GetExterminator();
        }

        private void SpawnBooster()
        {
            var boosterTile = _gridSpawner.GetSpawnTile();
            var randomTile = Random.Range(0, 7) <= 3 ? _heartPrefab : _shieldPrefab;
            var booster = Instantiate(randomTile, boosterTile.transform.position, Quaternion.identity);
            booster.transform.SetParent(_battleField.transform);
        }

        private void GetExterminator() =>
            _exterminator.gameObject.SetActive(true);

        private void Exterminate()
        {
            if (_spawnedEnemies.Count > _destroyNumber)
                DestroyEnemies(_destroyNumber);
            else
                DestroyEnemies(_spawnedEnemies.Count);
            _exterminator.gameObject.SetActive(false);
        }

        private void DestroyEnemies(int value)
        {
            for (int i = 0; i < value; i++)
            {
                int randomEnemy = Random.Range(0, _spawnedEnemies.Count - 1);
                var enemyHealth = _spawnedEnemies[randomEnemy].GetComponent<Health>();
                enemyHealth.TakeDamage(_spawnedEnemies[randomEnemy].Damage);
            }
        }

        private void EndTurn()
        {
            int randomValue = Random.Range(0, 101);
            StartCoroutine(SpawnEnemies());
            _playerMover.ActivateDice();

            if (randomValue > _boostChance)
                SpawnBooster();
        }
    }
}