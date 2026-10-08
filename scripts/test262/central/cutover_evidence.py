"""Preserve fixed cutover evidence without importing or changing production authority."""
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
MANIFEST_SHA = "dd708f3ce7ac05d92bf4b97a4f0680c4a6b7c383dc8ec120198ce22a9febfe1c"
EVIDENCE = {
    11293700607: "ee392023313476cca2291b270038e174892b01e1c35a5b14b696d33651dad059",
    11510693007: "da15aee0a42498eb5b7f439e198991d9ac84e555e1fc15714cc3f5ca81d04268",
    11513473147: "f3caca577d57e6d4ff8d45131e6133ae9ab7dee4feaf01427fb2a560b640f516",
    11515534653: "130857077a60584ecdac2b11c0505a49bc2b6f23765a6459a963752ff3a7fe75",
    11522581310: "c6f23b70175328cabfe0260195739eec631261ce84a1b9209cf7013ee4994631",
    11523270256: "63c3af412e252b597c838bfa98ddb03bfb9b7310906cdfd58c43ea32ce89ede7",
    11523056094: "53fe23bd6e6069ce6228883a6dd26a4f91232423a6794b7c940f96e2008299be",
    11522307991: "78a37b888450ea81bc7cf6c628dc0197bfb1951b279bff014bce0a19d2a00eba",
}
EXPECTED = {11290771863:381,11286853535:381,11285859826:381,11284676689:381,11284636756:381}


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


