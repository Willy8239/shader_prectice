using UnityEngine;
using UnityEditor;

/// <summary>
/// Unity �����Ϳ��� �Ȱ�(Fog) ������ �ǽð����� �����ϴ� ������ â�� ����ϴ�.
/// �� â�� Window > Rendering > Fog Controller �޴��� ���� �� �� �ֽ��ϴ�.
/// </summary>
public class FogControllerWindow : EditorWindow
{
    // ������ ��� �޴��� "Window/Rendering/Fog Controller" �׸��� �߰��ϰ�,
    // Ŭ�� �� ShowWindow() �޼ҵ带 ȣ���Ͽ� â�� ���ϴ�.
    [MenuItem("Window/Rendering/Fog Controller")]
    public static void ShowWindow()
    {
        // ������ �����ִ� â�� �������ų�, ������ ���� �����մϴ�.
        GetWindow<FogControllerWindow>("Fog Controller");
    }

    /// <summary>
    /// ������ â�� GUI�� �׸��� �޼ҵ��Դϴ�.
    /// ������� �Է¿� ���� RenderSettings�� ���� �����մϴ�.
    /// </summary>
    void OnGUI()
    {
        // â�� ������ ���� �۾��� ǥ���մϴ�.
        GUILayout.Label("Scene Fog Settings", EditorStyles.boldLabel);

        // RenderSettings�� ���� ���� ������ ������ ��� �ִ� static Ŭ�����Դϴ�.
        // �� ������ ���� �����ϸ� ���� �ٷ� ����˴ϴ�.

        // 1. �Ȱ� Ȱ��ȭ/��Ȱ��ȭ ���
        // EditorGUILayout.Toggle�� üũ�ڽ��� �����, üũ ����(bool)�� ��ȯ�մϴ�.
        RenderSettings.fog = EditorGUILayout.Toggle("Enable Fog", RenderSettings.fog);

        // �Ȱ��� Ȱ��ȭ�� ��쿡�� �Ʒ� �������� �����ݴϴ�.
        if (RenderSettings.fog)
        {
            // 2. �Ȱ� ���� ����
            // ColorField�� ������ ������ �� �ִ� �ʵ带 ����ϴ�.
            RenderSettings.fogColor = EditorGUILayout.ColorField("Fog Color", RenderSettings.fogColor);

            // 3. �Ȱ� ��� ���� (Linear, Exponential, ExponentialSquared)
            // EnumPopup�� ������(enum)�� ������ ��Ӵٿ� �޴��� �����ݴϴ�.
            RenderSettings.fogMode = (FogMode)EditorGUILayout.EnumPopup("Fog Mode", RenderSettings.fogMode);

            // 4. �Ȱ� ��忡 ���� ���� ����
            // ���õ� �Ȱ� ��忡 ���� �ʿ��� ������ ǥ���մϴ�.
            switch (RenderSettings.fogMode)
            {
                case FogMode.Linear:
                    // Linear ���: ���۰� �� �Ÿ��� �����մϴ�.
                    RenderSettings.fogStartDistance = EditorGUILayout.FloatField("Start Distance",
RenderSettings.fogStartDistance);
                    RenderSettings.fogEndDistance = EditorGUILayout.FloatField("End Distance",
RenderSettings.fogEndDistance);
                    break;

                case FogMode.Exponential:
                case FogMode.ExponentialSquared:
                    // Exponential ���: ��(Density)�� �����մϴ�.
                    // Slider�� ����� 0�� 1 ������ ���� ���� ������ �� �ֽ��ϴ�.
                    RenderSettings.fogDensity = EditorGUILayout.Slider("Density", RenderSettings.fogDensity, 0f,
1f);
                    break;
            }
        }
    }
}