#!/usr/bin/env bash
input=$(cat)
read_path=$(echo "$input" | jq -r '.tool_input.file_path // ""')
if [[ "$read_path" == *".env"* ]]; then
  echo "You cannot read the .env file" >&2
  exit 2
fi
exit 0
