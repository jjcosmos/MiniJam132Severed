using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private Animator _animator;

    private readonly int _bInAirHash = Animator.StringToHash("bInAir");
    private readonly int _bHoldingJump = Animator.StringToHash("bHoldingJump");
    private readonly int _lookBlend = Animator.StringToHash("LookBlend");

    private const float _slideSlopeDotDiff = .0001f;

    private float _jumpCharge = 0f;
    private bool _onSlopeThisFrame;
    private bool _inAirThisFrame;

    private void Update()
    {
        CheckState();
        MoveFromInput();
        
        _animator.SetBool(_bInAirHash, _inAirThisFrame || _onSlopeThisFrame);
    }

    private void MoveFromInput()
    {
        if (Input.GetButton("Jump"))
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
        
        if (!_inAirThisFrame && Input.GetButtonUp("Jump"))
        {
            var dirVec = new Vector3( Mathf.Clamp(Mathf.RoundToInt(hAxis), -1f, 1f), 0, 0);
            _body.velocity = _jumpForce * _jumpCharge * (Vector3.up + dirVec).normalized;
            _jumpCharge = 0f;
        }
    }

    private void CheckState()
    {
        if (Physics.BoxCast(transform.position, Vector3.one * (.5f * _groundCastWidth), Vector3.down, out var hitInfo, Quaternion.identity,
                _groundCastRange,
                _groundMask))
        {
            var dot = Vector3.Dot(hitInfo.normal, Vector3.up);
            _onSlopeThisFrame = dot < (1f - _slideSlopeDotDiff);
            _inAirThisFrame = false;
        }
        else
        {
            _onSlopeThisFrame = false;
            _inAirThisFrame = true;
        }

        _collider.sharedMaterial = _onSlopeThisFrame switch
        {
            true when _collider.sharedMaterial != _slipperyMat => _slipperyMat,
            false when _collider.sharedMaterial != _standardMat => _standardMat,
            _ => _collider.sharedMaterial
        };
    }

    private void OnGUI()
    {
        GUILayout.Label($"On Slope: {_onSlopeThisFrame}\nIn Air: {_inAirThisFrame}\nCharge: {_jumpCharge}");
    }
}
