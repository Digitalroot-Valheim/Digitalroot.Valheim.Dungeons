using System;
using System.Reflection;
using UnityEngine;

namespace Digitalroot.Valheim.TrapSpawners.Logging
{
  public static class LoggingUtils
  {
    /// <summary>
    /// Handles Errors from Delegates.
    /// </summary>
    /// <param name="method"></param>
    /// <param name="exception"></param>
    public static void HandleDelegateError(MethodInfo method, Exception exception)
    {
      Debug.LogError($"Error: {method.Name}");
      Debug.LogException(exception);
    }
  }
}
