"""Read GitHub health/receipt metadata and append a sanitized operational sample.

No fixture execution, queue/target changes, publication, S3 access or notification
delivery. A successful Actions job alone never verifies a retained backup.
"""
import argparse
from datetime import datetime, timezone
import hashlib
import io
import json
import os
from pathlib import Path
import re
import subprocess
import uuid
import zipfile
from .client import Client, canonical
from .diagnostics import report_failure, stage

WORKFLOWS={
    'backup':('test262-central-backup.yml','test262-catalogue-backup','backup-report.json'),
    'restore':('test262-retained-restore.yml','test262-retained-restore','restore-report.json'),
    'linux-smoke':('linux-smoke.yml',None,None),
    'windows-smoke':('windows-smoke.yml',None,None),
    'benchmarkdotnet-suite':('benchmarkdotnet-suite.yml',None,None),
}


def require(condition,message):
    if not condition:
        raise ValueError(message)


def github(path,binary=False):
    # The path is constructed from reviewed filenames and validated IDs, never
    # from artifact content. gh handles GitHub's authenticated download redirect.
    raw=subprocess.check_output(['gh','api',path],timeout=90)
    require(len(raw)<=2_000_000,'GitHub response exceeds monitor bound')
    return raw if binary else json.loads(raw)


def receipt_summary(metadata,raw,name,kind):
    require(not metadata.get('expired') and metadata['size_in_bytes']<=2_000_000,'Unavailable/oversize receipt artifact')
    digest=hashlib.sha256(raw).hexdigest()
    require(metadata.get('digest')=='sha256:'+digest,'Receipt ZIP digest differs from GitHub metadata')
    with zipfile.ZipFile(io.BytesIO(raw)) as archive:
        files=[f for f in archive.infolist() if Path(f.filename).name==name and not f.is_dir()]
        require(len(files)==1 and files[0].file_size<=262144,'Missing/ambiguous/oversize receipt')
        document=json.loads(archive.read(files[0]))
    if kind=='backup':
        verified=(document.get('backup_verified') is True and document.get('contains_performance_data') is False
                  and document.get('bucket_retention_verified') is True and document.get('stage')=='complete')
    else:
        verified=(document.get('restore_verified') is True and document.get('independent_retention_verified') is True
                  and document.get('production_cutover') is False and document.get('writer_bindings_disabled') is True
                  and document.get('restored_authority')=='shadow' and document.get('stage')=='complete')
    objects=document.get('objects',{})
    require(set(objects)=={'catalogue.dump','manifest.json'},'Receipt must verify both retained objects')
    retained=all(o.get('retention_verified') is True and o.get('version_id') and
                 re.fullmatch('[0-9a-f]{64}',o.get('sha256','')) for o in objects.values())
    deadlines=[datetime.fromisoformat(o['retain_until']) for o in objects.values()]
    require(all(d.tzinfo is not None for d in deadlines),'Retention timestamps need timezones')
    checked=document.get('checked_at')
    require(checked and datetime.fromisoformat(checked).tzinfo is not None,'Receipt check time missing')
    return {'artifact_id':metadata['id'],'artifact_sha256':digest,'receipt_verified':bool(verified),
            'retention_verified':bool(retained),'retention_until':min(deadlines).isoformat(),
            'receipt_checked_at':checked}


def collect_check(repository,kind,fetch=github):
    workflow,prefix,filename=WORKFLOWS[kind]
    base=f'repos/{repository}'
    result={'fetch_state':'unavailable'}
    try:
        runs=fetch(f'{base}/actions/workflows/{workflow}/runs?per_page=20')['workflow_runs']
        runs=[r for r in runs if r.get('head_repository',{}).get('full_name')==repository and
              r.get('event') in ('schedule','workflow_dispatch','release') and
              (kind not in ('backup','restore') or r.get('head_branch')=='master')]
        runs.sort(key=lambda r:r['id'],reverse=True)
        result['fetch_state']='observed'
        if not runs:
            return result
        latest=runs[0]
        result.update(latest_run_id=latest['id'],latest_attempt=latest['run_attempt'],
                      latest_state=latest['status'],latest_conclusion=latest.get('conclusion'),
                      latest_finished_at=latest['updated_at'] if latest['status']=='completed' else None)
        success=next((r for r in runs if r['status']=='completed' and r['conclusion']=='success'),None)
        if success:
            result.update(success_run_id=success['id'],success_attempt=success['run_attempt'],
                          success_finished_at=success['updated_at'],source_revision=success['head_sha'])
        if not prefix or not success:
            return result
        artifacts=fetch(f"{base}/actions/runs/{success['id']}/artifacts?per_page=100")['artifacts']
        expected=f"{prefix}-{success['id']}-{success['run_attempt']}"
        matching=[a for a in artifacts if a['name']==expected and not a.get('expired')]
        require(len(matching)==1,'Missing exact-attempt receipt artifact')
        artifact=matching[0]
        require(artifact['size_in_bytes']<=2_000_000 and artifact['workflow_run']['head_sha']==success['head_sha'] and
                artifact['workflow_run']['id']==success['id'],'Receipt artifact source differs')
        raw=fetch(f"{base}/actions/artifacts/{int(artifact['id'])}/zip",binary=True)
        result.update(receipt_summary(artifact,raw,filename,kind))
    except (ValueError,KeyError,TypeError,subprocess.SubprocessError,zipfile.BadZipFile):
        # Preserve observed workflow state, but never fabricate successful proof
        # from an absent/unreadable/expired/digest-mismatched receipt.
        result['receipt_verified']=None
    return result


def collect(repository,fetch=github):
    require(re.fullmatch(r'[A-Za-z0-9_.-]+/[A-Za-z0-9_.-]+',repository),'Invalid repository name')
    master=fetch(f'repos/{repository}/branches/master')['commit']
    revision=master['sha']
    require(re.fullmatch('[0-9a-f]{40}([0-9a-f]{24})?',revision),'Invalid master revision')
    return {'master_revision':revision,'master_committed_at':master['commit']['committer']['date'],
            'checks':{kind:collect_check(repository,kind,fetch) for kind in WORKFLOWS}}


def main(argv=None):
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--repository-name',default=os.getenv('GITHUB_REPOSITORY','tomacox74/js2il'))
    parser.add_argument('--output',required=True)
    parser.add_argument('--dry-run',action='store_true',help='Collect metadata without any database connection/write')
    args=parser.parse_args(argv)
    with stage('monitor.collect'):
        document=collect(args.repository_name)
    result={'schema_version':1,'notifications_enabled':False,'document':document,'dry_run':args.dry_run}
    if not args.dry_run:
        with stage('monitor.record'):
            client=Client()
            try:
                require(client.contract['repository_id']==os.environ['TEST262_REPOSITORY_ID'] and
                        client.contract['producer_id']==os.environ['TEST262_PRODUCER_ID'], 'Monitor identity mismatch')
                result['receipt']=client.call('monitor',str(uuid.uuid4()),document)
            finally:
                client.close()
    Path(args.output).parent.mkdir(parents=True,exist_ok=True)
    Path(args.output).write_text(canonical(result)+'\n')
    print(canonical({'recorded':not args.dry_run,'notifications_enabled':False}))


if __name__=='__main__':
    try:
        main()
    except Exception as error:
        report_failure(error)
        raise SystemExit(1)
