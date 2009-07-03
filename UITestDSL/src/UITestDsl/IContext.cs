using Ranorex;

namespace UITestDsl
{
    public interface IContext
    {
        /// <summary>
        /// Gets current form.
        /// </summary>
        Form Form { get; }

        /// <summary>
        /// Add <see cref="Ranorex.Form"/> provided to the form stack and sets it as current.
        /// </summary>
        /// <param name="form"></param>
        void AddForm( Form form );

        /// <summary>
        /// Add <see cref="Ranorex.Form"/> provided to the aliases collection aliases
        /// with given alias.
        /// </summary>
        /// <param name="form"></param>
        /// <param name="alias"></param>
        void AddForm( Form form, string alias );

        /// <summary>
        /// Get <see cref="Ranorex.Form"/> aliased by <paramref name="alias"/>.
        /// </summary>
        /// <param name="alias"></param>
        Form GetForm( string alias );
    }
}