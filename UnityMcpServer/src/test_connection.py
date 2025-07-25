#!/usr/bin/env python3
"""Test connection to Unity Editor from WSL."""

import socket
import json
import sys
from config import config

def test_unity_connection():
    """Test TCP connection to Unity Editor."""
    print(f"Testing connection to Unity at {config.unity_host}:{config.unity_port}")
    
    try:
        # Create socket
        sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        sock.settimeout(5.0)
        
        # Try to connect
        result = sock.connect_ex((config.unity_host, config.unity_port))
        
        if result == 0:
            print("✅ Successfully connected to Unity Editor!")
            
            # Try sending a simple command
            test_command = {
                "tool": "read_console",
                "parameters": {"action": "read"},
                "id": "test-connection"
            }
            
            message = json.dumps(test_command) + "\n"
            sock.sendall(message.encode('utf-8'))
            
            # Try to receive response
            sock.settimeout(2.0)
            try:
                response = sock.recv(4096).decode('utf-8')
                print(f"✅ Received response from Unity: {response[:100]}...")
            except socket.timeout:
                print("⚠️  Connected but no response received (Unity Bridge might not be initialized)")
            
            sock.close()
            return True
        else:
            print(f"❌ Failed to connect: Error code {result}")
            print("\nPossible solutions:")
            print("1. Ensure Unity Editor is running with the Unity MCP Bridge package")
            print("2. Check Windows Firewall settings for port 6400")
            print("3. Try setting UNITY_HOST environment variable:")
            print(f"   export UNITY_HOST=<your-windows-ip>")
            print("4. Current resolved IP:", config.unity_host)
            sock.close()
            return False
            
    except Exception as e:
        print(f"❌ Error during connection test: {e}")
        return False

if __name__ == "__main__":
    sys.exit(0 if test_unity_connection() else 1)