using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;

namespace Platformer.Mechanics
{
    /// <summary>
    /// This is the main class used to implement control of the player.
    /// It is a superset of the AnimationController class, but is inlined to allow for any kind of customisation.
    /// </summary>
    public class PlayerController : KinematicObject
    {
        public AudioClip jumpAudio;
        public AudioClip respawnAudio;
        public AudioClip ouchAudio;

        /// <summary>
        /// Max horizontal speed of the player.
        /// </summary>
        public float maxSpeed = 7;
        /// <summary>
        /// Initial jump velocity at the start of a jump.
        /// </summary>
        public float jumpTakeOffSpeed = 7;

        public JumpState jumpState = JumpState.Grounded;
        private bool stopJump;
        /*internal new*/ public Collider2D collider2d;
        /*internal new*/ public AudioSource audioSource;
        public Health health;
        public bool controlEnabled = true;

        public GameObject bulletPrefab;
        protected Vector2 muzzlePos;
        public float interval;
        public float timer;
        float direction;

        bool jump;
        Vector2 move;
        SpriteRenderer spriteRenderer;
        internal Animator animator;
        readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public Bounds Bounds => collider2d.bounds;

        void Awake()
        {
            health = GetComponent<Health>();
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            direction = 1f;
        }

        protected override void Update()
        {
            if (controlEnabled)
            {
                move.x = Input.GetAxis("Horizontal");
                if (jumpState == JumpState.Grounded && Input.GetButtonDown("Jump"))
                    jumpState = JumpState.PrepareToJump;
                else if (Input.GetButtonUp("Jump"))
                {
                    stopJump = true;
                    Schedule<PlayerStopJump>().player = this;
                }
                Shoot();
            }
            else
            {
                move.x = 0;
            }
            UpdateJumpState();
            if (Input.GetAxis("Horizontal") > 0)
            {
                direction = 1;
            } else if(Input.GetAxis("Horizontal") < 0)
            {
                direction = -1;
            }
            
            base.Update();
        }

        void UpdateJumpState()
        {
            jump = false;
            switch (jumpState)
            {
                case JumpState.PrepareToJump:
                    jumpState = JumpState.Jumping;
                    jump = true;
                    stopJump = false;
                    break;
                case JumpState.Jumping:
                    if (!IsGrounded)
                    {
                        Schedule<PlayerJumped>().player = this;
                        jumpState = JumpState.InFlight;
                    }
                    break;
                case JumpState.InFlight:
                    if (IsGrounded)
                    {
                        Schedule<PlayerLanded>().player = this;
                        jumpState = JumpState.Landed;
                    }
                    break;
                case JumpState.Landed:
                    jumpState = JumpState.Grounded;
                    break;
            }
        }

        protected override void ComputeVelocity()
        {
            if (jump && IsGrounded)
            {
                velocity.y = jumpTakeOffSpeed * model.jumpModifier;
                jump = false;
            }
            else if (stopJump)
            {
                stopJump = false;
                if (velocity.y > 0)
                {
                    velocity.y = velocity.y * model.jumpDeceleration;
                }
            }

            if (move.x > 0.01f)
                spriteRenderer.flipX = false;
            else if (move.x < -0.01f)
                spriteRenderer.flipX = true;

            animator.SetBool("grounded", IsGrounded);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
            animator.SetFloat("Speed", Mathf.Abs(velocity.x));
            targetVelocity = move * maxSpeed;
        }

        protected virtual void Shoot()
        {
            

            if (timer != 0)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                    timer = 0;
            }

            if (Input.GetButton("Fire1"))
            {
                if (timer == 0)
                {
                    timer = interval;
                    // StartCoroutine(FindObjectOfType<CameraController>().CameraShake(0.4f,0.4f));
                    animator.SetBool("IsShooting", true);
                    // GameObject bullet = Instantiate(bulletPrefab, muzzlePos.position, Quaternion.identity);
                    GameObject bullet = ObjectPool.Instance.GetObject(bulletPrefab);
                    muzzlePos.x = transform.position.x + direction * 0.9f;
                    muzzlePos.y = transform.position.y;
                    bullet.transform.position = muzzlePos;
                    bullet.GetComponent<Bullet>().SetSpeed( new Vector2(1,0)*direction);
                    animator.SetBool("IsShooting", false);
                }
            }
        }

        public enum JumpState
        {   
            Grounded,
            PrepareToJump,
            Jumping,
            InFlight,
            Landed
        }
    }
}