# Session Context Monitor

Continuously monitor context usage during development session and provide proactive warnings.

## Instructions:

1. **Real-Time Context Tracking**:
    - Monitor conversation growth rate
    - Track file access patterns
    - Watch tool usage accumulation
    - Assess session complexity evolution

2. **Proactive Warnings**:
    - **75% context**: "Context getting full - consider optimization"
    - **85% context**: "High context usage - prepare for transition"
    - **95% context**: "Critical - immediate action needed"

3. **Usage Pattern Analysis**:
   ● Context Monitor Status:
   Current Session: [session name]
   Context Usage: [XX,XXX] / 200,000 tokens ([XX]%)
   Growth Rate: ~[XXX] tokens per update
   Session Metrics:

Duration: [X] hours [X] minutes
Updates: [XX] entries
Files accessed: [XX] files
Tool calls: [XX] operations

Projection:

Estimated capacity remaining: ~[XX] updates
Suggested transition point: [time/updates]

Health Status: [Green/Yellow/Red]

4. **Smart Recommendations**:
- Based on current trajectory, when to transition
- Model switching suggestions for efficiency
- Context cleanup opportunities
- Session management best practices

Arguments: $ARGUMENTS (optional: "summary" for brief status, "detailed" for full analysis)
