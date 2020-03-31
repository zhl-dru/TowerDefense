﻿using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

[System.Serializable]
public struct EnemyAnimator
{
    PlayableGraph graph;
    /// <summary>
    /// 动画混合器
    /// </summary>
    AnimationMixerPlayable mixer;
    /// <summary>
    /// 前一个
    /// </summary>
    Clip previousClip;
    /// <summary>
    /// 进度
    /// </summary>
    float transitionProgress;
    /// <summary>
    /// 速度
    /// </summary>
    const float transitionSpeed = 5f;

    public enum Clip { Move, Intro, Outro, Dying }

    public Clip CurrentClip { get; private set; }

    public bool IsDone => GetPlayable(CurrentClip).IsDone();









    public void Configure(Animator animator, EnemyAnimationConfig config)
    {
        graph = PlayableGraph.Create();
        graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        mixer = AnimationMixerPlayable.Create(graph, 4);

        var clip = AnimationClipPlayable.Create(graph, config.Move);
        clip.Pause();
        mixer.ConnectInput((int)Clip.Move, clip, 0);

        clip = AnimationClipPlayable.Create(graph, config.Intro);
        clip.SetDuration(config.Intro.length);
        mixer.ConnectInput((int)Clip.Intro, clip, 0);

        clip = AnimationClipPlayable.Create(graph, config.Outro);
        clip.SetDuration(config.Outro.length);
        clip.Pause();
        mixer.ConnectInput((int)Clip.Outro, clip, 0);

        clip = AnimationClipPlayable.Create(graph, config.Dying);
        clip.SetDuration(config.Dying.length);
        clip.Pause();
        mixer.ConnectInput((int)Clip.Dying, clip, 0);

        var output = AnimationPlayableOutput.Create(graph, "Enemy", animator);
        output.SetSourcePlayable(mixer);
    }

    //public void Play(float speed)
    //{
    //    graph.GetOutput(0).GetSourcePlayable().SetSpeed(speed);
    //    graph.Play();
    //}

    public void PlayIntro()
    {
        SetWeight(Clip.Intro, 1f);
        CurrentClip = Clip.Intro;
        graph.Play();
        transitionProgress = -1f;
    }

    void SetWeight(Clip clip, float weight)
    {
        mixer.SetInputWeight((int)clip, weight);
    }

    public void PlayMove(float speed)
    {
        //SetWeight(CurrentClip, 0f);
        //SetWeight(Clip.Move, 1f);
        ////GetPlayable(Clip.Move).SetSpeed(speed);
        //var clip = GetPlayable(Clip.Move);
        //clip.SetSpeed(speed);
        //clip.Play();
        //CurrentClip = Clip.Move;
        GetPlayable(Clip.Move).SetSpeed(speed);
        BeginTransition(Clip.Move);
    }

    public void PlayOutro()
    {
        //SetWeight(CurrentClip, 0f);
        //SetWeight(Clip.Outro, 1f);
        //GetPlayable(Clip.Outro).Play();
        //CurrentClip = Clip.Outro;
        BeginTransition(Clip.Outro);
    }

    Playable GetPlayable(Clip clip)
    {
        return mixer.GetInput((int)clip);
    }

    public void PlayDying()
    {
        BeginTransition(Clip.Dying);
    }






    public void Stop()
    {
        graph.Stop();
    }

    public void Destroy()
    {
        graph.Destroy();
    }

    void BeginTransition(Clip nextClip)
    {
        previousClip = CurrentClip;
        CurrentClip = nextClip;
        transitionProgress = 0f;
        GetPlayable(nextClip).Play();
    }

    public void GameUpdate()
    {
        if (transitionProgress >= 0f)
        {
            transitionProgress += Time.deltaTime * transitionSpeed;
            if (transitionProgress >= 1f)
            {
                transitionProgress = -1f;
                SetWeight(CurrentClip, 1f);
                SetWeight(previousClip, 0f);
            }
            else
            {
                SetWeight(CurrentClip, transitionProgress);
                SetWeight(previousClip, 1f - transitionProgress);
            }
        }
    }
}
