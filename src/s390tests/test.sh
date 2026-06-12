#!/bin/bash
# run_s390x_tests.sh
# Runs all .dll files in a directory via $CORE_ROOT/corerun and prints results.
#
# Usage:
#   ./run_s390x_tests.sh [dll_dir]
#
# dll_dir defaults to ./s390x_dlls if not specified.
# CORE_ROOT must be set in the environment.

DLL_DIR="${1:-./s390x_dlls}"
DLL_DIR="$(realpath -m "$DLL_DIR")"

# ---------- sanity checks ----------
if [ -z "$CORE_ROOT" ]; then
    echo "ERROR: \$CORE_ROOT is not set."
    exit 1
fi

CORERUN="$CORE_ROOT/corerun"
if [ ! -x "$CORERUN" ]; then
    echo "ERROR: corerun not found or not executable: $CORERUN"
    exit 1
fi

if [ ! -d "$DLL_DIR" ]; then
    echo "ERROR: DLL directory not found: $DLL_DIR"
    exit 1
fi

DLLS=( "$DLL_DIR"/*.dll )
if [ ${#DLLS[@]} -eq 0 ] || [ ! -f "${DLLS[0]}" ]; then
    echo "ERROR: No .dll files found in $DLL_DIR"
    exit 1
fi

# ---------- run ----------
PASS=()
FAIL=()

echo "======================================================================"
echo "  s390x DLL Test Runner"
echo "  CORE_ROOT : $CORE_ROOT"
echo "  DLL dir   : $DLL_DIR"
echo "  DLLs found: ${#DLLS[@]}"
echo "======================================================================"
echo ""

for dll in "${DLLS[@]}"; do
    name=$(basename "$dll" .dll)

    # Run from CORE_ROOT so native libs (libSystem.Native.so etc.) are found
    output=$(cd "$CORE_ROOT" && "$CORERUN" "$dll" 2>&1)
    exit_code=$?

    if [ $exit_code -eq 0 ]; then
        echo "  PASS  $name"
        PASS+=("$name")
    else
        echo "  FAIL  $name  (exit $exit_code)"
        while IFS= read -r line; do
            echo "        $line"
        done <<< "$output"
        FAIL+=("$name")
    fi
done

# ---------- summary ----------
TOTAL=${#DLLS[@]}
PASS_COUNT=${#PASS[@]}
FAIL_COUNT=${#FAIL[@]}

echo ""
echo "======================================================================"
echo "  Summary"
echo "======================================================================"
echo "  Total  : $TOTAL"
echo "  Passed : $PASS_COUNT"
echo "  Failed : $FAIL_COUNT"

if [ $FAIL_COUNT -gt 0 ]; then
    echo ""
    echo "  Failed tests:"
    for f in "${FAIL[@]}"; do
        echo "      - $f"
    done
fi

echo ""
if [ "$TOTAL" -gt 0 ]; then
    PCT=$(awk "BEGIN { printf \"%.1f\", ($PASS_COUNT/$TOTAL)*100 }")
    echo "  Pass rate : $PCT%  ($PASS_COUNT/$TOTAL)"
fi
echo "======================================================================"

[ $FAIL_COUNT -eq 0 ]
