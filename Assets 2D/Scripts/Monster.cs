using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _distance;
    [SerializeField] private int _lives;
    [SerializeField] private Transform[] _moveSpots;
    [SerializeField] private SpriteRenderer _sprite;
    
    private int _currentSpot;
    private bool _isPursuitHero;

    private void Update()
    {
        ChangingMoveSpot();
        Raycasting();
        if(_lives <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void ChangingMoveSpot()
    {
        if (transform.position == _moveSpots[0].position)
        {
            _currentSpot = 1;
        }
        else if (transform.position == _moveSpots[1].position)
        {
            _currentSpot = 0;
        }
        if(!_isPursuitHero)
            Moving(_moveSpots[_currentSpot].transform);
    }

    private void Raycasting()
    {
        if (_sprite.flipX)
            _distance = -_distance;
        if (!_sprite.flipX && _distance < 0)
            _distance = -_distance;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, _distance);
        Debug.DrawRay(transform.position, transform.right * _distance, Color.yellow);
        if (hit.collider.TryGetComponent<Hero>(out Hero hero))
        {
            hero.GetDamage();
            Debug.Log("Collided");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("Trigger");
        if (collision.gameObject.TryGetComponent<Hero>(out Hero hero))
        {
            Debug.Log("Преследование");
            _isPursuitHero = true;
            Moving(hero.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _isPursuitHero = false;
    }

    private void Moving(Transform targetPositiom)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPositiom.position, _speed * Time.deltaTime);
        if(transform.position.x > targetPositiom.position.x)
            _sprite.flipX = true;
        else
            _sprite.flipX = false;
    }

    public void GetDamage()
    {
        _lives--;
    }
}
