using UnityEngine;
using UnityEditor;

/// <summary>
/// Unity 에디터에서 안개(Fog) 설정을 실시간으로 제어하는 에디터 창을 만듭니다.
/// 이 창은 Window > Rendering > Fog Controller 메뉴를 통해 열 수 있습니다.
/// </summary>
public class FogControllerWindow : EditorWindow
{
    // 에디터 상단 메뉴에 "Window/Rendering/Fog Controller" 항목을 추가하고,
    // 클릭 시 ShowWindow() 메소드를 호출하여 창을 엽니다.
    [MenuItem("Window/Rendering/Fog Controller")]
    public static void ShowWindow()
    {
        // 기존에 열려있는 창을 가져오거나, 없으면 새로 생성합니다.
        GetWindow<FogControllerWindow>("Fog Controller");
    }

    /// <summary>
    /// 에디터 창의 GUI를 그리는 메소드입니다.
    /// 사용자의 입력에 따라 RenderSettings의 값을 변경합니다.
    /// </summary>
    void OnGUI()
    {
        // 창의 제목을 굵은 글씨로 표시합니다.
        GUILayout.Label("Scene Fog Settings", EditorStyles.boldLabel);

        // RenderSettings는 현재 씬의 렌더링 설정을 담고 있는 static 클래스입니다.
        // 이 값들을 직접 수정하면 씬에 바로 적용됩니다.

        // 1. 안개 활성화/비활성화 토글
        // EditorGUILayout.Toggle은 체크박스를 만들고, 체크 상태(bool)를 반환합니다.
        RenderSettings.fog = EditorGUILayout.Toggle("Enable Fog", RenderSettings.fog);

        // 안개가 활성화된 경우에만 아래 설정들을 보여줍니다.
        if (RenderSettings.fog)
        {
            // 2. 안개 색상 설정
            // ColorField는 색상을 선택할 수 있는 필드를 만듭니다.
            RenderSettings.fogColor = EditorGUILayout.ColorField("Fog Color", RenderSettings.fogColor);

            // 3. 안개 모드 설정 (Linear, Exponential, ExponentialSquared)
            // EnumPopup은 열거형(enum)의 값들을 드롭다운 메뉴로 보여줍니다.
            RenderSettings.fogMode = (FogMode)EditorGUILayout.EnumPopup("Fog Mode", RenderSettings.fogMode);

            // 4. 안개 모드에 따른 세부 설정
            // 선택된 안개 모드에 따라 필요한 설정만 표시합니다.
            switch (RenderSettings.fogMode)
            {
                case FogMode.Linear:
                    // Linear 모드: 시작과 끝 거리를 설정합니다.
                    RenderSettings.fogStartDistance = EditorGUILayout.FloatField("Start Distance",
RenderSettings.fogStartDistance);
                    RenderSettings.fogEndDistance = EditorGUILayout.FloatField("End Distance",
RenderSettings.fogEndDistance);
                    break;

                case FogMode.Exponential:
                case FogMode.ExponentialSquared:
                    // Exponential 모드: 농도(Density)를 설정합니다.
                    // Slider를 사용해 0과 1 사이의 값을 쉽게 조절할 수 있습니다.
                    RenderSettings.fogDensity = EditorGUILayout.Slider("Density", RenderSettings.fogDensity, 0f,
1f);
                    break;
            }
        }
    }
}