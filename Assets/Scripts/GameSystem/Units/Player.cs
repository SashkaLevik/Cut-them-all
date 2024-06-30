using Assets.Scripts.GameSystem.Tiles;
using UnityEngine;

namespace Assets.Scripts.GameSystem.Units
{
    public class Player : Unit
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Enemy enemy))
            {
                enemy.GetComponent<Health>().TakeDamage(_damage);
            }
            else if (collision.TryGetComponent(out Heart heart))
            {
                GetComponent<Health>().Heal();
                Destroy(heart.gameObject);
            }
            else if (collision.TryGetComponent(out Shield shield))
            {
                GetComponent<Health>().SetDefence();
                Destroy(shield.gameObject);
            }
        }
    }
}