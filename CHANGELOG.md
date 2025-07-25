# Changelog

## [2.0.2] - 2025-01-25

### Added
- WSL (Windows Subsystem for Linux) configuration support
- New WSL Configuration window in Unity Editor (Window > Unity MCP > WSL Configuration)
- Automatic Windows host IP detection for WSL environments
- Support for `claude mcp add` and `claude mcp add-json` commands with WSL-specific options
- Environment variable support:
  - `UNITY_HOST` - Override Unity host IP address
  - `WSL_INTEROP_IP` - WSL-specific host IP override
- `WslConfigHelper` class for generating WSL-compatible configurations
- Connection test utility (`test_connection.py`) for troubleshooting

### Changed
- Updated Python server config to support dynamic host IP resolution
- Enhanced README with WSL troubleshooting section

### Fixed
- WSL to Windows connectivity issues when running MCP server in WSL

## [2.0.1] - Previous release
- Previous release notes...