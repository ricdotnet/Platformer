using UnityEngine;

namespace Entities
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float jump;
        [SerializeField] private LayerMask groundLayer;

        private Rigidbody2D _player;
        private Animator _animator;
        private BoxCollider2D _boxCollider;

        private static readonly int Walk = Animator.StringToHash("walk");
        private static readonly int Grounded = Animator.StringToHash("grounded");
        private static readonly int Jump = Animator.StringToHash("jump");

        private void Start()
        {
            _player = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _boxCollider = GetComponent<BoxCollider2D>();

            _player.gravityScale = 3;
            speed = 5;
            jump = 10;
        }

        private void Update()
        {
            var horizontalInput = Input.GetAxis("Horizontal");
            _player.velocity = new Vector2(horizontalInput * speed, _player.velocity.y);

            var transform1 = transform;
            transform1.localScale = horizontalInput switch
            {
                > 0.01f => _player.gravityScale > 0 ? Vector3.one : new Vector3(-1, 1, 1),
                < -0.01f => _player.gravityScale > 0 ? new Vector3(-1, 1, 1) : Vector3.one,
                _ => transform1.localScale
            };

            _animator.SetBool(Walk, horizontalInput != 0);
            _animator.SetBool(Grounded, IsOnGround());
        
            if (Input.GetKey(KeyCode.UpArrow) && IsOnGround())
            {
                DoJump();
            }

            if (Input.GetKey(KeyCode.Space))
            {
                _player.gravityScale = -1 * _player.gravityScale;
                _player.rotation += 180;
            } 

            if (!IsOnGround())
            {
                speed = 3;
            }
            else
            {
                speed = 5;
            }
        }

        private bool IsOnGround()
        {
            var direction = _player.gravityScale > 0 ? Vector2.down : Vector2.up;
        
            var bounds = _boxCollider.bounds;
            var raycastHit = Physics2D.BoxCast(bounds.center, bounds.size, 0, direction, 0.1f, groundLayer);

            return raycastHit;
        }

        private void DoJump()
        {
            var jumpPower = _player.gravityScale > 0 ? jump * 1 : jump * -1;
        
            _player.velocity = new Vector2(_player.velocity.x, jumpPower);
            _animator.SetTrigger(Jump);
        }
    }
}