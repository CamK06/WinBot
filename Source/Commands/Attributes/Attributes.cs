using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace WinBot.Util
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

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class CategoryAttribute : Attribute
    {
        /// <summary>
        /// Gets the category of this command
        /// </summary>
        public Category Category { get; }

        public CategoryAttribute(Category category)
        {
            this.Category = category;
        }
    }

    public enum Category
    {
        Main, Fun, Misc, Staff, Owner
    }
}