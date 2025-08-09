using UnityEngine;

/// <summary>
/// GameObject에 추가하여 씬의 안개(Fog) 설정을 제어하는 스크립트입니다.
/// 에디터에서 값을 변경하면 씬에 실시간으로 적용됩니다.
/// </summary>
[ExecuteInEditMode] // 이 속성 덕분에 에디터 모드에서도 코드가 실행됩니다.
public class FogManager : MonoBehaviour
{
    [Header("World Fog Properties")]
    [Tooltip("안개를 활성화하거나 비활성화합니다.")]
    [SerializeField] private bool enableFog = true;

    [Tooltip("안개의 색상을 지정합니다.")]
    [SerializeField] private Color fogColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    [Tooltip("안개의 계산 방식을 선택합니다.")]
    [SerializeField] private FogMode fogMode = FogMode.Exponential;

    [Tooltip("안개가 시작되는 카메라와의 거리입니다. (Linear 모드 전용)")]
    [SerializeField] private float fogStartDistance = 0f;

    [Tooltip("안개가 완전히 불투명해지는 카메라와의 거리입니다. (Linear 모드 전용)")]
    [SerializeField] private float fogEndDistance = 300f;

    [Tooltip("안개의 농도를 조절합니다. (Exponential 모드 전용)")]
    [Range(0f, 1f)]
    [SerializeField] private float fogDensity = 0.02f;
    [SerializeField] private float fogDensityMultiplier = 0.25f; // 농도에 대한 보정 값, 필요시 사용

    [Header("Local Volumetric Fog Properties")]
    [SerializeField] private ParticleSystem localVolumetricFog;

    [SerializeField] private float localVolumetricFogDensity = 10f;
    private float currentlocalVolumetricFogDensity; // 현재 안개 농도, 필요시 사용

    [SerializeField] private Vector3 localVolumetricFogSize = new Vector3(10, 1, 10);
    private Vector3 currentlocalVolumetricFogSize; // 현재 안개 농도, 필요시 사용

    [SerializeField] private Color localVolumetricFogStartColor = new Color(1f, 1f, 1f, 0.25f);
    private Color currentlocalVolumetricFogStartColor; // 현재 안개 시작 색상, 필요시 사용

    [SerializeField] private Color localVolumetricFogEndColor = new Color(0.5f, 0.5f, 0.5f, 0.25f);
    private Color currentlocalVolumetricFogEndColor; // 현재 안개 끝 색상, 필요시 사용

    /// <summary>
    /// 이 컴포넌트가 활성화될 때 호출됩니다. (에디터 및 런타임 모두)
    /// </summary>
    private void OnEnable()
    {
        ApplyFogSettings();
        ApplyLocalVolumetricFogSettings();
    }

    /// <summary>
    /// 에디터에서 이 스크립트의 값이 변경될 때마다 호출됩니다.
    /// </summary>
    private void OnValidate()
    {
        ApplyFogSettings();
        ApplyLocalVolumetricFogSettings();
    }

    /// <summary>
    /// 스크립트에 설정된 값들을 실제 씬의 렌더링 설정에 적용합니다.
    /// </summary>
    private void ApplyFogSettings()
    {
        // 현재 씬의 렌더링 설정은 RenderSettings static 클래스를 통해 접근합니다.

        // 안개 활성화 여부를 먼저 설정합니다.
        RenderSettings.fog = this.enableFog;

        // 안개가 비활성화 상태라면 다른 값들은 적용할 필요가 없습니다.
        if (!RenderSettings.fog)
            return;

        // 나머지 안개 속성들을 적용합니다.
        RenderSettings.fogColor = this.fogColor;
        RenderSettings.fogMode = this.fogMode;

        // 안개 모드에 따라 다른 값을 적용합니다.
        switch (RenderSettings.fogMode)
        {
            case FogMode.Linear:
                RenderSettings.fogStartDistance = this.fogStartDistance;
                RenderSettings.fogEndDistance = this.fogEndDistance;
                break;

            case FogMode.Exponential:
            case FogMode.ExponentialSquared:
                RenderSettings.fogDensity = this.fogDensity * fogDensityMultiplier;
                break;
        }
    }

    private void ApplyLocalVolumetricFogSettings()
    {
        if (localVolumetricFog == null)
            return;

        // 파티클 시스템의 main 모듈을 가져옵니다.
        var main = localVolumetricFog.main;
        var shape = localVolumetricFog.shape;

        // 현재 농도와 설정된 농도가 같으면 아무 것도 하지 않습니다.
        if (currentlocalVolumetricFogDensity != localVolumetricFogDensity)
        {
            // startSize를 두 상수 사이의 랜덤 값으로 설정합니다.
            main.startSize = localVolumetricFogDensity;
            // 현재 농도를 업데이트합니다.
            currentlocalVolumetricFogDensity = localVolumetricFogDensity;

            // 현재 파티클을 즉시 멈추고 클리어한 뒤, 다시 재생합니다.
            localVolumetricFog.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            localVolumetricFog.Play();

            RestartParticleSystem();
        }

        if (currentlocalVolumetricFogStartColor != localVolumetricFogStartColor &&
            currentlocalVolumetricFogEndColor != localVolumetricFogEndColor)
        {
            main.startColor = new ParticleSystem.MinMaxGradient(localVolumetricFogStartColor, localVolumetricFogEndColor);
            currentlocalVolumetricFogStartColor = localVolumetricFogStartColor;
            currentlocalVolumetricFogEndColor = localVolumetricFogEndColor;

            RestartParticleSystem();
        }

        if (currentlocalVolumetricFogSize != localVolumetricFogSize)
        {


            shape.scale = localVolumetricFogSize;
            currentlocalVolumetricFogSize = localVolumetricFogSize; // x값만 사용, 필요시 y, z도 사용 가능

            RestartParticleSystem();
        }
    }

    // [ContextMenu] 속성은 인스펙터의 컴포넌트 메뉴에 함수를 추가해줍니다.
    // 파티클을 수동으로 재시작하고 싶을 때 유용합니다.
    [ContextMenu("수동으로 파티클 재시작")]
    private void RestartParticleSystem()
    {
        if (localVolumetricFog == null)
            return;

        // 현재 파티클을 즉시 멈추고 클리어한 뒤, 다시 재생합니다.
        localVolumetricFog.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        localVolumetricFog.Play();
    }
}