def main():
    import boto3
    from urllib.parse import urlsplit
    output=Path(os.environ["EVIDENCE_OUTPUT"]);output.mkdir(parents=True,exist_ok=True)
    report={"history_complete":False,"production_changed":False,"verified_snapshots":[],
            "preserved_objects":[],"complete":False,"stage":"download"}
    s3=boto3.client("s3")
    uri=urlsplit(os.environ["TEST262_BACKUP_S3_URI"])
    if uri.scheme!="s3" or not uri.netloc or uri.query or uri.fragment:
        raise ValueError("Invalid S3 destination")
    prefix="/".join(filter(None,(uri.path.strip("/"),"cutover-evidence",os.environ["GITHUB_RUN_ID"]+"-"+os.environ["GITHUB_RUN_ATTEMPT"])))
    try:
        # Keep only one downloaded large archive at a time.
        with tempfile.TemporaryDirectory() as temporary:
            archive=Path(temporary)/"artifact.zip"
            manifest=None
            source_hashes={}
            for artifact_id,pinned in EVIDENCE.items():
                metadata=json.loads(subprocess.check_output(["gh","api",f"repos/{REPOSITORY}/actions/artifacts/{artifact_id}"],text=True))
                if metadata.get("expired") or metadata.get("digest")!="sha256:"+pinned:
                    raise ValueError("Artifact metadata changed")
                with archive.open("wb") as dest:
                    subprocess.run(["gh","api",f"repos/{REPOSITORY}/actions/artifacts/{artifact_id}/zip"],stdout=dest,stderr=subprocess.DEVNULL,check=True)
                with archive.open("rb") as stream:
                    if digest(stream)!=pinned:
                        raise ValueError("Artifact ZIP hash mismatch")
                with zipfile.ZipFile(archive) as z:
                    if artifact_id==11293700607:
                        raw=z.read(member(z,"history-manifest.json"))
                        if hashlib.sha256(raw).hexdigest()!=MANIFEST_SHA:
                            raise ValueError("Manifest hash mismatch")
                        manifest=json.loads(raw)
                        if manifest["repository"]!=REPOSITORY:
                            raise ValueError("Manifest repository mismatch")
                        for source in manifest["artifacts"]:
                            aid=source["artifact_id"]
                            if aid not in EXPECTED: continue
                            for db in source["databases"]:
                                if Path(db["path"]).name!="native.sqlite": continue
                                with z.open(next(n for n in z.namelist() if n.endswith(str(aid)+"/native.sqlite"))) as stream:
                                    actual=digest(stream)
                                with z.open(next(n for n in z.namelist() if n.endswith(str(aid)+"/source.zip"))) as stream:
                                    zip_hash=digest(stream)
                                if actual!=db["sha256"] or zip_hash!=source["archive_sha256"]:
                                    raise ValueError("Original source hash mismatch")
                                source_hashes[aid]=(actual,zip_hash,db["source_uri"])
                        if set(source_hashes)!=set(EXPECTED):
                            raise ValueError("Final sources missing from original archive")
                        report["missing_artifact_gaps"]=len(manifest["gaps"])
                    elif artifact_id in (11510693007,11513473147,11515534653):
                        imported=json.loads(z.read(member(z,"import-report.json")))
                        if not imported.get("complete") or imported.get("history_complete") is not False or imported["manifest_sha256"]!=MANIFEST_SHA:
                            raise ValueError("Import report not verified/attributed")
                        for snapshot in imported["snapshots"]:
                            aid=snapshot["artifact_id"];count=EXPECTED[aid]
                            sha,zip_sha,source_uri=source_hashes[aid]
                            if (snapshot["state"]!="verified" or snapshot["verified_observation_mappings"]!=count
                                or snapshot["result"]["observations"]!=count or snapshot["sha256"]!=sha
                                or snapshot["archive_sha256"]!=zip_sha or snapshot["source_uri"]!=source_uri):
                                raise ValueError("Snapshot report parity/attribution mismatch")
                            with tempfile.TemporaryDirectory() as spool:
                                checked=inspect_outbox(z,aid,count,spool)
                            report["verified_snapshots"].append(dict(artifact_id=aid,observations=count,
                                sqlite_sha256=sha,source_zip_sha256=zip_sha,source_uri=source_uri,outbox=checked))
                    elif artifact_id==11522581310:
                        restored=json.loads(z.read(member(z,"restore-report.json")))
                        if not restored["restore_verified"] or not restored["independent_retention_verified"]:
                            raise ValueError("Fresh backup restore report failed")
                        report["fresh_backup_restore"]=restored
                report["stage"]="encrypted-preservation"
                key=prefix+"/"+str(artifact_id)+".zip"
                s3.upload_file(str(archive),uri.netloc,key,ExtraArgs={"ServerSideEncryption":"aws:kms","SSEKMSKeyId":os.environ["TEST262_BACKUP_KMS_KEY"]})
                head=s3.head_object(Bucket=uri.netloc,Key=key)
                version=head.get("VersionId")
                if not version or version=="null": raise ValueError("Preserved object lacks version")
                retention=s3.get_object_retention(Bucket=uri.netloc,Key=key,VersionId=version)["Retention"]
                if (head.get("ServerSideEncryption")!="aws:kms" or head.get("SSEKMSKeyId")!=os.environ["TEST262_BACKUP_KMS_KEY"]
                    or retention.get("Mode")!="COMPLIANCE" or retention["RetainUntilDate"]<=datetime.now(timezone.utc)):
                    raise ValueError("Preserved object encryption/retention failed")
                remote=s3.get_object(Bucket=uri.netloc,Key=key,VersionId=version)
                with remote["Body"] as stream:
                    if digest(stream)!=pinned: raise ValueError("Preserved exact-version bytes differ")
                report["preserved_objects"].append(dict(artifact_id=artifact_id,key=key,version_id=version,sha256=pinned,retain_until=retention["RetainUntilDate"].isoformat()))
                archive.unlink()
                print("Verified and preserved artifact",artifact_id,flush=True)
            if {s["artifact_id"] for s in report["verified_snapshots"]}!=set(EXPECTED):
                raise ValueError("Five snapshot verification incomplete")
        report.update(complete=True,stage="complete",final_legacy_checkpoint_disposition="retained-only, not imported")
    except Exception as error:
        report["error_class"]=type(error).__name__
        report["aws_error_code"]=getattr(error,"response",{}).get("Error",{}).get("Code")
        raise
    finally:
        report["checked_at"]=datetime.now(timezone.utc).isoformat()
        path=output/"evidence-report.json";path.write_text(json.dumps(report,indent=2)+"\n")
        print(json.dumps({"stage":report["stage"],"complete":report["complete"],"verified_snapshots":len(report["verified_snapshots"])}),flush=True)
    # Report itself is retained off-project, too. No raw dump/payload is printed.
    s3.upload_file(str(path),uri.netloc,prefix+"/evidence-report.json",ExtraArgs={"ServerSideEncryption":"aws:kms","SSEKMSKeyId":os.environ["TEST262_BACKUP_KMS_KEY"]})


if __name__=="__main__":
    try: main()
    except Exception as error:
        print("Evidence verification failed ("+type(error).__name__+").",flush=True)
        raise SystemExit(1)
