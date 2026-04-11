/*using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class FirstPersonControllerSimple : MonoBehaviour
    {
        [Header("Movement")]
        public float MoveSpeed = 4f;
        public float SprintSpeed = 6f;
        public float SpeedChangeRate = 10f;

        [Header("Jump / Gravity")]
        public float JumpHeight = 1.2f;
        public float Gravity = -15f;
        public float JumpTimeout = 0.1f;
        public float FallTimeout = 0.15f;

        [Header("Ground Check")]
        public bool Grounded = true;
        public float GroundedOffset = -0.14f;
        public float GroundedRadius = 0.35f;
        public LayerMask GroundLayers;

        [Header("Audio")]
        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;
        public float FootstepInterval = 0.45f;

        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private Animator _animator;
        private bool _hasAnimator;

        private float _speed;
        private float _verticalVelocity;
        private float _terminalVelocity = 53f;
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;
        private float _footstepTimer;
        private bool _wasGrounded;

        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
            _hasAnimator = TryGetComponent(out _animator);

            AssignAnimationIDs();

            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);

            GroundedCheck();
            JumpAndGravity();
            Move();
            HandleFootsteps();
            HandleLandingSound();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(
                transform.position.x,
                transform.position.y - GroundedOffset,
                transform.position.z
            );

            Grounded = Physics.CheckSphere(
                spherePosition,
                GroundedRadius,
                GroundLayers,
                QueryTriggerInteraction.Ignore
            );

            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void Move()
        {
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

            if (_input.move == Vector2.zero)
                targetSpeed = 0f;

            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0f, _controller.velocity.z).magnitude;
            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            Vector3 move = (transform.right * _input.move.x + transform.forward * _input.move.y).normalized;

            _controller.Move(move * (_speed * Time.deltaTime) +
                             Vector3.up * (_verticalVelocity * Time.deltaTime));

            if (_hasAnimator)
            {
                float animSpeed = move.magnitude * inputMagnitude;
                _animator.SetFloat(_animIDSpeed, animSpeed);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                _fallTimeoutDelta = FallTimeout;

                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                if (_verticalVelocity < 0f)
                    _verticalVelocity = -2f;

                if (_input.jump && _jumpTimeoutDelta <= 0f)
                {
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                if (_jumpTimeoutDelta >= 0f)
                    _jumpTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                _jumpTimeoutDelta = JumpTimeout;

                if (_fallTimeoutDelta >= 0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                _input.jump = false;
            }

            if (_verticalVelocity < _terminalVelocity)
                _verticalVelocity += Gravity * Time.deltaTime;
        }

        private void HandleFootsteps()
        {
            Vector3 horizontalVelocity = new Vector3(_controller.velocity.x, 0f, _controller.velocity.z);
            bool isMoving = horizontalVelocity.magnitude > 0.1f;

            if (Grounded && isMoving)
            {
                _footstepTimer -= Time.deltaTime;

                if (_footstepTimer <= 0f)
                {
                    PlayFootstepSound();
                    _footstepTimer = FootstepInterval;
                }
            }
            else
            {
                _footstepTimer = 0f;
            }
        }

        private void PlayFootstepSound()
        {
            if (FootstepAudioClips != null && FootstepAudioClips.Length > 0)
            {
                int index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(
                    FootstepAudioClips[index],
                    transform.position + _controller.center,
                    FootstepAudioVolume
                );
            }
        }

        private void HandleLandingSound()
        {
            if (!_wasGrounded && Grounded && LandingAudioClip != null)
            {
                AudioSource.PlayClipAtPoint(
                    LandingAudioClip,
                    transform.position + _controller.center,
                    FootstepAudioVolume
                );
            }

            _wasGrounded = Grounded;
        }

        private void OnDrawGizmosSelected()
        {
            Color green = new Color(0f, 1f, 0f, 0.35f);
            Color red = new Color(1f, 0f, 0f, 0.35f);

            Gizmos.color = Grounded ? green : red;
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius
            );
        }
    }
}*/