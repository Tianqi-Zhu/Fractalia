using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]

public class RaymarchCam : SceneViewFilter
{    [SerializeField]    [Header("Global Settings")]
    private Shader _shader;

    public Material _raymarchMaterial
    {
        get
        {
            if (!_raymarchMat && _shader)
            {
                _raymarchMat = new Material(_shader);
                _raymarchMat.hideFlags = HideFlags.HideAndDontSave;
            }

            return _raymarchMat;
        }
    }

    private Material _raymarchMat;

    public Camera _camera
    {
        get
        {
            if (!_cam)
            {
                _cam = GetComponent<Camera>();
            }
            return _cam;
        }
    }
    private Camera _cam;    public float _precision;

    public Transform _directionalLight;    public Transform _player;    private float _forceFieldRad;    [Header("Visual Settings")]    public float _lightIntensity;
    [Range(0, 1)]
    public float _shadowIntensity;
    [Range(0, 1)]    public float _aoIntensity;
    public Color _mainColor;
    public Color _secColor;
    public Color _skyColor;    [Header("Fractal Settings")]    public float _scaleFactor;
    public int _iterations;    public Vector3 _modOffsetPos;    public Vector3 _iterationOffsetPos;
    public Vector3 _iterationOffsetRot;    [Header("transform Settings")]
    public Vector3 _globalPosition;
    public Vector3 _GlobalRotation;    public float _GlobalScale;
    public float _smoothRadius;    [HideInInspector]
    public Matrix4x4 _iterationTransform;

    [HideInInspector]
    public Matrix4x4 _globalTransform;


    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {        // Construct a Model Matrix for the iteration transform        _iterationTransform = Matrix4x4.TRS(            _iterationOffsetPos,            Quaternion.identity,            Vector3.one);        _iterationTransform *= Matrix4x4.TRS(            Vector3.zero,            Quaternion.Euler(_iterationOffsetRot),            Vector3.one);        // Send the matrix to our shader        _raymarchMaterial.SetMatrix("_iterationTransform", _iterationTransform.inverse);        _raymarchMaterial.SetVector("_modOffsetPos", _modOffsetPos);        _raymarchMaterial.SetFloat("_scaleFactor", _scaleFactor);        // Construct a Model Matrix for the global transform        _globalTransform = Matrix4x4.TRS(
            _globalPosition,
            Quaternion.identity,
            Vector3.one);
        _globalTransform *= Matrix4x4.TRS(
            Vector3.zero,
            Quaternion.Euler(_GlobalRotation),
            Vector3.one);
        // Send the matrix to our shader
        _raymarchMaterial.SetMatrix("_globalTransform", _globalTransform.inverse);        if (!_raymarchMaterial)
        {
            Graphics.Blit(source, destination);
            return;
        }        _raymarchMaterial.SetMatrix("_CamFrustrum", CamFrustrum(_camera));
        _raymarchMaterial.SetMatrix("_CamToWorld", _camera.cameraToWorldMatrix);        _raymarchMaterial.SetFloat("_maxDistance", Camera.main.farClipPlane);
        _raymarchMaterial.SetFloat("_precision", _precision);        _raymarchMaterial.SetFloat("_lightIntensity", _lightIntensity);
        _raymarchMaterial.SetFloat("_shadowIntensity", _shadowIntensity);
        _raymarchMaterial.SetFloat("_aoIntensity", _aoIntensity);        _raymarchMaterial.SetColor("_mainColor", _mainColor);
        _raymarchMaterial.SetColor("_secColor", _secColor);
        _raymarchMaterial.SetColor("_skyColor", _skyColor);        _raymarchMaterial.SetInt("_iterations", _iterations);        _raymarchMaterial.SetVector("_lightDir", _directionalLight ? _directionalLight.forward : Vector3.down);        _raymarchMaterial.SetFloat("_GlobalScale", _GlobalScale);
        _raymarchMaterial.SetFloat("_smoothRadius", _smoothRadius);        RenderTexture.active = destination;
        _raymarchMaterial.SetTexture("_MainTex", source);        _raymarchMaterial.SetVector("_player", _player ? _player.position : Vector3.zero);        _forceFieldRad = _player.gameObject.GetComponent<SphereCollider>().radius;        _raymarchMaterial.SetFloat("_forceFieldRad", _forceFieldRad);

        GL.PushMatrix();
        GL.LoadOrtho();
        _raymarchMaterial.SetPass(0);
        GL.Begin(GL.QUADS);

        //BL
        GL.MultiTexCoord2(0, 0.0f, 0.0f);
        GL.Vertex3(0.0f, 0.0f, 3.0f);

        //BR
        GL.MultiTexCoord2(0, 1.0f, 0.0f);
        GL.Vertex3(1.0f, 0.0f, 2.0f);

        //TR
        GL.MultiTexCoord2(0, 1.0f, 1.0f);
        GL.Vertex3(1.0f, 1.0f, 1.0f);

        //TL
        GL.MultiTexCoord2(0, 0.0f, 1.0f);
        GL.Vertex3(0.0f, 1.0f, 0.0f);

        GL.End();
        GL.PopMatrix();    }    private Matrix4x4 CamFrustrum(Camera cam)
    {
        Matrix4x4 frustrum = Matrix4x4.identity;
        float fov = Mathf.Tan((cam.fieldOfView * 0.5f) * Mathf.Deg2Rad);

        Vector3 goUp = Vector3.up * fov;
        Vector3 goRight = Vector3.right * fov * cam.aspect;

        Vector3 TL = (-Vector3.forward - goRight + goUp);
        Vector3 TR = (-Vector3.forward + goRight + goUp);
        Vector3 BL = (-Vector3.forward - goRight - goUp);
        Vector3 BR = (-Vector3.forward + goRight - goUp);

        frustrum.SetRow(0, TL);
        frustrum.SetRow(1, TR);
        frustrum.SetRow(2, BR);
        frustrum.SetRow(3, BL);


        return frustrum;
    }
}