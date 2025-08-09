using UnityEngine;

/// <summary>
/// GameObject�� �߰��Ͽ� ���� �Ȱ�(Fog) ������ �����ϴ� ��ũ��Ʈ�Դϴ�.
/// �����Ϳ��� ���� �����ϸ� ���� �ǽð����� ����˴ϴ�.
/// </summary>
[ExecuteInEditMode] // �� �Ӽ� ���п� ������ ��忡���� �ڵ尡 ����˴ϴ�.
public class FogManager : MonoBehaviour
{
    [Header("World Fog Properties")]
    [Tooltip("�Ȱ��� Ȱ��ȭ�ϰų� ��Ȱ��ȭ�մϴ�.")]
    [SerializeField] private bool enableFog = true;

    [Tooltip("�Ȱ��� ������ �����մϴ�.")]
    [SerializeField] private Color fogColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    [Tooltip("�Ȱ��� ��� ����� �����մϴ�.")]
    [SerializeField] private FogMode fogMode = FogMode.Exponential;

    [Tooltip("�Ȱ��� ���۵Ǵ� ī�޶���� �Ÿ��Դϴ�. (Linear ��� ����)")]
    [SerializeField] private float fogStartDistance = 0f;

    [Tooltip("�Ȱ��� ������ ������������ ī�޶���� �Ÿ��Դϴ�. (Linear ��� ����)")]
    [SerializeField] private float fogEndDistance = 300f;

    [Tooltip("�Ȱ��� �󵵸� �����մϴ�. (Exponential ��� ����)")]
    [Range(0f, 1f)]
    [SerializeField] private float fogDensity = 0.02f;
    [SerializeField] private float fogDensityMultiplier = 0.25f; // �󵵿� ���� ���� ��, �ʿ�� ���

    [Header("Local Volumetric Fog Properties")]
    [SerializeField] private ParticleSystem localVolumetricFog;

    [SerializeField] private float localVolumetricFogDensity = 10f;
    private float currentlocalVolumetricFogDensity; // ���� �Ȱ� ��, �ʿ�� ���

    [SerializeField] private Vector3 localVolumetricFogSize = new Vector3(10, 1, 10);
    private Vector3 currentlocalVolumetricFogSize; // ���� �Ȱ� ��, �ʿ�� ���

    [SerializeField] private Color localVolumetricFogStartColor = new Color(1f, 1f, 1f, 0.25f);
    private Color currentlocalVolumetricFogStartColor; // ���� �Ȱ� ���� ����, �ʿ�� ���

    [SerializeField] private Color localVolumetricFogEndColor = new Color(0.5f, 0.5f, 0.5f, 0.25f);
    private Color currentlocalVolumetricFogEndColor; // ���� �Ȱ� �� ����, �ʿ�� ���

    /// <summary>
    /// �� ������Ʈ�� Ȱ��ȭ�� �� ȣ��˴ϴ�. (������ �� ��Ÿ�� ���)
    /// </summary>
    private void OnEnable()
    {
        ApplyFogSettings();
        ApplyLocalVolumetricFogSettings();
    }

    /// <summary>
    /// �����Ϳ��� �� ��ũ��Ʈ�� ���� ����� ������ ȣ��˴ϴ�.
    /// </summary>
    private void OnValidate()
    {
        ApplyFogSettings();
        ApplyLocalVolumetricFogSettings();
    }

    /// <summary>
    /// ��ũ��Ʈ�� ������ ������ ���� ���� ������ ������ �����մϴ�.
    /// </summary>
    private void ApplyFogSettings()
    {
        // ���� ���� ������ ������ RenderSettings static Ŭ������ ���� �����մϴ�.

        // �Ȱ� Ȱ��ȭ ���θ� ���� �����մϴ�.
        RenderSettings.fog = this.enableFog;

        // �Ȱ��� ��Ȱ��ȭ ���¶�� �ٸ� ������ ������ �ʿ䰡 �����ϴ�.
        if (!RenderSettings.fog)
            return;

        // ������ �Ȱ� �Ӽ����� �����մϴ�.
        RenderSettings.fogColor = this.fogColor;
        RenderSettings.fogMode = this.fogMode;

        // �Ȱ� ��忡 ���� �ٸ� ���� �����մϴ�.
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

        // ��ƼŬ �ý����� main ����� �����ɴϴ�.
        var main = localVolumetricFog.main;
        var shape = localVolumetricFog.shape;

        // ���� �󵵿� ������ �󵵰� ������ �ƹ� �͵� ���� �ʽ��ϴ�.
        if (currentlocalVolumetricFogDensity != localVolumetricFogDensity)
        {
            // startSize�� �� ��� ������ ���� ������ �����մϴ�.
            main.startSize = localVolumetricFogDensity;
            // ���� �󵵸� ������Ʈ�մϴ�.
            currentlocalVolumetricFogDensity = localVolumetricFogDensity;

            // ���� ��ƼŬ�� ��� ���߰� Ŭ������ ��, �ٽ� ����մϴ�.
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
            currentlocalVolumetricFogSize = localVolumetricFogSize; // x���� ���, �ʿ�� y, z�� ��� ����

            RestartParticleSystem();
        }
    }

    // [ContextMenu] �Ӽ��� �ν������� ������Ʈ �޴��� �Լ��� �߰����ݴϴ�.
    // ��ƼŬ�� �������� ������ϰ� ���� �� �����մϴ�.
    [ContextMenu("�������� ��ƼŬ �����")]
    private void RestartParticleSystem()
    {
        if (localVolumetricFog == null)
            return;

        // ���� ��ƼŬ�� ��� ���߰� Ŭ������ ��, �ٽ� ����մϴ�.
        localVolumetricFog.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        localVolumetricFog.Play();
    }
}