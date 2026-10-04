"""Archive recoverable master-workflow SQLite history and record expired gaps.

No artifact is executable and no imported history is promoted to trusted/native
acceptance. Downloaded files are kept with SHA-256, run SHA and artifact attribution.
"""
import argparse
import hashlib
import json
from pathlib import Path
import subprocess
import zipfile


def pages(endpoint):
    return json.loads(subprocess.check_output(['gh','api','--paginate','--slurp',endpoint],text=True))


def archive(args):
    output=Path(args.output); output.mkdir(parents=True,exist_ok=True)
    manifest={'repository':args.repository_name,'artifacts':[],'gaps':[],'history_complete':False}
    count=0
    for workflow in ('test262-catalog.yml','test262-native-port.yml'):
        runs=[r for page in pages(f'repos/{args.repository_name}/actions/workflows/{workflow}/runs?branch=master&status=completed&per_page=100') for r in page['workflow_runs']]
        for run in runs:
            if run['head_repository']['full_name']!=args.repository_name or run['head_branch']!='master':
                continue
            artifacts=[a for page in pages(f'repos/{args.repository_name}/actions/runs/{run["id"]}/artifacts?per_page=100') for a in page['artifacts']]
            selected=[a for a in artifacts if a['name'] in ('test262-catalog','test262-native-reconciled-state','test262-native-state') or a['name'].startswith('test262-catalog-shard-')]
            if not selected:
                manifest['gaps'].append({'workflow':workflow,'run_id':run['id'],'reason':'no catalogue artifact'})
            for artifact in selected:
                record={'workflow':workflow,'run_id':run['id'],'source_revision':run['head_sha'],
                        'artifact_id':artifact['id'],'artifact_name':artifact['name'],'created_at':artifact['created_at']}
                if artifact['expired']:
                    manifest['gaps'].append(dict(record,reason='expired'));continue
                directory=output/str(artifact['id']);directory.mkdir(exist_ok=True)
                path=directory/'source.zip'
                if not path.is_file():
                    with path.open('wb') as stream:
                        subprocess.run(['gh','api',f'repos/{args.repository_name}/actions/artifacts/{artifact["id"]}/zip'],stdout=stream,check=True)
                record['archive_sha256']=hashlib.sha256(path.read_bytes()).hexdigest()
                databases=[]
                with zipfile.ZipFile(path) as archive:
                    for member in archive.infolist():
                        # Never extract executable files, traversal paths, symlinks or an arbitrary archive tree.
                        if member.filename not in ('catalog.sqlite','native.sqlite'):
                            continue
                        if member.file_size>512*1024*1024:
                            raise ValueError('Oversized catalogue archive member')
                        data=archive.read(member)
                        target=directory/member.filename; target.write_bytes(data)
                        databases.append({'path':str(target),'sha256':hashlib.sha256(data).hexdigest(),
                                          'source_uri':f'github-actions:{args.repository_name}/{workflow}/{run["id"]}/{artifact["id"]}'})
                manifest['artifacts'].append(dict(record,databases=databases));count+=1
                # Persist the recovery inventory after each artifact, including interrupted traversals.
                (output/'history-manifest.json').write_text(json.dumps(manifest,indent=2)+'\n')
    manifest['history_complete']=not manifest['gaps']
    (output/'history-manifest.json').write_text(json.dumps(manifest,indent=2)+'\n')
    print(json.dumps({'recoverable_archives':count,'history_gaps':len(manifest['gaps']),'manifest':str(output/'history-manifest.json')}))


if __name__=='__main__':
    parser=argparse.ArgumentParser();parser.add_argument('--repository-name',default='tomacox74/js2il');parser.add_argument('--output',required=True)
    archive(parser.parse_args())
