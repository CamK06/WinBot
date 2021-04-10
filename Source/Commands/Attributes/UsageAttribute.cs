using System;

namespace WinBot.Commands.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class UsageAttribute : Attribute
    {
        /// <summary>
        /// Gets the usage of this command
        /// </summary>
        public string Usage { get; }

        public UsageAttribute(string usage)
        {
            this.Usage = usage;
        }
    }
}