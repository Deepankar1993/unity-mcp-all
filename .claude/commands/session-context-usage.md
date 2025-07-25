# Session Context Usage Analyzer

Analyze and report the current Claude Code context usage for this conversation.

## Instructions:

1. **Analyze Active Context Sources**:
    - CLAUDE.md file (if present) - count characters and estimate tokens
    - Current session file content and size
    - Git status and recent commits in working directory
    - Environment information (OS, working directory, date)
    - Files read during this conversation session
    - Tool usage (MCP, git, file operations)

2. **Calculate Token Usage Breakdown**:
   For each context source, provide:
    - Character count
    - Estimated token count (chars ÷ 4 for rough estimate)
    - Context contribution percentage

3. **Identify Major Context Consumers**:
    - CLAUDE.md instructions: [character count] → ~[tokens] tokens
    - Session history and tracking: ~[tokens] tokens
    - Git operations and status: ~[tokens] tokens
    - File reads during session: [list files] → ~[tokens] tokens
    - Tool calls and responses: ~[tokens] tokens
    - Conversation history: ~[tokens] tokens

4. **Provide Context Usage Report**:
   ● Context Usage Analysis for Current Session:
   Active Context Sources:

CLAUDE.md - [X,XXX] characters of project instructions
Session file - [session name and size]
Git status - Shows [X] modified files
Environment info - Working directory, OS, date

Token Usage Breakdown (approximate):

Initial context from CLAUDE.md: ~[X,XXX] tokens
Session history recap: ~[XXX] tokens
Git status and commits: ~[XXX] tokens
Environment details: ~[XX] tokens
File reads during session:
[List each file with estimated tokens]
Tool calls and responses: ~[X,XXX] tokens
Conversation length: ~[X,XXX] tokens

Estimated Total Context: ~[XX,XXX] tokens used in this conversation
Context Efficiency:

Largest consumers: [top 3 sources]
Optimization suggestions: [recommendations]
Model recommendation: [Sonnet/Opus] based on complexity


5. **Provide Optimization Recommendations**:
- If context > 150,000 tokens: Recommend `/project:session-transition`
- If context > 100,000 tokens: Suggest monitoring and potential `/compact`
- If CLAUDE.md is large: Suggest optimization
- If many files read: Recommend focused file access
- If session file is large: Suggest session cycling

6. **Model Usage Recommendations**:
- Current context size appropriate for current model?
- Should switch to Opus for complex analysis?
- Cost optimization suggestions based on context pattern

Arguments: $ARGUMENTS (optional: "detailed" for verbose analysis, "optimize" for recommendations only)

This provides real-time context awareness for session management and cost optimization.
