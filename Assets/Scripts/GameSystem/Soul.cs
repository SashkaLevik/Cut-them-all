using System.Collections;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
    public class Soul : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed;

        private GameObject _target;
        private BattleSystem _battleSystem;

        private void Start()
        {
            StartCoroutine(MoveToTarget(_target.transform));
        }

        public void Init(GameObject target, BattleSystem battleSystem)
        {
            _target = target;
            _battleSystem = battleSystem;
        }            
        
        private IEnumerator MoveToTarget(Transform target)
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 200));
            yield return new WaitForSeconds(0.2f);

            while (transform.position != target.position)
            {                
                transform.position = Vector3.MoveTowards(transform.position, target.position, _moveSpeed * Time.deltaTime);
                yield return null;
            }

            _battleSystem.RiseScore();
            Destroy(gameObject);
        }
    }
}