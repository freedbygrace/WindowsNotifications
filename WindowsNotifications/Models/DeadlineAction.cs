using System;
using System.Diagnostics;

namespace WindowsNotifications.Models
{
    /// <summary>
    /// Represents an action to take when a notification deadline is reached
    /// </summary>
    public class DeadlineAction
    {
        /// <summary>
        /// The type of action to take
        /// </summary>
        public DeadlineActionType ActionType { get; set; } = DeadlineActionType.None;

        /// <summary>
        /// The command to execute (for Process action type)
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// The arguments for the command (for Process action type)
        /// </summary>
        public string Arguments { get; set; }

        /// <summary>
        /// The URL to open (for Url action type)
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The script to execute (for Script action type)
        /// </summary>
        public string Script { get; set; }

        /// <summary>
        /// The custom action to execute (for Custom action type)
        /// </summary>
        public Action<NotificationResult> CustomAction { get; set; }

        /// <summary>
        /// Creates a new DeadlineAction with no action
        /// </summary>
        public DeadlineAction()
        {
        }

        /// <summary>
        /// Creates a new DeadlineAction that runs a process
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="arguments">The arguments for the command</param>
        public DeadlineAction(string command, string arguments = null)
        {
            ActionType = DeadlineActionType.Process;
            Command = command;
            Arguments = arguments;
        }

        /// <summary>
        /// Creates a new DeadlineAction that opens a URL
        /// </summary>
        /// <param name="url">The URL to open</param>
        public static DeadlineAction OpenUrl(string url)
        {
            return new DeadlineAction
            {
                ActionType = DeadlineActionType.Url,
                Url = url
            };
        }

        /// <summary>
        /// Creates a new DeadlineAction that executes a PowerShell script
        /// </summary>
        /// <param name="script">The PowerShell script to execute</param>
        public static DeadlineAction ExecuteScript(string script)
        {
            return new DeadlineAction
            {
                ActionType = DeadlineActionType.Script,
                Script = script
            };
        }

        /// <summary>
        /// Creates a new DeadlineAction that executes a custom action
        /// </summary>
        /// <param name="action">The custom action to execute</param>
        public static DeadlineAction ExecuteCustomAction(Action<NotificationResult> action)
        {
            return new DeadlineAction
            {
                ActionType = DeadlineActionType.Custom,
                CustomAction = action
            };
        }

        /// <summary>
        /// Executes the deadline action
        /// </summary>
        /// <param name="result">The notification result</param>
        public void Execute(NotificationResult result)
        {
            try
            {
                switch (ActionType)
                {
                    case DeadlineActionType.Process:
                        if (!string.IsNullOrEmpty(Command))
                        {
                            Process.Start(Command, Arguments ?? string.Empty);
                        }
                        break;

                    case DeadlineActionType.Url:
                        if (!string.IsNullOrEmpty(Url))
                        {
                            Process.Start(Url);
                        }
                        break;

                    case DeadlineActionType.Script:
                        if (!string.IsNullOrEmpty(Script))
                        {
                            Process.Start("powershell.exe", $"-ExecutionPolicy Bypass -Command \"{Script}\"");
                        }
                        break;

                    case DeadlineActionType.Custom:
                        CustomAction?.Invoke(result);
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error executing deadline action: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// The type of action to take when a deadline is reached
    /// </summary>
    public enum DeadlineActionType
    {
        /// <summary>
        /// No action
        /// </summary>
        None,

        /// <summary>
        /// Run a process
        /// </summary>
        Process,

        /// <summary>
        /// Open a URL
        /// </summary>
        Url,

        /// <summary>
        /// Execute a PowerShell script
        /// </summary>
        Script,

        /// <summary>
        /// Execute a custom action
        /// </summary>
        Custom
    }
}
