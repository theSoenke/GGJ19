﻿using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 1.0f;
    public float animationWalkSpeedFactor = 1.0f;

    private Vector3 inputVector;

    private Rigidbody rigidbodyComponent;
    private Animator animator;
    private Vector3 prevInputVector = new Vector3();
    public GameObject bullet;
    public Transform BulletParent;
    public AudioSource ShotAudio;
    public AudioSource PartyAudio;
    public float bulletSpeed = 1.0f;
    public float ShotSoundPitch = 0.1f;

    public Transform GunMuzzle;

    private GameStateMessage.GameState _gameState;


    private const float SMALL_FLOAT = 0.0001f;

    private void Start()
    {
        rigidbodyComponent = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        prevInputVector = new Vector3(1.0f, 0.0f, 0.0f);

        MessageBus.Subscribe<GameStateMessage>(this, OnGameStateEvent);
        MessageBus.Subscribe<PartyMessage>(this, OnParty);
    }

    private void OnParty(PartyMessage obj)
    {
        if(obj.Start == true)
        {
            PartyAudio.Play();
        }
    }

    void Update()
    {
        inputVector = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        if (inputVector.sqrMagnitude >= 1.0f)
        {
            inputVector = inputVector.normalized;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

        UpdateAnimator();        
        UpdateRotation();
    }

    void FixedUpdate()
    {
        UpdatePosition();
    }

    private void UpdateAnimator()
    {
        var animationWalkSpeed = animationWalkSpeedFactor * rigidbodyComponent.velocity.magnitude;
        animator.SetFloat("WalkSpeed", animationWalkSpeed);
    }

    private void Shoot()
    {
        if(!CanShoot()) return;

        GameObject bulletGameObject;
        if (BulletParent == null)
        {
            bulletGameObject = Instantiate(bullet, GunMuzzle.position, transform.rotation);
        }
        else
        {
            bulletGameObject = Instantiate(bullet, GunMuzzle.position, transform.rotation, BulletParent);
        }
        bulletGameObject.GetComponent<Rigidbody>().velocity = transform.forward * bulletSpeed;
        ShotAudio.pitch = 1 + UnityEngine.Random.Range(-ShotSoundPitch, ShotSoundPitch);
        ShotAudio.Play();
    }

    private void UpdatePosition()
    {       
        var speedVector = inputVector * movementSpeed;
        rigidbodyComponent.velocity = speedVector;
        //var nextPos = transform.position + speedVector * Time.fixedDeltaTime;
        //rigidbodyComponent.MovePosition(nextPos);
    }

    private bool CanShoot()
    {
        return _gameState == GameStateMessage.GameState.WaveStart;
    }

    private void UpdateRotation()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100))
        {
            transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
        }
    }


    private void OnGameStateEvent(GameStateMessage msg)
    {
        _gameState = msg.State;
    }
}
