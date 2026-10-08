"""Preserve a reviewed manifest of required cutover evidence without importing or changing production authority."""
import hashlib
import json
import os
from pathlib import Path
import sqlite3
import subprocess
import tempfile
import zipfile
from datetime import datetime, timezone

REPOSITORY = "tomacox74/js2il"
DEFAULT_MANIFEST=Path(__file__).with_name('cutover-evidence.json')


def load_manifest(path):
    import re
    raw=Path(path).read_bytes()
    config=json.loads(raw)
    if (config.get('schema_version')!=1 or config.get('repository')!=REPOSITORY
        or config.get('history_complete') is not False or not config.get('scope')):
        raise ValueError('Invalid evidence manifest scope or repository')
    artifacts=config['required_artifacts'];sources=config['expected_sources']
    evidence={row['artifact_id']:row['sha256'] for row in artifacts}
    expected={row['artifact_id']:row['observations'] for row in sources}
    if (not evidence or len(evidence)!=len(artifacts) or len(expected)!=len(sources)
        or any(type(i) is not int or i<=0 or not re.fullmatch(r'[0-9a-f]{64}',h) for i,h in evidence.items())
        or not re.fullmatch(r'[0-9a-f]{64}',config['history_manifest_sha256'])
        or any(type(i) is not int or i<=0 or type(c) is not int or c<0 for i,c in expected.items())
        or any(row.get('database') not in ('native.sqlite','catalog.sqlite') for row in sources)):
        raise ValueError('Duplicate or invalid evidence pins/coverage')
    imports=config['import_report_ids'];restores=config['restore_report_ids']
    validators=[config['history_archive_id'],*imports,*restores]
    if (not imports or not restores or not expected or len(set(validators))!=len(validators)
        or not set(validators)<=set(evidence)):
        raise ValueError('Required validation artifacts are missing or ambiguous')
    return config,evidence,expected,hashlib.sha256(raw).hexdigest()


def digest(stream):
    h=hashlib.sha256()
    for chunk in iter(lambda:stream.read(1024*1024),b""):
        h.update(chunk)
    return h.hexdigest()


def member(z, basename):
    matches=[n for n in z.namelist() if Path(n).name==basename]
    if len(matches)!=1:
        raise ValueError("Missing or ambiguous evidence member")
    return matches[0]


def inspect_outbox(z, artifact_id, count, directory):
    name=member(z,str(artifact_id)+"-outbox.sqlite")
    target=Path(directory)/Path(name).name
    # Preserve WAL/SHM alongside the DB; never deserialize arbitrary archive paths.
    for suffix in ("","-wal","-shm"):
        if name+suffix in z.namelist():
            with z.open(name+suffix) as source,Path(str(target)+suffix).open("wb") as dest:
                while chunk:=source.read(1024*1024):
                    dest.write(chunk)
    with sqlite3.connect(target.resolve().as_uri()+"?mode=ro",uri=True) as db:
        if db.execute("PRAGMA integrity_check").fetchone()[0]!="ok":
            raise ValueError("Outbox integrity failed")
        scope=json.loads(db.execute("SELECT identity FROM scope WHERE singleton=1").fetchone()[0])
        if scope[0]!="faf01df8-aa65-4375-a708-6c1e555f957b":
            raise ValueError("Outbox repository mismatch")
        observed=0
        for payload,expected,receipt in db.execute("SELECT payload,digest,receipt FROM messages"):
            rows=json.loads(payload)
            canonical=json.dumps(rows,sort_keys=True,separators=(",",":"),ensure_ascii=False)
            if hashlib.sha256(canonical.encode()).hexdigest()!=expected or not receipt:
                raise ValueError("Corrupt or unacknowledged outbox message")
            json.loads(receipt)
            observed+=len(rows)
        if observed!=count or db.execute("SELECT count(*) FROM completions WHERE receipt IS NULL").fetchone()[0]:
            raise ValueError("Outbox coverage/completion mismatch")
    return {"messages_observations":observed,"pending":0,"integrity":"ok"}


