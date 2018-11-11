using UnityEngine;
using Cinemachine;
using System.Collections;
using System;

/// <summary>
/// An add-on module for Cinemachine to shake the camera
/// </summary>
[ExecuteInEditMode]
[SaveDuringPlay]
[AddComponentMenu("")] // Hide in menu
public class CameraShake : CinemachineExtension
{
    [Tooltip("Amplitude of the shake")]
    public float m_Range = 0.5f;
    public float timeToShake = 0.5f;
    private bool isShake = false;

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Body && isShake)
        {
            Vector3 shakeAmount = GetOffset();
            state.PositionCorrection += shakeAmount;
        }
    }

    private void Start()
    {
        PlayerManager.instance.ShakeCamera += Shake;
    }

    void Shake(object sender, EventArgs e)
    {
        StartCoroutine(TimeShake());
    }

    IEnumerator TimeShake()
    {
        isShake = true;
        yield return new WaitForSeconds(timeToShake);
        isShake = false;
    }

    Vector3 GetOffset()
    {
        // Note: change this to something more interesting!
        return new Vector3(
            UnityEngine.Random.Range(-m_Range, m_Range),
            UnityEngine.Random.Range(-m_Range, m_Range),
            UnityEngine.Random.Range(-m_Range, m_Range));
    }
}