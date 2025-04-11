using System;

namespace WindowsNotifications.Models
{
    /// <summary>
    /// Represents an action to take when a notification deadline is reached.
    /// </summary>
    public class DeadlineAction
    {
        /// <summary>
        /// Gets or sets the type of deadline action.
        /// </summary>
        public DeadlineActionType ActionType { get; set; }

        /// <summary>
        /// Gets or sets the command to execute when the deadline is reached.
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// Gets or sets the script to execute when the deadline is reached.
        /// </summary>
        public string Script { get; set; }

        /// <summary>
        /// Gets or sets the action to execute when the deadline is reached.
        /// </summary>
        public Action Action { get; set; }

        /// <summary>
        /// Creates a new deadline action that executes a command.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <returns>A new deadline action.</returns>
        public static DeadlineAction ExecuteCommand(string command)
        {
            return new DeadlineAction
            {
                ActionType = DeadlineActionType.Command,
                Command = command
            };
        }

        /// <summary>
        /// Creates a new deadline action that executes a PowerShell script.
        /// </summary>
        /// <param name="script">The PowerShell script to execute.</param>
        /// <returns>A new deadline action.</returns>
        public static DeadlineAction ExecuteScript(string script)
        {
            return new DeadlineAction
            {
                ActionType = DeadlineActionType.Script,
                Script = script
            };
        }

        /// <summary>
        /// Creates a new deadline action that executes a .NET action.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>A new deadline action.</returns>
        public static DeadlineAction ExecuteAction(Action action)
        {
            return new DeadlineAction
            {
                ActionType = DeadlineActionType.Action,
                Action = action
            };
        }
    }

    /// <summary>
    /// Defines the types of deadline actions.
    /// </summary>
    public enum DeadlineActionType
    {
        /// <summary>
        /// No action.
        /// </summary>
        None,

        /// <summary>
        /// Execute a command.
        /// </summary>
        Command,

        /// <summary>
        /// Execute a PowerShell script.
        /// </summary>
        Script,

        /// <summary>
        /// Execute a .NET action.
        /// </summary>
        Action
    }
}
