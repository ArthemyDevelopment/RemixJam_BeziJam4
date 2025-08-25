using System.Collections;
using ArthemyDev.ScriptsTools;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public class SFXManager : SingletonManager<SFXManager>
{

    [BoxGroup("References"), SerializeField] private AudioSource VoiceSource;
    [BoxGroup("References"), SerializeField] private AudioSource LockSound;
    [BoxGroup("References"), SerializeField] private AudioSource RotateSound;
    [BoxGroup("References"), SerializeField] private AudioSource HoldSound;
    [BoxGroup("References"), SerializeField] private AudioSource LineSound;
    [BoxGroup("References"), SerializeField] private AudioSource MoveSound;
    

    [BoxGroup("Sfx Settings"), SerializeField] private float RepeatClearedLinesDelay;

    [FoldoutGroup("DEBUG"), SerializeField, ReadOnly]private bool canPlaySFX = true;


    public void TriggerLockSound()
    {
        if (!canPlaySFX) return;
        LockSound.Play();
    }

    public void TriggerRotateSound()
    {
        if (!canPlaySFX) return;
        RotateSound.Play();
    }

    public void TriggerHoldSound()
    {
        if (!canPlaySFX) return;
        HoldSound.Play();
    }
    public void TriggerMoveSound()
    {
        if (!canPlaySFX) return;
        MoveSound.Play();
    }

    public void TriggerLineSound(int clearedLines)
    {
        StartCoroutine(TriggerLineCoroutine(clearedLines));
    }

    public void TriggerEffectVoice(AudioClip voice)
    {
        VoiceSource.Stop();
        VoiceSource.clip = voice;
        VoiceSource.Play();
        
    }


    IEnumerator TriggerLineCoroutine(int clearedLines)
    {
        canPlaySFX = false;
        for (int i = 0; i < clearedLines; i++)
        {
            LineSound.Play();
            yield return ScriptsTools.GetWait(RepeatClearedLinesDelay);
        }

        StartCoroutine(ResumeSFX(LineSound.clip.length));
    }

    IEnumerator ResumeSFX(float time)
    {
        yield return ScriptsTools.GetWait(time);
        canPlaySFX = true;
    }
}
