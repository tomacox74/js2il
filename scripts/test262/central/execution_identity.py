"""Exact execution identity shared by trusted preparation and independent clients."""
import json
from pathlib import Path
import subprocess
import sys
from .client import canonical, fixture_environment
from .inventory import REPO
sys.path.insert(0,str(REPO/'scripts/test262'))
import catalog


def execution_identity(args, corpus):
    root=Path(args.root).resolve()
    pin=json.loads((REPO/'tests/test262/test262.pin.json').read_text())['upstream']
    entry = Path(args.jroc if args.kind=='mvp-composite' else args.host).resolve()
    capabilities = {} if args.kind=='mvp-composite' else json.loads(subprocess.check_output(['dotnet',str(entry),'--capabilities'],text=True,env=fixture_environment()))
    env = catalog.environment(entry)
    # Include all native testing implementation as well as tooling; SHA is diagnostic, binary identities authoritative.
    tooling = catalog.hash_files(REPO,['scripts/test262/**/*.py','scripts/test262/**/*.js','scripts/test262/NativeScreeningHost/**/*.cs','tests/Jroc.Testing/**/*.cs','tests/test262/test262.pin.json'])
    doc = {'runner':args.kind,'upstream':pin,'inventory':corpus,'binaries':catalog.hash_files(entry.parent,['*.dll','*.deps.json','*.runtimeconfig.json']),
           'harness':catalog.hash_files(root,['harness/**/*']),'tooling':tooling,'capabilities':capabilities,
           'environment_identity':env['identity'],'timeouts':{'runtime':args.runtime_timeout,'compile':args.compile_timeout,'cap_ms':args.cap_ms}}
    if getattr(args, 'compilation_coverage', False):
        doc['compiler_options'] = {'compilation_coverage': True, 'coverage_measurement': 'statement-source-sites-v1'}
    return doc
