using System.Collections;
using Home.Shared;

namespace Witchcraft.Utils;

public static class Coroutines
{
    public static void PerformTimedAction(float duration, Action<float> action) => ApplicationController.ApplicationContext.StartCoroutine(CoPerformTimedAction(duration, action));

    public static IEnumerator CoPerformTimedAction(float duration, Action<float> action)
    {
        for (var t = 0f; t < duration; t += Time.deltaTime)
        {
            action(t / duration);
            yield return new WaitForEndOfFrame();
        }

        action(1f);
    }

    public static IEnumerator Wait(float duration)
    {
        while (duration > 0)
        {
            duration -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}