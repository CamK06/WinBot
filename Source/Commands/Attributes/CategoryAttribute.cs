using System;

namespace WinBot.Commands.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class CategoryAttribute : Attribute
    {
        /// <summary>
        /// Gets the category of this command
        /// </summary>
        public Category category { get; }

        public CategoryAttribute(Category category)
        {
            this.category = category;
        }
    }

    public enum Category
    {
        Main, Fun, Misc, Staff, Owner, Images, NerdStuff
    }
}