# Session Context Optimization

Analyze current context usage and provide optimization recommendations or automatic cleanup.

## Instructions:

1. **Current Context Assessment**:
    - Analyze active context sources and their token consumption
    - Identify inefficient context usage patterns
    - Calculate context density (useful info per token)

2. **Optimization Strategies**:
    - **Heavy CLAUDE.md**: Suggest breaking into focused sections
    - **Large session files**: Recommend session cycling or summaries
    - **Excessive file reads**: Suggest targeted file access
    - **Tool call accumulation**: Recommend conversation cleanup
    - **Stale context**: Identify outdated information

3. **Provide Actionable Recommendations**:
   ● Context Optimization Analysis:
   Current Usage: ~[XX,XXX] tokens
   Efficiency Score: [X]/10
   Optimization Opportunities:

[Issue] - [Impact] tokens - [Solution]
[Issue] - [Impact] tokens - [Solution]

Immediate Actions:

[Action 1]: [Expected token savings]
[Action 2]: [Expected token savings]

Long-term Improvements:

[Strategy 1]
[Strategy 2]


4. **Auto-Optimization Options** (if requested):
- Suggest `/compact` to compress conversation
- Recommend `/clear` with context preservation strategy
- Propose session transition with key context extraction
- Advise model switching for cost efficiency

Arguments: $ARGUMENTS (optional: "auto" for automatic recommendations, "execute" for guided optimization)
