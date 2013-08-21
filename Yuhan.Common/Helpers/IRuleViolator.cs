using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuhan.Common.Helpers
{
    public interface IRuleViolator
    {
        Boolean IsValid { get; }

        IEnumerable<RuleViolation> GetRuleViolations();
    }
}
