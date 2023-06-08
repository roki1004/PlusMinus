using UnityEngine;
using System.Collections;

public enum SOUND_TYPE
{
    BUTTON = 0,
    EFFECT,
    TYPE_LENGTH,
}

public enum BGM_INDEX
{
    LOBBY = 0,
    STAGE
}

public enum CLIP_INDEX
{
    OPTION_BUTTON = 0,
    STAGE_BUTTON,
    PAGE_BUTTON,
    OPEN_EFFECT,
    MOVE_BUTTON,
    BATTLE_EFFECT,
    TURN_REDUCE,
    UNDO_EFFECT,
    TURN_EFFECT,
    NORMAL_BUTTON,
}

public class SoundManager : MonoBehaviour
{
	//======================================
	private static SoundManager Instance = null;
	//======================================
	void Awake()
	{
		if(Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(this.gameObject);
		}

        DontDestroyOnLoad(this);

		MakeSource();
		SetClip();

		PlayBGM(BGM_INDEX.LOBBY);
	}
	//======================================
	private AudioSource _bgmSource;
	private AudioSource[] _sfxSource;
	private AudioClip[] _bgmClip;
	private AudioClip[] _sfxClip;
	//======================================
    void MakeSource()
    {
        _bgmSource = gameObject.AddComponent<AudioSource>();
        _bgmSource.loop = true;
        _bgmSource.volume = 0.35f;

		_sfxSource = new AudioSource[(int)SOUND_TYPE.TYPE_LENGTH];

        for(int i = 0; i < _sfxSource.Length; i++)
        {
            _sfxSource[i] = gameObject.AddComponent<AudioSource>();
            _sfxSource[i].loop = false;
            _sfxSource[i].playOnAwake = false;
        }
        
        //> Set sound sfx
		//_sfxSource[0].volume = 0.1f;
		//_sfxSource[1].volume = 0.13f;
		//_sfxSource[2].volume = 0.2f;
		//_sfxSource[3].volume = 0.15f;
		//_sfxSource[4].volume = 0.2f;
    }

	void SetClip()
	{
		_bgmClip = Resources.LoadAll<AudioClip>("Sound/BGM");
		_sfxClip = Resources.LoadAll<AudioClip>("Sound/SFX");
	}
	//======================================
	public void PlayBGM(BGM_INDEX index)
    {
		_bgmSource.clip = _bgmClip[(int)index];
        _bgmSource.Play();
    }

	public void StopBGM()
    {
        _bgmSource.Stop();
    }

	public void VolumeBGM(float volume)
    {
        _bgmSource.volume = volume;
    }
	//======================================
	public void PlaySFX(SOUND_TYPE type, CLIP_INDEX index)
    {
		_sfxSource[(int)type].PlayOneShot(_sfxClip[(int)index], 1.0f);
    }

	public void StopSFX(SOUND_TYPE type)
    {
        _sfxSource[(int)type].Stop();
    }

	public void VolumeSFX(SOUND_TYPE type, float volume)
    {
        _sfxSource[(int)type].volume = volume;
    }
	//======================================
}
