using System;
using System.Collections;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private Rigidbody _body;
    [SerializeField] private Collider _collider;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _groundCastRange = 1f;
    [SerializeField] private float _groundCastWidth = 1f;
    [SerializeField] private PhysicMaterial _standardMat;
    [SerializeField] private PhysicMaterial _slipperyMat;
    [SerializeField] private float _jumpChargeSpeed = 2f;
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _horizontalForce = 5f;
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _groundcastPoint;

    private readonly int _bInAirHash = Animator.StringToHash("bInAir");
    private readonly int _bHoldingJump = Animator.StringToHash("bHoldingJump");
    private readonly int _lookBlend = Animator.StringToHash("LookBlend");

    private const float _slideSlopeDotDiff = .0001f;

    private float _jumpCharge = 0f;
    private bool _onSlopeThisFrame;
    private bool _inAirThisFrame;
    private bool _fullyGrounded;

    public Rigidbody Body => _body;
    public bool CanMove { get; private set; } = false;

    private void Start()
    {
        _body.isKinematic = true;
        _body.isKinematic = false;
    }
    
    public void SetCanMove(bool canMove)
    {
        CanMove = canMove;
    }

    private void Update()
    {
        //CheckState();
        MoveFromInput();
        
        _animator.SetBool(_bInAirHash, _inAirThisFrame || _onSlopeThisFrame);
    }

    private void FixedUpdate()
    {
        CheckState();
    }

    private float _interpolatedDirection = 1f;
    private void MoveFromInput()
    {
        if (!CanMove)
        {
            _animator.SetFloat(_lookBlend, (_interpolatedDirection + 1f) /2f);
            return;
        }
        
        if (Input.GetButton("Jump") && !_inAirThisFrame && !_onSlopeThisFrame)
        {
            _jumpCharge += Time.deltaTime * _jumpChargeSpeed;
            _jumpCharge = Mathf.Clamp01(_jumpCharge);
            _animator.SetBool(_bHoldingJump, true);
        }
        else
        {
            _animator.SetBool(_bHoldingJump, false);
        }

        var hAxis = Input.GetAxis("Horizontal");
        _interpolatedDirection = Mathf.MoveTowards(_interpolatedDirection, Mathf.RoundToInt(Input.GetAxisRaw("Horizontal")), Time.deltaTime * 10f);
        _animator.SetFloat(_lookBlend, (_interpolatedDirection + 1f) /2f);

        if (!_inAirThisFrame && !_onSlopeThisFrame && Input.GetButtonUp("Jump"))
        {
            var dirVec = new Vector3( Mathf.Clamp(Mathf.RoundToInt(hAxis), -1f, 1f), 0, 0);
            _body.velocity = _jumpCharge *( (Vector3.up * _jumpForce) + (_horizontalForce * dirVec.normalized));
            _jumpCharge = 0f;
        }
    }

    private void CheckState()
    {
        var wasOnSlope = _onSlopeThisFrame;
        if (_body.velocity.y <= 0 && GroundCheck(out var hitInfo))
        {
            _onSlopeThisFrame = NormalIsSlope(hitInfo.normal);

            if (!_fullyGrounded && !_onSlopeThisFrame)
            {
                _body.velocity = Vector3.zero;
            }
            
            _inAirThisFrame = false;
        }
        else
        {
            _inAirThisFrame = true;
        }

        _collider.sharedMaterial = _onSlopeThisFrame switch
        {
            true when _collider.sharedMaterial != _slipperyMat => _slipperyMat,
            false when _collider.sharedMaterial != _standardMat => _standardMat,
            _ => _collider.sharedMaterial
        };

        _fullyGrounded = !_onSlopeThisFrame && !_inAirThisFrame;
        if (!wasOnSlope && _onSlopeThisFrame)
        {
            _body.velocity = Vector3.zero;
        }
    }

    private void OnGUI()
    {
        //GUILayout.Label($"On Slope: {_onSlopeThisFrame}\nIn Air: {_inAirThisFrame}\nCharge: {_jumpCharge}");
    }

    private const float BoxCastHeight = 0.09f;
    private bool GroundCheck(out RaycastHit hitInfo)
    {
        var midPoint = _groundcastPoint.position + new Vector3(0f, BoxCastHeight / 2f, 0);
        //return Physics.BoxCast(midPoint, new Vector3(.5f * _groundCastWidth, BoxCastHeight / 2f, 0.5f),
        //    Vector3.down, out hitInfo, Quaternion.identity,
        //    _groundCastRange,
        //    _groundMask);
        Span<Vector3> casts = stackalloc Vector3[]{midPoint, midPoint + Vector3.right * (_groundCastWidth * 0.5f),  midPoint - Vector3.right * (_groundCastWidth * 0.5f)};
        foreach (var t in casts)
        {
            if (Physics.Raycast(t, Vector3.down, out hitInfo, _groundCastRange, _groundMask))
            {
                return true;
            }
        }

        hitInfo = new RaycastHit();
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        //Gizmos.DrawWireCube(_groundcastPoint.position + new Vector3(0f, BoxCastHeight / 2f, 0), new Vector3( _groundCastWidth, BoxCastHeight, 0.5f) );
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireCube(_groundcastPoint.position + new Vector3(0f, BoxCastHeight / 2f, 0) + Vector3.down * _groundCastRange, new Vector3(_groundCastWidth, BoxCastHeight, 0.5f));
        //Gizmos.color = Color.white;
        
        var midPoint = _groundcastPoint.position + new Vector3(0f, BoxCastHeight / 2f, 0);
        //return Physics.BoxCast(midPoint, new Vector3(.5f * _groundCastWidth, BoxCastHeight / 2f, 0.5f),
        //    Vector3.down, out hitInfo, Quaternion.identity,
        //    _groundCastRange,
        //    _groundMask);
        Span<Vector3> casts = stackalloc Vector3[]{midPoint, midPoint + Vector3.right * (_groundCastWidth * 0.5f),  midPoint - Vector3.right * (_groundCastWidth * 0.5f)};
        foreach (var t in casts)
        {
            Gizmos.DrawLine(t, t + Vector3.down * _groundCastRange);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        var isGround = _groundMask == (_groundMask | (1 << collision.gameObject.layer));

        var grounded = GroundCheck(out _);
        var onFlatGround = AnyIsFlat(collision.contacts, collision.contactCount);
        if (collision.relativeVelocity.y > 0f && (isGround || grounded) && onFlatGround)
        {
            _body.velocity = Vector3.zero;
        }
        else if (collision.relativeVelocity.y < 0f)
        {
            //_body.velocity = Vector3.Scale(new Vector3(1f, 0f, 1f), _body.velocity);
        }
        else
        {
            //Debug.Log($"Failed to resolve, Grounded: {isGround} Flat: {onFlatGround} RelVel {collision.relativeVelocity.y}");
            //Debug.Break();
        }
    }

    private static bool AnyIsFlat(ContactPoint[] array, int count)
    {
        for (var i = 0; i < count; i++)
        {
            if(!NormalIsSlope(array[i].normal)) return true;
        }

        return false;
    }

    private static bool NormalIsSlope(Vector3 normal)
    {
        var dot = Vector3.Dot(normal, Vector3.up);
        return dot < (1f - _slideSlopeDotDiff);
    }
}
