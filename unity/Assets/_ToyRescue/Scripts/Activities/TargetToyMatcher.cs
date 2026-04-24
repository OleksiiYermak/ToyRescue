using System;
using System.Collections.Generic;

namespace ToyRescue.Activities
{
    public sealed class TargetToyMatcher : ITargetToyMatcher
    {
        public bool IsTargetToy(IReadOnlyList<string> validCategories, string toyCategory, bool countsAsTarget)
        {
            if (!countsAsTarget || string.IsNullOrWhiteSpace(toyCategory))
            {
                return false;
            }

            if (validCategories == null || validCategories.Count == 0)
            {
                return true;
            }

            for (int i = 0; i < validCategories.Count; i++)
            {
                if (string.Equals(validCategories[i], toyCategory, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