def main(argv=None):
    import argparse
    import boto3
    from .retained_objects import destination, verify_bucket, verify_object
    parser=argparse.ArgumentParser()
    parser.add_argument('--manifest',type=Path,default=DEFAULT_MANIFEST)
    args=parser.parse_args(argv)
    # The workflow accepts reviewed, committed repository manifests only.
    path=args.manifest.resolve()
    if not path.is_relative_to(Path(__file__).resolve().parents[3]):
        raise ValueError('Evidence manifest must be inside the reviewed repository')
    config,evidence,expected,config_sha=load_manifest(path)
    databases={row['artifact_id']:row['database'] for row in config['expected_sources']}
    output=Path(os.environ["EVIDENCE_OUTPUT"]);output.mkdir(parents=True,exist_ok=True)
    report={"history_complete":False,"production_changed":False,"verified_snapshots":[],
            "preserved_objects":[],"complete":False,"stage":"download"}
    s3=boto3.client("s3")
    bucket,base=destination(os.environ['TEST262_BACKUP_S3_URI'])
    kms=os.environ['TEST262_BACKUP_KMS_KEY']
    prefix='/'.join(filter(None,(base,'cutover-evidence',os.environ['GITHUB_RUN_ID']+'-'+os.environ['GITHUB_RUN_ATTEMPT'])))
    report.update(scope=config['scope'],evidence_manifest_sha256=config_sha,
                  required_artifact_ids=list(evidence),expected_source_count=len(expected))
    try:
        verify_bucket(s3,bucket)
        # Keep only one downloaded large archive at a time.
        with tempfile.TemporaryDirectory() as temporary:
            archive=Path(temporary)/"artifact.zip"
            manifest=None
            source_hashes={}
            for artifact_id in [config['history_archive_id'],*(i for i in evidence if i!=config['history_archive_id'])]:
                pinned=evidence[artifact_id]
                metadata=json.loads(subprocess.check_output(["gh","api",f"repos/{REPOSITORY}/actions/artifacts/{artifact_id}"],text=True))
                if metadata.get("expired") or metadata.get("digest")!="sha256:"+pinned:
                    raise ValueError("Artifact metadata changed")
                with archive.open("wb") as dest:
                    subprocess.run(["gh","api",f"repos/{REPOSITORY}/actions/artifacts/{artifact_id}/zip"],stdout=dest,stderr=subprocess.DEVNULL,check=True)
                with archive.open("rb") as stream:
                    if digest(stream)!=pinned:
                        raise ValueError("Artifact ZIP hash mismatch")
                with zipfile.ZipFile(archive) as z:
                    if artifact_id==config['history_archive_id']:
                        raw=z.read(member(z,"history-manifest.json"))
                        if hashlib.sha256(raw).hexdigest()!=config['history_manifest_sha256']:
                            raise ValueError("Manifest hash mismatch")
                        manifest=json.loads(raw)
                        if manifest["repository"]!=REPOSITORY:
                            raise ValueError("Manifest repository mismatch")
                        for source in manifest["artifacts"]:
                            aid=source["artifact_id"]
                            if aid not in expected: continue
                            for db in source["databases"]:
                                if Path(db["path"]).name!=databases[aid]: continue
                                with z.open(next(n for n in z.namelist() if n.endswith(str(aid)+"/"+databases[aid]))) as stream:
                                    actual=digest(stream)
                                with z.open(next(n for n in z.namelist() if n.endswith(str(aid)+"/source.zip"))) as stream:
                                    zip_hash=digest(stream)
                                if actual!=db["sha256"] or zip_hash!=source["archive_sha256"]:
                                    raise ValueError("Original source hash mismatch")
                                source_hashes[aid]=(actual,zip_hash,db["source_uri"])
                        if set(source_hashes)!=set(expected):
                            raise ValueError("Final sources missing from original archive")
                        report["missing_artifact_gaps"]=len(manifest["gaps"])
                    elif artifact_id in config['import_report_ids']:
                        imported=json.loads(z.read(member(z,"import-report.json")))
                        if not imported.get("complete") or imported.get("history_complete") is not False or imported["manifest_sha256"]!=config['history_manifest_sha256']:
                            raise ValueError("Import report not verified/attributed")
                        for snapshot in imported["snapshots"]:
                            aid=snapshot["artifact_id"];count=expected[aid]
                            sha,zip_sha,source_uri=source_hashes[aid]
                            if (snapshot["state"]!="verified" or snapshot["verified_observation_mappings"]!=count
                                or snapshot["result"]["observations"]!=count or snapshot["sha256"]!=sha
                                or snapshot["archive_sha256"]!=zip_sha or snapshot["source_uri"]!=source_uri):
                                raise ValueError("Snapshot report parity/attribution mismatch")
                            with tempfile.TemporaryDirectory() as spool:
                                checked=inspect_outbox(z,aid,count,spool)
                            report["verified_snapshots"].append(dict(artifact_id=aid,observations=count,
                                sqlite_sha256=sha,source_zip_sha256=zip_sha,source_uri=source_uri,outbox=checked))
                    elif artifact_id in config['restore_report_ids']:
                        restored=json.loads(z.read(member(z,"restore-report.json")))
                        if not restored["restore_verified"] or not restored["independent_retention_verified"]:
                            raise ValueError("Fresh backup restore report failed")
                        report.setdefault("backup_restores",[]).append(dict(artifact_id=artifact_id,report=restored))
                report["stage"]="encrypted-preservation"
                key=prefix+"/"+str(artifact_id)+".zip"
                s3.upload_file(str(archive),bucket,key,ExtraArgs={"ServerSideEncryption":"aws:kms","SSEKMSKeyId":kms})
                proof=verify_object(s3,bucket,key,kms)
                if proof['sha256']!=pinned:
                    raise ValueError('Preserved exact-version bytes differ')
                report['preserved_objects'].append(dict(proof,artifact_id=artifact_id,key=key))
                archive.unlink()
                print("Verified and preserved artifact",artifact_id,flush=True)
            if {s["artifact_id"] for s in report["verified_snapshots"]}!=set(expected):
                raise ValueError("Configured snapshot verification incomplete")
        if {o['artifact_id'] for o in report['preserved_objects']}!=set(evidence):
            raise ValueError('Configured required artifact set preservation incomplete')
        report.update(complete=True,stage="complete",final_legacy_checkpoint_disposition="retained-only, not imported")
        report['checked_at']=datetime.now(timezone.utc).isoformat()
        path=output/'evidence-report.json';path.write_text(json.dumps(report,indent=2)+'\n')
        key=prefix+'/evidence-report.json'
        s3.upload_file(str(path),bucket,key,ExtraArgs={'ServerSideEncryption':'aws:kms','SSEKMSKeyId':kms})
        proof=verify_object(s3,bucket,key,kms)
        with path.open('rb') as stream:
            if proof['sha256']!=digest(stream):
                raise ValueError('Preserved evidence report bytes differ')
        report['receipt']=dict(proof,key=key)
    except Exception as error:
        report["complete"]=False
        report["stage"]="failed"
        report["error_class"]=type(error).__name__
        report["aws_error_code"]=getattr(error,"response",{}).get("Error",{}).get("Code")
        raise
    finally:
        report["checked_at"]=datetime.now(timezone.utc).isoformat()
        path=output/"evidence-report.json";path.write_text(json.dumps(report,indent=2)+"\n")
        print(json.dumps({"stage":report["stage"],"complete":report["complete"],"verified_snapshots":len(report["verified_snapshots"])}),flush=True)


if __name__=="__main__":
    try: main()
    except Exception as error:
        print("Evidence verification failed ("+type(error).__name__+").",flush=True)
        raise SystemExit(1)
