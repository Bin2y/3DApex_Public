using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using static Define;
using static SoundClips;

public enum SoundType
{
    Bgm,
    Sfx,
    COUNT
}
public interface IListener
{
    void OnEvent(EventType eventType, object sender, params object[] paramObjects);
}
public class AudioManager : Singleton<AudioManager>, IListener
{
    [Header("#AudioMixer")]
    [SerializeField] private AudioMixer _audioMixer;

    [Header("#BGM")]
    [SerializeField] private AudioClip[] _bgmClips;
    [SerializeField] AudioSource _bgmPlayer = null;
    [Header("#SFX")]
    [SerializeField] private AudioClip[] _sfxClips;
    [SerializeField] AudioSource[] _sfxPlayer = null;

    private Dictionary<string, AudioClip> _bgmDic = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> _sfxDic = new Dictionary<string, AudioClip>();

    int Channels = 32;

    protected override void Awake()
    {
        _isDontDestroyOnLoad = true;
        base.Awake();
    }

    /// <summary>
    /// </summary>
    protected override void Start()
    {
        base.Start();

        SetDictionary();
        _sfxPlayer = new AudioSource[Channels];
        CreateAudioPlayer();
    }

    /// <summary>
    /// 세이브 된 세팅 파일 LocalData에서 불러오면 볼륨 조절
    /// </summary>
    public void Init()
    {
        // 이벤트 -= += 로 두 번 등록되는 것을 방지
        //EventManager.Instance.RemoveListener(EventType.OnSetVolume, this);
        //EventManager.Instance.AddListener(EventType.OnSetVolume, this);
    }

    private void SetDictionary()
    {
        for (int i = 0; i < _bgmClips.Length; i++)
        {
            _bgmDic.Add(_bgmClips[i].name, _bgmClips[i]);
        }
        for (int i = 0; i < _sfxClips.Length; i++)
        {
            _sfxDic.Add(_sfxClips[i].name, _sfxClips[i]);
        }
    }

    public void PlayBGM(SoundClips.BGM bgm)
    {
        string bgmName = bgm.ToString();

        if (!_bgmDic.TryGetValue(bgmName, out AudioClip clip))
        {
            Debug.LogError($"BGM '{bgmName}' not found.");
            return;
        }
/*        AudioClip clip = _bgmDic[bgmName];*/

        if (clip == null)
        {
            throw new System.NullReferenceException($"BGM Clip Does not Exist");
        }
        _bgmPlayer.clip = clip;
        _bgmPlayer.Play();
    }

    public void StopBgm()
    {
        _bgmPlayer?.Stop();
    }

    public void PlaySFX(SoundClips.SFX sfx)
    {
        string sfxName = sfx.ToString();
        if (!_sfxDic.TryGetValue(sfxName, out AudioClip clip))
        {
            Debug.LogError($"SFX '{sfxName}' not found.");
            return;
        }
        //AudioClip clip = _sfxDic[sfxName];


        foreach (AudioSource source in _sfxPlayer)
        {
            if (!source.isPlaying)
            {
                source.clip = clip;
                source.Play();
                return;
            }
        }
        Debug.LogWarning("All SFX channels are busy.");
        return;
    }

    /// <summary>
    /// 오디오를 미리 풀링해서 생성해놓고 초기화 까지 완료
    /// </summary>
    private void CreateAudioPlayer()
    {
        GameObject obj = new GameObject("BGMPlayer");
        obj.transform.SetParent(transform);
        AudioSource bgmPlayer = obj.AddComponent<AudioSource>();
        _bgmPlayer = bgmPlayer;
        _bgmPlayer.loop = true;
        _bgmPlayer.outputAudioMixerGroup = _audioMixer.FindMatchingGroups("BGM")[0];
        _bgmPlayer.playOnAwake = false;


        for (int i = 0; i < Channels; i++)
        {
            GameObject obj2 = new GameObject("SfxPlayer");
            obj2.transform.SetParent(transform);
            AudioSource SfxPlayer = obj2.AddComponent<AudioSource>();
            _sfxPlayer[i] = SfxPlayer;
            _sfxPlayer[i].loop = false;
            _sfxPlayer[i].playOnAwake = false;
            _sfxPlayer[i].spatialBlend = 0.5f;
            _sfxPlayer[i].outputAudioMixerGroup = _audioMixer.FindMatchingGroups("SFX")[0];
        }
    }

    /// <summary>
    /// 볼륨의 크기를 조절 오디오 믹서를 이용
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    public void SetVolume(SoundType type, float value)
    {
        _audioMixer.SetFloat(type.ToString(), value);
    }

    /// <summary>
    /// 오디오 매니저가 먼저 초기화, 생성 되어서 이벤트 매니저에다가 일단 등록하고 LocalData의 값을 받아서 볼륨을 조절
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="sender"></param>
    /// <param name="paramObjects"></param>
    public void OnEvent(EventType eventType, object sender, params object[] paramObjects)
    {
        /*if (eventType == EventType.OnSetVolume)
        {
            // 받아온 사운드 설정 데이터 저장
            LocalSoundSettingData settingData = paramObjects[0] as LocalSoundSettingData;

            if (settingData == null)
            {
                throw new System.NullReferenceException($"AudioManager::OnEvent() LocalSoundSettingData is null");
            }


            DataManager.Instance.LocalDataSystem.SaveJsonData(settingData);

            // 볼륨 조절
            SetVolume(SoundType.Bgm, Mathf.Log10(settingData.BGMVolume) * 20);
            SetVolume(SoundType.Sfx, Mathf.Log10(settingData.SFXVolume) * 20);
        }*/
    }
}

