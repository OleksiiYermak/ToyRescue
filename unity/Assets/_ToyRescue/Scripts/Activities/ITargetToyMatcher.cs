using System.Collections.Generic;

namespace ToyRescue.Activities
{
    public interface ITargetToyMatcher
    {
        bool IsTargetToy(IReadOnlyList<string> validCategories, string toyCategory, bool countsAsTarget);
    }
}
