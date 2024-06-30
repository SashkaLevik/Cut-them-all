using Assets.Scripts.GameSystem.Tiles;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
    public class GridSpawner : MonoBehaviour
    {
        [SerializeField] private float _xStartPos, _yStartPos;
        [SerializeField] private int _wedth, _height;
        [SerializeField] private float _xSpace, _ySpace;
        [SerializeField] private GameObject _battleField;
        [SerializeField] private Tile _obstacleTile, _walcableTile;

        private float z = 90f;
        private Dictionary<Vector3, Tile> _tiles;

        public void GenerateGrid()
        {
            _tiles = new Dictionary<Vector3, Tile>();

            for (int x = 0; x < _wedth; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    var randomTile = Random.Range(0, 20) == 2 ? _obstacleTile : _walcableTile;
                    var spawnedTile = Instantiate(randomTile, new Vector3(_xStartPos + (_xSpace * (x)), _yStartPos + (-_ySpace * (y)), z), Quaternion.identity);
                    spawnedTile.transform.SetParent(_battleField.transform);
                    _tiles.Add(spawnedTile.transform.position, spawnedTile);
                }
            }
        }

        public Tile GetSpawnTile()
        {
            return _tiles.Where(t => t.Value.IsFree).OrderBy(t => Random.value).First().Value;
        }

        public Tile GetTile(Vector3 pos)
        {
            return _tiles.Where(t => t.Key == pos).FirstOrDefault().Value;            
        }        
    }
}