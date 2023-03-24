using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodButton : MonoBehaviour
{
    public event EventHandler OnUsed;

    [SerializeField] private Material _greenMat;
    [SerializeField] private Material _darkGreenMat;

    private MeshRenderer _meshRenderer;
    private bool _canUseButton;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _canUseButton = true;
    }

    private void Start()
    {
        ResetButton();
    }

    private void ResetButton()
    {
        _meshRenderer.material = _greenMat;
        transform.localScale = new Vector3(1, 1.2f, 1);

        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, UnityEngine.Random.Range(-2.5f, 2.5f));

        _canUseButton = true;
    }

    public bool CanUseButton()
    {
        return _canUseButton;
    }

    public void UseButton()
    {
        if (_canUseButton)
        {
            _meshRenderer.material = _darkGreenMat;
            transform.localScale = new Vector3(1, 0.7f, 1);
            _canUseButton = false;

            OnUsed?.Invoke(this, EventArgs.Empty);
        }
    }
}
