#!/usr/bin/env python3
"""
split_s390x_tests.py

Parses a C# file containing s390x instruction tests decorated with
[S390xTest(number, mnemonic, description)] and compiles each one into
its own DLL.

Each generated program contains:
  - The S390xTestAttribute definition
  - The Consume<T> helper (defeats JIT constant-folding)
  - A class S390xInstructionTest with:
      - The single extracted test method (always named s390xHw)
      - A Main() that calls s390xHw(), prints PASS/FAIL, returns exit code

Usage:
    python3 split_s390x_tests.py <input.cs> [options]

Options:
    --outdir DIR    Where to put the final DLLs  (default: ./s390x_dlls)
    --tfm    TFM    Target framework moniker      (default: net9.0)
    --keep-src      Keep the temporary build trees under <outdir>/_src/
    --run           Run each DLL with corerun after building and report results
    --corerun PATH  Path to corerun (overrides $CORE_ROOT/corerun)
"""

import re
import sys
import os
import shutil
import argparse
import subprocess
import tempfile
import textwrap


# ---------------------------------------------------------------------------
# Parsing
# ---------------------------------------------------------------------------

# Matches:  [S390xTest(42, "mnemonic", "description")]
_ATTR_RE = re.compile(
    r'\[S390xTest\(\s*(\d+)\s*,\s*"([^"]+)"\s*,\s*"([^"]+)"\s*\)\]'
)

# Matches the start of a public static int Test_xxx() method
_METHOD_START_RE = re.compile(
    r'^\s*public\s+static\s+int\s+(Test_\w+)\s*\(\s*\)'
)


def _extract_braced_body(source: str, open_brace_pos: int) -> str:
    """
    Given the position of the opening '{' of a method, return the full
    text from '{' up to and including the matching '}'.
    """
    depth = 0
    i = open_brace_pos
    while i < len(source):
        if source[i] == '{':
            depth += 1
        elif source[i] == '}':
            depth -= 1
            if depth == 0:
                return source[open_brace_pos: i + 1]
        i += 1
    raise ValueError(f"Unmatched '{{' at position {open_brace_pos}")


def parse_tests(source: str) -> list[dict]:
    """
    Find every [S390xTest(...)] attribute immediately followed by a
    public static int Test_xxx() method and return a list of dicts:
        number, mnemonic, desc, method_name, body (the {...} block)
    """
    tests = []
    lines = source.splitlines(keepends=True)

    # Work line-by-line so we can correlate attribute line → method line
    i = 0
    while i < len(lines):
        attr_m = _ATTR_RE.search(lines[i])
        if attr_m:
            number   = int(attr_m.group(1))
            mnemonic = attr_m.group(2)
            desc     = attr_m.group(3)

            # Scan forward for the method signature (skip blank/comment lines)
            j = i + 1
            method_m = None
            while j < len(lines) and j <= i + 4:
                method_m = _METHOD_START_RE.match(lines[j])
                if method_m:
                    break
                j += 1

            if not method_m:
                print(f"  WARNING: [S390xTest({number},...)] on line {i+1} "
                      f"not followed by a Test_ method — skipped.",
                      file=sys.stderr)
                i += 1
                continue

            method_name = method_m.group(1)

            # Find the opening brace of the method body in the full source
            # (join lines up to this point to get the absolute offset)
            prefix_len  = sum(len(l) for l in lines[:j])
            rest        = source[prefix_len:]
            brace_rel   = rest.index('{')
            body        = _extract_braced_body(rest, brace_rel)

            tests.append({
                "number":      number,
                "mnemonic":    mnemonic,
                "desc":        desc,
                "method_name": method_name,
                "body":        body,   # includes the outer { }
            })

            # Advance past the method
            body_lines = body.count('\n')
            i = j + body_lines + 1
            continue

        i += 1

    if not tests:
        raise ValueError(
            "No [S390xTest(...)] attributes found.\n"
            "Expected: [S390xTest(N, \"mnemonic\", \"description\")]"
        )

    # Sort by test number so output is deterministic
    tests.sort(key=lambda t: t["number"])
    return tests


# ---------------------------------------------------------------------------
# Code generation
# ---------------------------------------------------------------------------

CSPROJ_TEMPLATE = """\
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>{tfm}</TargetFramework>
    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
    <Nullable>enable</Nullable>
    <ImplicitUsings>disable</ImplicitUsings>
    <AssemblyName>{assembly}</AssemblyName>
  </PropertyGroup>
</Project>
"""

CS_TEMPLATE = """\
// Auto-generated — s390x instruction test: {mnemonic}
// {desc}  (test #{number})

public class S390xInstructionTest
{{
    public static int Main()
    {{
        int result = s390xHw();
        string status = result == 0 ? "PASS" : "FAIL";
        System.Console.WriteLine($"{{status}}: Test {number} ({mnemonic}) - {desc}");
        return result;
    }}

    public static int s390xHw()
    {body}
}}
"""


def assembly_name(test: dict) -> str:
    safe = re.sub(r'[^A-Za-z0-9_]', '_', test["mnemonic"])
    return f"Test{test['number']:03d}_{safe}"


def generate_cs(test: dict) -> str:
    return CS_TEMPLATE.format(
        number   = test["number"],
        mnemonic = test["mnemonic"],
        desc     = test["desc"],
        body     = test["body"],
    )


# ---------------------------------------------------------------------------
# Build + run helpers
# ---------------------------------------------------------------------------

