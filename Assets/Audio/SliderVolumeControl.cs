using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SliderVolumeControl : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider volumeSlider;

    // AudioMixer에서 설정한 파라미터 이름 (정확히 일치해야 함)
    private string mixerParameter = "MasterVol";

    void Start()
    {
        // 1. 슬라이더의 값이 변경될 때마다 SetVolume 함수가 실행되도록 연결
        volumeSlider.onValueChanged.AddListener(SetVolume);

        // (선택) 시작할 때 슬라이더의 초기값을 오디오 믹서에 맞춰주거나 1로 세팅
        volumeSlider.value = 1f;
    }

    // 슬라이더에서 값을 받아 오디오 믹서에 적용하는 함수
    public void SetVolume(float sliderValue)
    {
        // 핵심: 슬라이더 값(0~1)을 오디오 믹서의 데시벨(dB) 값(-80~0)으로 변환
        float dbVolume = Mathf.Log10(sliderValue) * 20f;

        // 오디오 믹서에 변환된 볼륨 적용
        audioMixer.SetFloat(mixerParameter, dbVolume);
    }
}
