"""
Configuration settings for the Unity MCP Server.
This file contains all configurable parameters for the server.
"""

import os
import subprocess
from dataclasses import dataclass

def get_windows_host_ip():
    """Get the Windows host IP from WSL.
    
    Priority order:
    1. WSL_INTEROP_IP environment variable (if set)
    2. Default gateway from ip route (usually works best)
    3. Nameserver from /etc/resolv.conf
    4. Fallback to localhost
    """
    # Try environment variable first
    if os.environ.get('WSL_INTEROP_IP'):
        return os.environ['WSL_INTEROP_IP']
    
    # Try to get default gateway (usually the most reliable)
    try:
        result = subprocess.run(['ip', 'route', 'show', 'default'], 
                               capture_output=True, text=True)
        if result.returncode == 0 and result.stdout:
            # Extract IP from "default via X.X.X.X dev eth0"
            parts = result.stdout.strip().split()
            if len(parts) > 2 and parts[0] == 'default' and parts[1] == 'via':
                return parts[2]
    except:
        pass
    
    # Try to get from /etc/resolv.conf
    try:
        result = subprocess.run(['grep', 'nameserver', '/etc/resolv.conf'], 
                               capture_output=True, text=True)
        if result.returncode == 0 and result.stdout:
            # Extract IP from "nameserver X.X.X.X"
            parts = result.stdout.strip().split()
            if len(parts) > 1:
                return parts[1]
    except:
        pass
    
    # Fallback to localhost
    return "localhost"

@dataclass
class ServerConfig:
    """Main configuration class for the MCP server."""
    
    # Network settings
    unity_host: str = os.environ.get('UNITY_HOST', get_windows_host_ip())
    unity_port: int = 6400
    mcp_port: int = 6500
    
    # Connection settings
    connection_timeout: float = 86400.0  # 24 hours timeout
    buffer_size: int = 16 * 1024 * 1024  # 16MB buffer
    
    # Logging settings
    log_level: str = "INFO"
    log_format: str = "%(asctime)s - %(name)s - %(levelname)s - %(message)s"
    
    # Server settings
    max_retries: int = 3
    retry_delay: float = 1.0

# Create a global config instance
config = ServerConfig() 