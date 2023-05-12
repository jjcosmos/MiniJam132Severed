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

    private readonly int _bInAirHash = Animator.StringToHash("bInAir");
    private readonly int _bHoldingJump = Animator.StringToHash("bHoldingJump");
    private readonly int _lookBlend = Animator.StringToHash("LookBlend");

    private const float _slideSlopeDotDiff = .0001f;

    private float _jumpCharge = 0f;
    private bool _onSlopeThisFrame;
    private bool _inAirThisFrame;
    private bool _fullyGrounded;
    
    private void Update()
    {
        CheckState();
        MoveFromInput();
        
        _animator.SetBool(_bInAirHash, _inAirThisFrame || _onSlopeThisFrame);
    }
    

    private void MoveFromInput()
    {
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
        _animator.SetFloat(_lookBlend, (hAxis + 1f) /2f);
        
        if (!_inAirThisFrame && !_onSlopeThisFrame && Input.GetButtonUp("Jump"))
        {
            var dirVec = new Vector3( Mathf.Clamp(Mathf.RoundToInt(hAxis), -1f, 1f), 0, 0);
            _body.velocity = _jumpCharge *( (Vector3.up * _jumpForce) + (_horizontalForce * dirVec.normalized));
            _jumpCharge = 0f;
        }
    }

    private void CheckState()
    {
        if (GroundCheck(out var hitInfo))
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
    }

    private void OnGUI()
    {
        GUILayout.Label($"On Slope: {_onSlopeThisFrame}\nIn Air: {_inAirThisFrame}\nCharge: {_jumpCharge}");
    }

    private bool GroundCheck(out RaycastHit hitInfo)
    {
        return Physics.BoxCast(transform.position + new Vector3(0, 0.01f, 0), Vector3.one * (.5f * _groundCastWidth),
            Vector3.down, out hitInfo, Quaternion.identity,
            _groundCastRange,
            _groundMask);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.y > 0f && GroundCheck(out _) && AnyIsFlat(collision.contacts, collision.contactCount))
        {
            _body.velocity = Vector3.zero;
        }
        else if (collision.relativeVelocity.y < 0f)
        {
            //_body.velocity = Vector3.Scale(new Vector3(1f, 0f, 1f), _body.velocity);
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
