using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Core;

namespace Platformer.Mechanics
{
    public class Bullet : MonoBehaviour
    {
        public float speed=20;
        protected int damage = 50;
        public GameObject explosionPrefab;
        new private Rigidbody2D rigidbody;

        void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();
        }

        public void SetSpeed(Vector2 direction)
        {
            rigidbody.velocity = direction * speed;
        }

        void Update()
        {

        }

        private void OnTriggerEnter2D(Collider2D _colInfo)
        {

            if (_colInfo.gameObject.tag == "Enemy")
            {
                Debug.Log("Hit");
               // GameObject exp = ObjectPool.Instance.GetObject(explosionPrefab);
              //  exp.transform.position = transform.position;
                Vector2 difference = (_colInfo.transform.position - transform.position).normalized;
                _colInfo.transform.position = new Vector2(_colInfo.transform.position.x + difference.x / 4,
                                                          _colInfo.transform.position.y + difference.y / 4);
                EnemyController enemy = _colInfo.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    //TODO Enemy damage
                    //enemy.TakeDamage(damage);
                }
                else
                {
                    Debug.Log("Enemy is null");
                }
            }

            // Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            ObjectPool.Instance.PushObject(gameObject);
        }
    }
}