def build_dll(test: dict, outdir: str, tfm: str, keep_src: bool) -> tuple[bool, str]:
    """
    Write a temp project, run `dotnet build`, copy the DLL to outdir.
    Returns (success, dll_path_or_error_message).
    """
    asmname  = assembly_name(test)
    dll_dest = os.path.join(outdir, f"{asmname}.dll")
    build_dir = tempfile.mkdtemp(prefix=f"s390x_{asmname}_")

    try:
        cs_path     = os.path.join(build_dir, f"{asmname}.cs")
        csproj_path = os.path.join(build_dir, f"{asmname}.csproj")

        with open(cs_path, "w") as f:
            f.write(generate_cs(test))
        with open(csproj_path, "w") as f:
            f.write(CSPROJ_TEMPLATE.format(tfm=tfm, assembly=asmname))

        result = subprocess.run(
            ["dotnet", "build", "--nologo", "-v", "quiet", "-c", "Release"],
            cwd=build_dir,
            capture_output=True,
            text=True,
        )
        if result.returncode != 0:
            msg = (result.stdout + result.stderr).strip()
            return False, msg[-800:] if len(msg) > 800 else msg

        # Locate produced DLL
        dll_src = os.path.join(build_dir, "bin", "Release", tfm, f"{asmname}.dll")
        if not os.path.exists(dll_src):
            for root, _, files in os.walk(os.path.join(build_dir, "bin")):
                for fn in files:
                    if fn == f"{asmname}.dll":
                        dll_src = os.path.join(root, fn)
                        break

        shutil.copy2(dll_src, dll_dest)

        if keep_src:
            keep_dir = os.path.join(outdir, "_src", asmname)
            shutil.copytree(build_dir, keep_dir, dirs_exist_ok=True)

        return True, dll_dest

    finally:
        if not keep_src:
            shutil.rmtree(build_dir, ignore_errors=True)


def run_dll(dll_path: str, corerun: str | None) -> tuple[bool, str]:
    if corerun:
        cmd = [corerun, dll_path]
        cwd = os.path.dirname(corerun)   # run from CORE_ROOT so native libs are found
    else:
        cmd = ["dotnet", dll_path]
        cwd = None

    result = subprocess.run(cmd, capture_output=True, text=True, cwd=cwd)
    output = (result.stdout + result.stderr).strip()
    return result.returncode == 0, output


# ---------------------------------------------------------------------------
# Main
# ---------------------------------------------------------------------------

def main():
    parser = argparse.ArgumentParser(
        description="Split s390x C# test file into individual DLLs."
    )
    parser.add_argument("input",            help="Path to the combined C# source file")
    parser.add_argument("--outdir",         default="s390x_dlls",
                        help="Output directory for DLLs (default: ./s390x_dlls)")
    parser.add_argument("--tfm",            default="net9.0",
                        help="Target framework moniker (default: net9.0)")
    parser.add_argument("--keep-src",       action="store_true",
                        help="Keep generated source under <outdir>/_src/")
    parser.add_argument("--run",            action="store_true",
                        help="Run each DLL after building and report PASS/FAIL")
    parser.add_argument("--corerun",        default=None,
                        help="Path to corerun binary (defaults to $CORE_ROOT/corerun)")
    args = parser.parse_args()

    # Resolve corerun
    corerun = args.corerun
    if args.run and corerun is None:
        core_root = os.environ.get("CORE_ROOT")
        if core_root:
            corerun = os.path.join(core_root, "corerun")
            if not os.access(corerun, os.X_OK):
                print(f"WARNING: corerun not executable at {corerun}, falling back to dotnet",
                      file=sys.stderr)
                corerun = None

    with open(args.input) as f:
        source = f.read()

    try:
        tests = parse_tests(source)
    except ValueError as e:
        print(f"Error: {e}", file=sys.stderr)
        sys.exit(1)

    os.makedirs(args.outdir, exist_ok=True)

    runner_desc = corerun if corerun else "dotnet"
    print(f"Found {len(tests)} test(s) in '{args.input}'")
    print(f"Output directory : {os.path.abspath(args.outdir)}")
    print(f"Target framework : {args.tfm}")
    if args.run:
        print(f"Runner           : {runner_desc}")
    print()

    passed = failed = build_failed = 0

    for test in tests:
        asmname = assembly_name(test)
        label   = f"Test {test['number']:3d}: {test['mnemonic']:<12}  {test['desc'][:50]}"

        ok, info = build_dll(test, args.outdir, args.tfm, args.keep_src)
        if not ok:
            print(f"  BUILD FAIL  {label}")
            # Indent each error line for readability
            for line in info.splitlines():
                print(f"              {line}")
            build_failed += 1
            failed += 1
            continue

        if args.run:
            ok, output = run_dll(info, corerun)
            status = "PASS" if ok else "FAIL"
            print(f"  {status}        {label}")
            if not ok:
                for line in output.splitlines():
                    print(f"              {line}")
                failed += 1
            else:
                passed += 1
        else:
            print(f"  BUILT       {label}  ->  {asmname}.dll")

    total = len(tests)
    print()
    if args.run:
        pct = f"{passed/total*100:.1f}" if total else "0.0"
        print(f"Results : {passed}/{total} passed  ({pct}%),  {failed} failed"
              + (f"  ({build_failed} build errors)" if build_failed else ""))
    else:
        print(f"Built {total - build_failed}/{total} DLLs"
              + (f"  ({build_failed} build errors)" if build_failed else ""))

    print("Done.")
    sys.exit(1 if failed else 0)


if __name__ == "__main__":
    main()
