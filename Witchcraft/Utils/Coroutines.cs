using System.Collections;
using Home.Shared;

namespace Witchcraft.Utils;

public static class Coroutines
{
    public static Coroutine Start(IEnumerator coroutine) => ApplicationController.ApplicationContext.StartCoroutine(coroutine);

    public static void Stop(IEnumerator coroutine) => ApplicationController.ApplicationContext.StopCoroutine(coroutine);

    public static void PerformTimedAction(float duration, Action<float> action) => Start(CoPerformTimedAction(duration, action));

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
        yield return new WaitForSeconds(duration);
    }
}