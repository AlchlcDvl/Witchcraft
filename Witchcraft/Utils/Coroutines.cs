using System.Collections;
using Home.Shared;

namespace Witchcraft.Utils;

public static class Coroutines
{
    public static T Start<T>(T coroutine) where T : IEnumerator
    {
        ApplicationController.ApplicationContext.StartCoroutine(coroutine);
        return coroutine;
    }

    public static void Stop(IEnumerator coroutine) => ApplicationController.ApplicationContext.StopCoroutine(coroutine);

    public static void PerformTimedAction(float duration, Action<float> action) => Start(CoPerformTimedAction(duration, action));

    public static IEnumerator CoPerformTimedAction(float duration, Action<float> action)
    {
        for (var t = 0f; t < duration; t += Time.deltaTime)
        {
            action(t / duration);
            yield return null;
        }

        action(1f);
    }

    public static IEnumerator Wait(float duration)
    {
        while (duration > 0)
        {
            duration -= Time.deltaTime;
            yield return null;
        }
    }
}