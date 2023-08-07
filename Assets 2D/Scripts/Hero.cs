using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class Hero : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private int _lives = 5;
    [SerializeField] private float _distance = 3f;
    [SerializeField] private float jumpForce = 15f;
    private bool isGrounded;
    private bool isCanAtacked = true;
    private bool isAttackedNow = false;

    public Transform GroundCheck;
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer _sprite;
    
    private States State
    {
        get { return (States)animator.GetInteger("state"); }
        set { animator.SetInteger("state", (int)value); }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _sprite = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        CheckGround();
    }

    private void Update()
    {
        if (isGrounded && !isAttackedNow) State = States.idle;
        if (Input.GetButtonDown("Fire1") && isCanAtacked)
        {
            isCanAtacked = false;
            isAttackedNow = true;
            DealDamage();
            StartCoroutine(WaitAttackRecharge());
        }

        if (Input.GetButton("Horizontal"))
            Run();
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            Jump();

        Raycasting();

        if (_lives <= 0)
            Destroy(gameObject);
    }

    private void Raycasting()
    {
        if (_sprite.flipX)
            _distance = -_distance;
        if (!_sprite.flipX && _distance < 0)
            _distance = -_distance;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, _distance);
        Debug.DrawRay(transform.position, transform.right * _distance, Color.yellow);
        if (hit.collider.TryGetComponent<Monster>(out Monster monster))
        {
            monster.GetDamage();
            Debug.Log("Collided");
        }
    }

    private void Run()
    {
        if (isGrounded && !isAttackedNow) State = States.run;

        Vector3 dir = transform.right * Input.GetAxis("Horizontal");
        transform.position = Vector3.MoveTowards(transform.position,
            transform.position + dir, speed * Time.deltaTime);
        _sprite.flipX = dir.x < 0.0f;
    }

    private void Jump()
    {
        
        rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }

    private void CheckGround()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(GroundCheck.position, 0.1f);
        isGrounded = collider.Length > 3;
        if (!isGrounded) State = States.jump;
    }

    private void DealDamage()
    {
        var rnd = new System.Random();
        var typeAttack = rnd.Next(1, 3);

        if (typeAttack == 1)
            State = States.attack;
        if (typeAttack == 2)
            State = States.attack2;
    }

    public void GetDamage()
    {
        _lives -= 1;
    }

    private IEnumerator WaitAttackRecharge()
    {
        yield return new WaitForSeconds(0.5f);
        isCanAtacked = true;
        isAttackedNow = false;
    }
}

public enum States
{
    idle,
    run,
    jump,
    attack,
    attack2
}

